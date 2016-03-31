using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Exposing methods to the clients, so they can remote access to the scrum proccess.
    /// If method needs security access or specific user-role, the client will have
   /// to send User object which contains his password in the public Password property.
    /// </summary>
    [ServiceContract]
    public interface IScrumMasterService
    {
        /// <summary>
        /// Exposing the name of the team of current scrum-team
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns>Team's name </returns>
        [OperationContract]
        string GetTeamName(User actUser);
        /// <summary>
        /// Returns list of the users who registering to the scrum-team
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns>Teams members, including thier roles, names, etc.</returns>
        [OperationContract]
        List<User> GetUsersList(User actUser);
        /// <summary>
        /// Create new user in the team with given details
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="password"></param>
        /// <param name="actUser">Security access data. Only manager can adding users to team</param>
        /// <returns>The new user</returns>
        [OperationContract]
        User CreateNewUser(string name, User.Position[] pos, string password, User actUser);
        /// <summary>
        ///  Insert new UserStory to the scrum proccess with given details
        /// </summary>
        /// <param name="name">The UserStory name</param>
        /// <param name="description">The UserStory description</param>
        /// <param name="priority">The priority of this UserStory</param>
        /// <param name="actUser">Security access data</param>
        /// <returns>The new UserStory</returns>
        [OperationContract]
        UserStory CreateNewUserStory(string name, string description, int priority, User actUser);
        /// <summary>
        /// Insert new sprint to the scrum proccess with given details
        /// </summary>
        /// <param name="newSprintStart"></param>
        /// <param name="newSprintDuration"></param>
        /// <param name="actUser">Security access data. Only scrum-master can create new sprint</param>
        /// <returns>The new sprint</returns>
        [OperationContract]
        Sprint CreateNewSprint(Nullable<DateTime> newSprintStart, double newSprintDuration, User actUser);
        /// <summary>
        /// Returns the server current sprint
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns></returns>
        [OperationContract]
        Sprint GetCurrentSprint(User actUser);
        /// <summary>
        /// The log on method.
        /// </summary>
        /// <param name="userName">The asking client user name</param>
        /// <param name="password">The asking client password</param>
        /// <returns></returns>
        [OperationContract]
        User GetUser(string userName, string password);
        /// <summary>
        /// Returns the scrum-master user of the team
        /// </summary>
        /// <param name="actUser">Security access data</param>
        /// <returns></returns>
        [OperationContract]
        int GetScrumMasterUser(User actUser);
        /// <summary>
        /// Insert new task to scrum proccess with the given details
        /// </summary>
        /// <param name="nus">The parent UserStory of the task</param>
        /// <param name="name">Task's name</param>
        /// <param name="description">Task's description</param>
        /// <param name="plannedEffort">How many days the task attended take.</param>
        /// <param name="handlerUserID">The ID of the users who will handle this task. -1 is possible</param>
        /// <param name="actUser">Security access data</param>
        /// <returns>The new ScrumTask with these details or null if error occured</returns>
        [OperationContract]
        ScrumTask CreateNewScrumTask(UserStory nus, string name, string description, double plannedEffort, int handlerUserID, User actUser);
        /// <summary>
        /// Binding given task to given user
        /// </summary>
        /// <param name="st">The tak to be binding to user</param>
        /// <param name="currentUser">The user who intersting in the task and security access data</param>
        [OperationContract]
        void BindTaskToUser(ScrumTask st, User currentUser);
        /// <summary>
        /// Changes given job-status to new status.
        /// </summary>
        /// <param name="job">The Job to change</param>
        /// <param name="jSta">The updated job status</param>
        /// <param name="actUser">Security access data. In addition, only product-owner can mark User Story as Accepted</param>
        [OperationContract]
        void ChangeJobStatus(Job job, Job.JobStatuses jSta, User actUser);
        /// <summary>
        /// Sets the user capacity (days of work in current sprint) to newCap
        /// </summary>
        /// <param name="newCap">The updated capacity in the current sprint</param>
        /// <param name="actUser">The user to update his capacity, and security access data</param>
        [OperationContract]
        void SetUserCapacity(double newCap, User actUser);
        /// <summary>
        /// Return the sprint which preceded to clientCurrentSprint
        /// </summary>
        /// <param name="clientCurrentSprint">The sprint which the user see now (not equal to server current sprint)</param>
        /// <param name="actUser">Security access data</param>
        /// <returns></returns>
        [OperationContract]
        Sprint GetPreviosSprint(int clientCurrentSprint, User actUser);
        /// <summary>
        /// Return the sprint which following to clientCurrentSprint
        /// </summary>
        /// <param name="clientCurrentSprint">The sprint which the user sees now (not equal to server current sprint)</param>
        /// <param name="actUser"></param>
        /// <returns></returns>
        [OperationContract]
        Sprint GetNextSprint(int clientCurrentSprint, User actUser);
        /// <summary>
        /// Check if the UserStorys changed since updateFlag time.
        /// </summary>
        /// <param name="updateFlag">The time to compare</param>
        /// <returns> true- if UserStorys changed. false - UserStorys didn't changed </returns>
        [OperationContract]
        bool IsUssChangedSince(DateTime updateFlag);
        /// <summary>
        /// Check if the users list changed since updateFlag time.
        /// </summary>
        /// <param name="updateFlag">The time to compare</param>
        /// <returns> true- if users list changed. false - users list didn't changed </returns>
        [OperationContract]
        bool IsUsersChangedSince(DateTime updateFlag);
        /// <summary>
        /// Check if current sprint changed since updateFlag time.
        /// </summary>
        /// <param name="updateFlag">The time to compare</param>
        /// <returns> true- if current sprint changed. false - current sprint didn't changed </returns>
        [OperationContract]
        bool IsSprintChangedSince(DateTime updateFlag);
        /// <summary>
        /// Removes user from users list.
        /// </summary>
        /// <param name="userId">The ID of the user that need to be removed</param>
        /// <param name="actUser"></param>
        /// <returns>True if the required user deleted successfuly. False if an error occurred.</returns>
        [OperationContract]
        bool RemoveUser(int userId, User actUser);
        /// <summary>
        /// Updating details of user which exist in the users list.
        /// </summary>
        /// <param name="userId">The ID of the user who need to be updating</param>
        /// <param name="newUserDetails">User object which contains the new details for the exists user. All the fields will be copied to the exsiting user.</param>
        /// <param name="actUser"></param>
        /// <returns>The updated user object. Null if something got wrong.</returns>
        [OperationContract]
        User UpdateExistingUser(int userId, User newUserDetails, User actUser);
        /// <summary>
        /// Allows the product-owner updates a specific UserStory
        /// </summary>
        /// <param name="usId">The ID of the UserStory that should be updated</param>
        /// <param name="sprintId">The ID of the sprint which contains the UserStory</param>
        /// <param name="newUSDetails">Contains the all fields for the update.
        ///  Every field in the US will be replaced with the corresponding field in newUSDetails object</param>
        /// <param name="actUser"></param>
        /// <returns>The updated UserStory or null if somwthing got wrong</returns>
        [OperationContract]
        UserStory UpdateUserStory(int usId, int sprintId, UserStory newUSDetails, User actUser);
        /// <summary>
        /// Removes US from a given Sprint USs list
        /// </summary>
        /// <param name="usId">The ID of the US that should be removed</param>
        /// <param name="sprintId">The ID of the Sprint which contains the US</param>
        /// <param name="actUser"></param>
        /// <returns>True if the required US deleted successfuly. False if an error occurred.</returns>
        [OperationContract]
        bool RemoveUserStory(int usId, int sprintId, User actUser);
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
        [OperationContract]
        ScrumTask UpdateScrumTask(int usId, int sprintId, int stId, ScrumTask newSTDetails, User actUser);
        /// <summary>
        /// Allows the users updates a specific ScrumTask
        /// </summary>
        /// <param name="usId">The ID of the UserStory which contains the ScrumTask</param>
        /// <param name="sprintId">The ID of the sprint which contains the UserStory</param>
        /// <param name="stId">The ID of the ScrumTask that should be removed</param>
        /// <param name="actUser"></param>
        /// <returns>True if the required ST was deleted successfuly. False if an error occurred.</returns>
        [OperationContract]
        bool RemoveScrumTask(int usId, int sprintId, int stId, User actUser);
    }

}
