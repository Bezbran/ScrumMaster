using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace ScrumMasterWcf
{
    /// <summary>
    /// Representing a sprint (a time period of scrum working) in scrum process
    /// </summary>
    [DataContract]
    public class Sprint : IHaveID
    {
        /// <summary>
        /// Stores the last ID number that was given to sprint.
        /// For scrum saving and opening purpose.
        /// </summary>
        static int lastId = 0;
        /// <summary>
        /// Exposes the last ID number that was given to task.
        /// For scrum saving and opening purpose.
        /// Therefore one can set LastID only when LastID==0,
        ///  which means that there is new scrum process running.
        /// </summary>
        public static int LastId
        {
            get { return lastId; }
            set
            {
                Console.WriteLine("{0}", lastId);
                if (lastId == 0) lastId = value;
            }
        }
        /// <summary>
        /// Stores the sprint ID
        /// </summary>
        [DataMember]
        private int id;
        /// <summary>
        /// Returns the job ID
        /// </summary>
        public int ID
        {
            get
            {
                return id;
            }
        }
        /// <summary>
        /// Stores the duration (in days, can be fractions) of this sprint
        /// </summary>
        [DataMember]
        private double duration;
        /// <summary>
        /// Stores the date when this sprint starting
        /// </summary>
        [DataMember]
        private DateTime start;
        /// <summary>
        /// Stores the UserStorys that have to be done in this sprint
        /// </summary>
        [DataMember]
        private List<UserStory> uss;
        /// <summary>
        /// Stores the ID of the sprint which preceded this sprint
        /// </summary>
        [DataMember]
        private int prevSprintID = -1;
        /// <summary>
        /// Stores the ID of the sprint which following this sprint
        /// </summary>
        [DataMember]
        private int nxtSprintID = -1;
        /// <summary>
        /// Stores for each user how many days (can be fractions) he have in this sprint.
        /// The ID of the user is the key of the data structure.
        /// </summary>
        [DataMember]
        private SortedList<int, double> usersCapacity;
        /// <summary>
        /// Returns the sprint's duration (in days, can be fractions)
        /// </summary>
        public double Duration
        {
            get
            {
                return duration;
            }
        }
        /// <summary>
        /// Returns the sprint's starting date
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return start;
            }
        }
        /// <summary>
        /// Returns the UserStorys of this sprint.
        /// </summary>
        public List<UserStory> UserStorys { get { return uss; } }
        /// <summary>
        /// Returns the ID of the sprint which preceded this sprint
        /// </summary>
        public int PrevSprintID
        {
            get
            {
                return prevSprintID;
            }
        }
        /// <summary>
        /// Returns the ID of the sprint which following this sprint
        /// </summary>
        public int NxtSprintID
        {
            get
            {
                return nxtSprintID;
            }
        }
        /// <summary>
        /// Creates new sprint with given details.
        /// </summary>
        /// <param name="start">When sprint is starting</param>
        /// <param name="duration">The duration (in days, can be fractions) of the sprint</param>
        public Sprint(DateTime start, double duration)
        {
            this.id = ++lastId;
            this.start = start;
            this.duration = duration;
            uss = new List<UserStory>();
            usersCapacity = new SortedList<int, double>();
        }
        /// <summary>
        /// Adding a new UserStory to this sprint
        /// </summary>
        /// <param name="newUS">The new UserStory object</param>
        public void AddUS(UserStory newUS)
        {
            this.uss.Add(newUS);
        }
        /// <summary>
        /// Creates new sprint which following this sprint
        /// </summary>
        /// <param name="newSprintStart">When the new sprint is starting </param>
        /// <param name="newSprintDuration">The duration (in days, can be fractions) of the new sprint</param>
        /// <returns>The new sprint</returns>
        public Sprint CreateNextSprint(DateTime newSprintStart, double newSprintDuration)
        {
            if (this.nxtSprintID == -1)
            {
                Sprint newSpr = new Sprint(newSprintStart, newSprintDuration);
                newSpr.prevSprintID = this.ID;
                this.nxtSprintID = newSpr.ID;
                return newSpr;
            }
            else
                return this;
        }
        /// <summary>
        /// Returns the capacity (in days, can be fractions) of user in this sprint
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns></returns>
        public double GetUserCapacity(int userId)
        {
            try
            {
                return usersCapacity[userId];
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// Set the capacity (in days, can be fractions) of the given user in this sprint.
        /// If the capacity is more than sprint duration, the user capacity will be defined as the sprint duration.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="cap">The new capacity</param>
        public void SetUserCapacity(int userId, double cap)
        {
            if (cap > this.Duration) cap = this.Duration;
            usersCapacity[userId] = cap;
        }
    }
}