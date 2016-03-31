using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;

namespace ScrumMasterWcf
{
    public partial class ScrumMasterService
    {

        #region Basic Elements
        /// <summary>
        /// Stores the scrum-team name
        /// </summary>
        [DataMember]
        private string teamName = "No Name";
        /// <summary>
        /// Stores the scrum current sprint
        /// </summary>
        [DataMember]
        private Sprint currentSprint = null;
        /// <summary>
        /// Stores the all sprints that belongs to this scrum-process
        /// </summary>
        [DataMember]
        private List<Sprint> sprintList = null;
        /// <summary>
        /// Stores the members that belongs to this scrum-team
        /// </summary>
        [DataMember]
        private List<User> usersList = null;
        /// <summary>
        /// Stores the encrypted passwords of the team members 
        /// </summary>
        [DataMember]
        private SortedList<int, byte[]> usersEncryptedPasswords;
        /// <summary>
        /// Stores the scrum-master member
        /// </summary>
        [DataMember]
        private int scrumMasterUser = -1;
        /// <summary>
        /// Stores the manager member
        /// </summary>
        [DataMember]
        private int managerUser = -1;
        /// <summary>
        /// Stores the product-owner member
        /// </summary>
        [DataMember]
        private int productOwnerUser = -1;
        /// <summary>
        /// Stores the members who recently logged in (Using to quicken the authentication check)
        /// </summary>
        private List<User> logedInUsers = null;
        /// <summary>
        /// Stores the time (in miliscondes) that user stays in logedInUsers list
        /// </summary>
        [DataMember]
        private const int LoggedInTimeOutValue = 2000;
        /// <summary>
        /// Stores the time of the last update of the UserStorys list
        /// </summary>
        [DataMember]
        private DateTime ussUpdateTime = DateTime.Now;
        /// <summary>
        /// Stores the time of the last update of the Users list
        /// </summary>
        [DataMember]
        private DateTime usersUpdateTime = DateTime.Now;
        /// <summary>
        /// Stores the time of the last update of the Sprints list
        /// </summary>
        [DataMember]
        private DateTime sprintUpdateTime = DateTime.Now;
        /// <summary>
        /// Initializing new scrum-process team with the given name
        /// </summary>
        /// <param name="teamName">The name for the new team. Canno't be changed later.</param>
        public ScrumMasterService(string teamName)
        {
            this.teamName = teamName;
            sprintList = new List<Sprint>();
            usersList = new List<User>();
            usersEncryptedPasswords = new SortedList<int, byte[]>();
            logedInUsers = new List<User>();
            UpdateUsersPasswords();
        }
        #endregion
        #region Properties
        /// <summary>
        /// The name of the new scrum-process team
        /// </summary>
        public string TeamName
        {
            get
            {
                return teamName;
            }
        }
        /// <summary>
        /// Stores the sprint that the team work on right now
        /// </summary>
        public Sprint CurrentSprint
        {
            get
            {
                return currentSprint;
            }
        }
        /// <summary>
        /// Stores the ScrumMasterUser of this team
        /// </summary>
        public int ScrumMasterUser
        {
            get
            {
                return scrumMasterUser;
            }
        }
        /// <summary>
        /// Stores the list of the team-members
        /// </summary>
        public List<User> UsersList
        {
            get
            {
                return this.usersList;
            }
        }
        #endregion
        #region Security And permissions
        /// <summary>
        /// Adding the user to the now loggin users for TimeOutValue miliseconds in order to simplify the security tests
        /// </summary>
        /// <param name="usr">The user to add</param>
        private void SetAuthentication(User usr)
        {
            try
            {
                User rsltUser = usersList.First((x) => x.Name == usr.Name);
                if (rsltUser.ID == usr.ID)
                    if (rsltUser.ComparePassword(usr))
                    {
                        logedInUsers.Add(rsltUser);
                        Timer tmr = new Timer(LoggedInTimeOutValue);
                        tmr.Elapsed += delegate (object sender, ElapsedEventArgs e) { logedInUsers.Remove(rsltUser); };
                    }
            }
            catch
            {

            }
        }
        /// <summary>
        /// Checking if the given user have been authenticated well, and try to authenticate him if not.
        /// </summary>
        /// <param name="usr"> The User to check</param>
        /// <returns>true - if the user authentication success. false - if it fails</returns>
        private bool CheckAuthentication(User usr)
        {
            try
            {
                if (logedInUsers.FirstOrDefault((x) => x.ID == usr.ID) != null) return true;
                User clientUser = GetUser(usr.Name, usr.Password);
                if (clientUser == null || clientUser.Name == "113") return false;
                SetAuthentication(usr);
                return true;
            }
            catch
            {

            }
            return false;

        }
        /// <summary>
        /// Check if the given User have permissins to execute the given operation
        /// </summary>
        /// <param name="operationNameLowerCase">The requested operation name, normalized to lower cases</param>
        /// <param name="usr">The User to check</param>
        /// <returns>true - if the user have permission to the action. false - if he not having permission for it</returns>
        private bool CheckPermission(string operationNameLowerCase, User usr)
        {
            switch (operationNameLowerCase)
            {
                case "createnewuser":
                    if (usr.Positions.Contains(User.Position.Manager)) return true;
                    return false;
                case "createnewuserstory":
                    if (usr.Positions.Contains(User.Position.ProductOwner)) return true;
                    return false;
                case "updateuserstory":
                    if (usr.Positions.Contains(User.Position.ProductOwner)) return true;
                    return false;
                case "removeuserstory":
                    if (usr.Positions.Contains(User.Position.ProductOwner)) return true;
                    return false;
                case "createnewsprint":
                    if (usr.Positions.Contains(User.Position.ScrumMaster)) return true;
                    return false;
                case "updateexistinguser":
                    if (usr.Positions.Contains(User.Position.Manager)) return true;
                    return false;
                case "removeuser":
                    if (usr.Positions.Contains(User.Position.Manager)) return true;
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Runs the security checks for given user and operation
        /// </summary>
        /// <param name="actUser">Object which representing the user who wants to do the operation</param>
        /// <param name="opNameLower">The operation name that requested by user</param>
        /// <returns></returns>
        private bool SecurityCheck(object actUser, string opNameLower)
        {
            if (actUser.GetType() != typeof(User) || !CheckAuthentication((User)actUser) || !CheckPermission(opNameLower, (User)actUser))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
