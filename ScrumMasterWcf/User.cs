using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Represent a team-member in the scrum-team
    /// </summary>
    [DataContract]
    public class User : IHaveID
    {
        /// <summary>
        /// The possible positions that user can have.
        /// One can have more than one position.
        /// ScrumMaster position can asign to only one user in the team.
        /// </summary>
        public enum Position : int
        {
            /// <summary>
            /// Means that the user is a Customer
            /// </summary>
            Customer = 0,
            /// <summary>
            /// Means that the user is a Product-Owner
            /// </summary>
            ProductOwner,
            /// <summary>
            /// Means that the user is a Manager
            /// </summary>
            Manager,
            /// <summary>
            /// Means that the user is a Scrum-Master
            /// </summary>
            ScrumMaster,
            /// <summary>
            /// Means that the user is a Developer
            /// </summary>
            Developer,
            /// <summary>
            /// Means that the user is an Architect
            /// </summary>
            Architect,
            /// <summary>
            /// Means that the user is a QA
            /// </summary>
            QA,
            /// <summary>
            /// Means that the user is a QC
            /// </summary>
            QC
        }
        /// <summary>
        /// The encoder of the passwords.
        ///  It encrypting them so one cann't know them by breaking to the server.
        /// </summary>
        static SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
        static UnicodeEncoding encoding = new UnicodeEncoding();
        /// <summary>
        /// Stores the last ID number that was given to user.
        /// For scrum saving and opening purpose.
        /// </summary>
        static int lastID = 0;
        /// <summary>
        /// Stores the user ID
        /// </summary>
        [DataMember]
        private int id;
        /// <summary>
        /// Stores the user name
        /// </summary>
        [DataMember]
        private string name;
        /// <summary>
        /// Stores the user positions
        /// </summary>
        [DataMember]
        private Position[] Pos;
        /// <summary>
        /// Returns the user positions
        /// </summary>
        public Position[] Positions
        {
            get { return Pos; }
            set {
                if (UsersEncryptedPasswords == null) //So it's the client
                    Pos = value;
            }
        }
        /// <summary>
        /// Exposes the last ID number that was given to user.
        /// For scrum saving and opening purpose.
        /// Therefore one can set LastID only when LastID==0,
        ///  which means that there is new scrum process running.
        /// </summary>
        public static int LastId
        {
            get { return lastID; }
            set
            {
                Console.WriteLine("{0}", lastID);
                if (lastID == 0) lastID = value;
            }
        }
        /// <summary>
        /// Returns the user ID number
        /// </summary>
        public int ID
        {
            get { return this.id; }
        }
        /// <summary>
        /// Returns the user name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set {
                if (UsersEncryptedPasswords == null) //So it's the client
                    name = value;
            }
        }
        /// <summary>
        /// NOT stores the password of the user in the server.
        /// Using JUST for pass a temporal information like authentication from client.
        /// The users passwords are saving in the UsersEncryptedPasswords data structure.
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// Saves the user password secure by encrypting it.
        /// </summary>
        /// <param name="rawPassword">The password to encrypt for the user</param>
        public void SetPassword(string rawPassword)
        {
            if (UsersEncryptedPasswords != null) //So it's the server
                UsersEncryptedPasswords[this.ID] = HashPassword(rawPassword);
            else 
                this.Password = rawPassword;

        }
        /// <summary>
        /// Link to the UsersEncryptedPasswords data structure in the server.
        /// Not defined in this class, but in ScrumMasterService class.
        /// </summary>
        public static SortedList<int, byte[]> UsersEncryptedPasswords { get; internal set; }
        /// <summary>
        /// Returns the tasks that blongs to this user in the given sprint
        /// </summary>
        /// <param name="sprint">Sprint to search task in</param>
        /// <returns>The tasks that blongs to this user in the given sprint</returns>
        public List<ScrumTask> GetTasks(Sprint sprint)
        {
            List<ScrumTask> rtrnTsks = new List<ScrumTask>();
            foreach (UserStory us in sprint.UserStorys)
                foreach (ScrumTask st in us.ScrumTasks)
                    if (st.HandleUserID == this.id)
                        rtrnTsks.Add(st);
            return rtrnTsks;


        }
        /// <summary>
        /// Creates new user with the given details
        /// </summary>
        /// <param name="name">The new user name</param>
        /// <param name="password">The new user password (will be encrypted)</param>
        /// <param name="pst">The new user positions</param>
        public User(string name, string password, Position[] pst)
        {
            // We not accepting numbers as names, because numbers as user-names indicating errors
            try
            {
                Convert.ToInt32(name);
                return;
            }
            catch
            {

            }
            this.name = name;
            this.Pos = pst;
            if (password != "Error")
            // "Error" notify the client that there is a problem
            {
                this.id = ++lastID;
                SetPassword(password);

            }
        }
        /// <summary>
        /// Checks if given string is the user password
        /// </summary>
        /// <param name="password">String to check</param>
        /// <returns>true - if it is the user password. false - otherwise</returns>
        public bool CheckPassword(string password)
        {
            byte[] param = HashPassword(password);
            return CompareHashedPassword(param);
        }
        /// <summary>
        /// Checks id given encryption of string equals to this user encrypted password
        /// </summary>
        /// <param name="param">Encryption of string</param>
        /// <returns>true - if they identical. false - otherwise</returns>
        private bool CompareHashedPassword(byte[] param)
        {
            if (UsersEncryptedPasswords == null) return false;
            byte[] password = UsersEncryptedPasswords[this.ID];
            for (int i = 0; i < password.Length; i++)
                if (param[i] != password[i])
                    return false;
            if (param.Length != password.Length)
                return false;
            return true;
        }
        /// <summary>
        /// Check if the password in the Password property (not encrypted password)
        ///  in the user object is the correct password of this user.
        /// </summary>
        /// <param name="usr">The given User object with information in Password property</param>
        /// <returns>true - if is the correct password. false - otherwise</returns>
        public bool ComparePassword(User usr)
        {
            return CheckPassword(usr.Password);
        }
        /// <summary> 
        ///  It encrypting the password so one cann't know them by breaking into the server.
        /// </summary>
        /// <param name="password">String to encrypt as password</param>
        /// <returns>The encrypted password</returns>
        public static byte[] HashPassword(string password)
        {
            return provider.ComputeHash(encoding.GetBytes(password));
        }

        public void UpdateDetails(User newUserDetails)
        {
            if (newUserDetails == null) return;
            this.name = newUserDetails.name;
            this.Pos = newUserDetails.Pos;
            if (newUserDetails.Password != "Error")
            // "Error" notify the client that there is a problem
            {                
                this.SetPassword(newUserDetails.Password);
            }
        }
    }
}
