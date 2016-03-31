using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ScrumMasterWcf
{   
    /// <summary>
    /// Class which implemented the demands for manage scrum-process.
    /// Implementing IScrumMasterService interface to handle the server-client communication.
    /// Implementing IParameterInspector and IEndpointBehavior interfaces to handle
    /// security checks before and after every call.
    /// Intended to communicate to the all team-members so it defines InstanceContextMode = InstanceContextMode.Single
    /// </summary>
    [DataContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class ScrumMasterService : IScrumMasterService, IParameterInspector, IEndpointBehavior
    {
        // Description of these methods can be found in the IScrumMasterService interface code
        // Every method that need permissions to act, starting with SecurityCheck
        #region IScrumMasterService Members
        /// <summary>
        /// Create new sprint and set it as the current sprint
        /// </summary>
        /// <param name="newSprintStart">The date when the sprint start at.</param>
        /// <param name="newSprintDuration">The duration (in days, can be fragmants) of the sprint</param>
        /// <param name="actUser">The user that runs the function</param>
        /// <returns>The new sprint OR null in error</returns>
        public Sprint CreateNewSprint(Nullable<DateTime> newSprintStart, double newSprintDuration, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;

            Sprint newSpr = null;
            if (CurrentSprint == null)
                // The first sprint
                newSpr = new Sprint(newSprintStart.GetValueOrDefault(), newSprintDuration);
            else
                // From the second sprint and on, every new sprint need to know the before and after sprints
                newSpr = currentSprint.CreateNextSprint(newSprintStart.GetValueOrDefault(), newSprintDuration);
            sprintList.Add(newSpr);
            currentSprint = newSpr;
            return CurrentSprint;
        }
        /// <summary>
        /// Returns the sprint that came before the sprint with the id = clientCurrentSprintId
        /// </summary>
        /// <param name="clientCurrentSprintId">The sprint to refer as a basis</param>
        /// <param name="actUser">The user that runs the function</param>
        /// <returns>The sprint that came before clientCurrentSprint</returns>
        public Sprint GetPreviosSprint(int clientCurrentSprintId, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;

            Sprint clientCurrentSprint = ChangeSprint((x) => x.ID == clientCurrentSprintId);
            if (clientCurrentSprint.PrevSprintID == -1) return clientCurrentSprint;
            return ChangeSprint((x) => x.ID == clientCurrentSprint.PrevSprintID);
        }
        /// <summary>
        /// Returns the sprint that came after the sprint with the id = clientCurrentSprintId
        /// </summary>
        /// <param name="clientCurrentSprintId">The sprint to refer as a basis</param>
        /// <param name="actUser">The user that runs the function</param>
        /// <returns>The sprint that came after clientCurrentSprint</returns>
        public Sprint GetNextSprint(int clientCurrentSprintId, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;

            Sprint clientCurrentSprint = ChangeSprint((x) => x.ID == clientCurrentSprintId);
            return ChangeSprint((x) => x.ID == clientCurrentSprint.NxtSprintID);
        }
        /// <summary>
        /// Search sprint that fulfill newSprint=true. 
        /// </summary>
        /// <param name="newSprint"> Function to determine the alternative sprint. Gets Sprint object and returns boolean value</param>
        /// <returns>The alternative sprint</returns>
        private Sprint ChangeSprint(Func<Sprint, bool> newSprint)
        {
            Sprint tmpSprint = this.currentSprint;
            try
            {
                tmpSprint = sprintList.FirstOrDefault(newSprint);
                if (tmpSprint == null)
                    // newSprint not found. Returns the default which is the server current sprint.
                    tmpSprint = currentSprint;
            }
            catch
            {
                // Error occurred. Returns the default which is the server current sprint.
                tmpSprint = currentSprint;
            }
            return tmpSprint;
        }
        /// <summary>
        /// Create new team-member
        /// </summary>
        /// <param name="name">The name of the new member</param>
        /// <param name="pos">The positions of the new member</param>
        /// <param name="password">The password of the new member</param>
        /// <param name="actUser">The user that runs the function</param>
        /// <returns>The new team-member</returns>
        public User CreateNewUser(string name, User.Position[] pos, string password, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            try
            {
                int nameAsInt = -1;
                if (int.TryParse(name, out nameAsInt))
                    // Name cann't be number
                    throw new CustomException("User-name cannot be number", 117);
                User usr = usersList.FirstOrDefault((x) => x.Name == name);
                if (usr != null && this.UsersList.Count > 1)
                    // If UsersList.Count==1, only the manager existing and we will allow it be duplicate
                    throw new CustomException("The user-name already exist", 114);
                bool[] posLeagal = CheckPositionsLegalization(pos);

                var newUser = new User(name, password, pos);
                this.usersList.Add(newUser);
                UpdateSpecialUsers(posLeagal, newUser);
                return newUser;
            }
            catch (CustomException csex)
            {
                return new User(csex.ExceptionCode.ToString(), "Error", pos);
            }
        }
        /// <summary>
        /// Update the special users in the team if one of them need to.
        /// </summary>
        /// <param name="posLeagal">Array of 3 boolean values which represents which of the special positions the newUser owns.</param>
        /// <param name="newUser">The user that candidate to be special user.</param>
        private void UpdateSpecialUsers(bool[] posLeagal, User newUser)
        {
            if (posLeagal[0]) this.managerUser = newUser.ID;
            if (posLeagal[1]) this.scrumMasterUser = newUser.ID;
            if (posLeagal[2]) this.productOwnerUser = newUser.ID;
        }
        /// <summary>
        /// Checks if the given positions can be assigend to user. If one of them is illegal an CustomException will be thrown.
        /// </summary>
        /// <param name="pos">The list of the positions to check</param>
        /// <param name="userId">The ID number of the user to assign the positions to. 
        /// Optional parameter. In use when updating existing user and check if he is special user.</param>
        /// <returns>Array of boolean variables. The value in the [0] is true if Manager is in the positions.
        /// The The value in the [1] is true if ScrumMaster is in the positions.
        /// The value in the [2] is true if ProductOwner is in the positions.
        /// The value in the [3] is true if the conclusion of the legalization check is positive. </returns>
        private bool[] CheckPositionsLegalization(User.Position[] pos, int userId = -1)
        {
            bool[] posExists = new bool[4];
            var isManagerInPos = pos.Where((x) => x == User.Position.Manager).Count() > 0 ? true : false;
            var isScrumMasterInPos = pos.Where((x) => x == User.Position.ScrumMaster).Count() > 0 ? true : false;
            var isProductOwnerInPos = pos.Where((x) => x == User.Position.ProductOwner).Count() > 0 ? true : false;
            if ((managerUser == -1 || managerUser == userId) && !isManagerInPos)
            {
                throw new CustomException("There is no Manager.", 111);
            }
            if (managerUser != -1 && managerUser != userId && isManagerInPos)
            {
                throw new CustomException("Manager user is already exist.", 112);
            }
            if (scrumMasterUser != -1 && scrumMasterUser != userId && isScrumMasterInPos)
            {
                throw new CustomException("Scrum-master user is already exist.", 115);
            }
            if (productOwnerUser != -1 && productOwnerUser != userId && isProductOwnerInPos)
            {
                throw new CustomException("Product-owner user is already exist.", 116);
            }
            posExists[0] = isManagerInPos;
            posExists[1] = isScrumMasterInPos;
            posExists[2] = isProductOwnerInPos;
            posExists[3] = true;
            return posExists;
        }

        /// <summary>
        /// Create new UserStory in the current sprint
        /// </summary>
        /// <param name="name">The name/header of the new UserStory</param>
        /// <param name="description">The description of the new UserStory</param>
        /// <param name="priority">The priority of the new UserStory</param>
        /// <param name="actUser">The user that runs the function</param>
        /// <returns>The new UserStory</returns>
        public UserStory CreateNewUserStory(string name, string description, int priority, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            UserStory newUS = new UserStory(name, description, priority);
            CurrentSprint.AddUS(newUS);
            return newUS;
        }
        /// <summary>
        /// Create new task in the given UserStory
        /// </summary>
        /// <param name="nus">The UserStory that owns the task</param>
        /// <param name="name">The name/header of the new task</param>
        /// <param name="description">The description of the new task</param>
        /// <param name="plannedEffort">The planned effort (in days, can fragmantes) of the new task</param>
        /// <param name="handlerUserID">The ID of the user who will handle the new task. -1 if not defined right now</param>
        /// <param name="actUser">The user that runs the function</param>
        /// <returns>The new task</returns>
        public ScrumTask CreateNewScrumTask(UserStory nus, string name, string description, double plannedEffort, int handlerUserID, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            ScrumTask nst = new ScrumTask(name, description, plannedEffort, handlerUserID);
            CurrentSprint.UserStorys.FirstOrDefault((x) => x.ID == nus.ID).AddTask(nst);
            return nst;
        }

        /// <summary>
        /// Returns the user with the given name and password
        /// </summary>
        /// <param name="userName">The name of the required user</param>
        /// <param name="password">The password of the required user</param>
        /// <returns>The required user</returns>
        public User GetUser(string userName, string password)
        {
            User usr = null;
            try
            {
                usr = usersList.First((x) => x.Name == userName);

                if (!usr.CheckPassword(password))
                {

                    throw new CustomException("Wrong password", 113);

                }
            }
            catch (CustomException cex)
            {
                switch (cex.ExceptionCode)
                {
                    case 113:
                        User.Position[] posa = { User.Position.Developer };
                        usr = new User("113", "Error", posa);
                        break;
                }
            }
            catch
            {
                usr = null;
            }
            return usr;

        }
        /// <summary>
        /// Returns the user with the given ID
        /// </summary>
        /// <param name="userID">The ID of the required user</param>
        /// <returns>The required user</returns>
        internal User GetUser(int userID)
        {
            return usersList.First((x) => x.ID == userID);
        }
        /// <summary>
        /// Inform the User class where are the encrypted passwords of the users.
        /// </summary>
        public void UpdateUsersPasswords()
        {
            User.UsersEncryptedPasswords = this.usersEncryptedPasswords;
        }

        /// <summary>
        /// Binding given task to given user
        /// </summary>
        /// <param name="st">The tak to be binding to user</param>
        /// <param name="user">The user who intersting in the task and security access data</param>
        public void BindTaskToUser(ScrumTask st, User user)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(user, thisMethodName)) return;
            st.BindTaskToUser(user);
        }
        /// <summary>
        /// Changes given job-status to new status.
        /// </summary>
        /// <param name="job">The Job to change</param>
        /// <param name="jSta">The updated job status</param>
        /// <param name="usr">Security access data. In addition, only product-owner can mark User Story as Accepted</param>
        public void ChangeJobStatus(Job job, Job.JobStatuses jSta, User usr)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(usr, thisMethodName)) return;
            if (jSta != Job.JobStatuses.Accepted || usr.Positions.Contains(User.Position.ProductOwner))
                job.ChangeStatus(jSta);
        }
        /// <summary>
        /// Sets the user capacity (days of work in current sprint) to newCap
        /// </summary>
        /// <param name="newCap">The updated capacity in the current sprint</param>
        /// <param name="actUser">The user to update his capacity, and security access data</param>
        public void SetUserCapacity(double newCap, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return;
            this.CurrentSprint.SetUserCapacity(actUser.ID, newCap);
        }

        /// <summary>
        /// Check if the UserStorys changed since updateFlag time.
        /// </summary>
        /// <param name="updateFlag">The time to compare</param>
        /// <returns> true- if UserStorys changed. false - UserStorys didn't changed </returns>
        public bool IsUssChangedSince(DateTime updateFlag)
        {
            return (this.ussUpdateTime > updateFlag);
        }
        /// <summary>
        /// Check if the users list changed since updateFlag time.
        /// </summary>
        /// <param name="updateFlag">The time to compare</param>
        /// <returns> true- if users list changed. false - users list didn't changed </returns>
        public bool IsUsersChangedSince(DateTime updateFlag)
        {
            return (this.usersUpdateTime > updateFlag);
        }
        /// <summary>
        /// Check if current sprint changed since updateFlag time.
        /// </summary>
        /// <param name="updateFlag">The time to compare</param>
        /// <returns> true- if current sprint changed. false - current sprint didn't changed </returns>
        public bool IsSprintChangedSince(DateTime updateFlag)
        {
            return (this.sprintUpdateTime > updateFlag);
        }

        /// <summary>
        /// Exposing the name of the team of current scrum-team
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns>Team's name </returns>
        public string GetTeamName(User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            return this.TeamName;
        }
        /// <summary>
        /// Returns list of the users who registering to the scrum-team
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns>Teams members, including thier roles, names, etc.</returns>
        public List<User> GetUsersList(User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            return this.UsersList;
        }
        /// <summary>
        /// Returns the server current sprint
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns></returns>
        public Sprint GetCurrentSprint(User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            return this.CurrentSprint;
        }
        /// <summary>
        /// Returns the scrum-master user of the team
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns></returns>
        public int GetScrumMasterUser(User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return -1;
            return this.ScrumMasterUser;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newUserDetails"></param>
        /// <param name="actUser"></param>
        /// <returns></returns>
        public User UpdateExistingUser(int userId, User newUserDetails, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            User localUser = UsersList.FirstOrDefault((x) => x.ID == userId);
            try
            {
                bool[] posLegal = CheckPositionsLegalization(newUserDetails.Positions, localUser.ID);
                if (localUser != null)
                {
                    if (!posLegal[3]) return localUser;
                    localUser.UpdateDetails(newUserDetails);
                    UpdateSpecialUsers(posLegal, localUser);
                }
            }
            catch (CustomException csex)
            {
                return new User(csex.ExceptionCode.ToString(), "Error", newUserDetails.Positions);
            }
            return localUser;
        }
        /// <summary>
        /// Removes user from users list.
        /// </summary>
        /// <param name="userId">The ID of the user that need to be removed</param>
        /// <param name="actUser"></param>
        /// <returns>True if the required user deleted successfuly. False if an error occurred.</returns>
        public bool RemoveUser(int userId, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return false;
            if (managerUser == userId) return false;
            User localUser = UsersList.FirstOrDefault((x) => x.ID == userId);
            if (localUser == null) return false;

            if (scrumMasterUser == userId) scrumMasterUser = -1;
            if (productOwnerUser == userId) productOwnerUser = -1;

            this.usersEncryptedPasswords.Remove(userId);
            do
            {
                UsersList.Remove(localUser);
                localUser = null;
                // Check if the user was deleted correctly
                localUser = UsersList.FirstOrDefault((x) => x.ID == userId);
            } while (localUser != null);

            return true;
        }
        /// <summary>
        /// Allows the product-owner updates a specific UserStory
        /// </summary>
        /// <param name="usId">The ID of the UserStory that should be updated</param>
        /// <param name="sprintId">The ID of the sprint which contains the UserStory</param>
        /// <param name="newUSDetails">Contains the all fields for the update.
        ///  Every field in the US will be replaced with the corresponding field in newUSDetails object</param>
        /// <param name="actUser"></param>
        /// <returns>The updated UserStory or null if somwthing got wrong</returns>
        public UserStory UpdateUserStory(int usId, int sprintId, UserStory newUSDetails, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            Sprint localSprint = this.sprintList.FirstOrDefault((x) => x.ID == sprintId);
            if (localSprint == null) return null;
            UserStory localUS = localSprint.UserStorys.FirstOrDefault((x) => x.ID == usId);
            if (localUS == null) return null;

            localUS.UpdateDetails(newUSDetails);

            return localUS;
        }
        /// <summary>
        /// Removes US from a given Sprint USs list
        /// </summary>
        /// <param name="usId">The ID of the US that should be removed</param>
        /// <param name="sprintId">The ID of the Sprint which contains the US</param>
        /// <param name="actUser"></param>
        /// <returns>True if the required US deleted successfuly. False if an error occurred.</returns>
        public bool RemoveUserStory(int usId, int sprintId, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return false;
            Sprint localSprint = this.sprintList.FirstOrDefault((x) => x.ID == sprintId);
            if (localSprint == null) return false;
            UserStory localUS = localSprint.UserStorys.FirstOrDefault((x) => x.ID == usId);
            if (localUS == null) return false;
            localSprint.UserStorys.Remove(localUS);
            return true;
        }
        /// <summary>
        /// Allows the users updates a specific ScrumTask
        /// </summary>
        /// <param name="usId">The ID of the UserStory which contains the ScrumTask</param>
        /// <param name="sprintId">The ID of the sprint which contains the UserStory</param>
        /// <param name="stId">The ID of the ScrumTask that should be updated</param>
        /// <param name="newSTDetails">Contains the all fields for the update.
        ///  Every field in the ST will be replaced with the corresponding field in newSTDetails object</param>
        /// <param name="actUser"></param>
        /// <returns>The updated ScrumTask or null if somwthing got wrong</returns>
        public ScrumTask UpdateScrumTask(int usId, int sprintId, int stId, ScrumTask newSTDetails, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return null;
            Sprint localSprint = this.sprintList.FirstOrDefault((x) => x.ID == sprintId);
            if (localSprint == null) return null;
            UserStory localUS = localSprint.UserStorys.FirstOrDefault((x) => x.ID == usId);
            if (localUS == null) return null;
            ScrumTask localST = localUS.ScrumTasks.FirstOrDefault((x) => x.ID == stId);
            if (localST == null) return null;
            localST.UpdateDetails(newSTDetails);
            return localST;
        }
        /// <summary>
        /// Allows the users updates a specific ScrumTask
        /// </summary>
        /// <param name="usId">The ID of the UserStory which contains the ScrumTask</param>
        /// <param name="sprintId">The ID of the sprint which contains the UserStory</param>
        /// <param name="stId">The ID of the ScrumTask that should be removed</param>
        /// <param name="actUser"></param>
        /// <returns>True if the required ST was deleted successfuly. False if an error occurred.</returns>
        public bool RemoveScrumTask(int usId, int sprintId, int stId, User actUser)
        {
            var thisMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            if (!SecurityCheck(actUser, thisMethodName)) return false;
            Sprint localSprint = this.sprintList.FirstOrDefault((x) => x.ID == sprintId);
            if (localSprint == null) return false;
            UserStory localUS = localSprint.UserStorys.FirstOrDefault((x) => x.ID == usId);
            if (localUS == null) return false;
            ScrumTask localST = localUS.ScrumTasks.FirstOrDefault((x) => x.ID == stId);
            if (localST == null) return false;
            localUS.ScrumTasks.Remove(localST);
            return true;
        }
        #endregion
    }
}
