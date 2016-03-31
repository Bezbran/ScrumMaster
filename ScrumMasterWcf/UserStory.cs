using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Represent a UserStory - a mission which hane to be done in sprint).
    /// </summary>
    [DataContract]
    public class UserStory : Job
    {
        /// <summary>
        /// Stores the all UserStorys which created. It is needed for some methods.
        /// </summary>
        public static List<UserStory> allUserStorys = new List<UserStory>();
        /// <summary>
        /// Stores the last ID number that was given to UserStory.
        /// For scrum saving and opening purpose.
        /// </summary>
        static int lastID = 0;
        /// <summary>
        /// Exposes the last ID number that was given to task.
        /// For scrum saving and opening purpose.
        /// Therefore one can set LastID only when LastID==0,
        ///  which means that there is new scrum process running.
        /// </summary>
        public static int LastId
        {
            get { return lastID; }
            set { if (lastID == 0) lastID = value; }
        }
        /// <summary>
        /// Stores the priority of this UserStory
        /// </summary>
        [DataMember]
        private int priority;
        /// <summary>
        /// Returns the priority of this UserStory
        /// </summary>
        public int Priority
        {
            get
            {
                return priority;
            }
        }
        /// <summary>
        /// Stores the tasks which comprissing this UserStory
        /// </summary>
        [DataMember]
        private List<ScrumTask> Tasks;
        /// <summary>
        /// Returns the collection of tasks which comprissing this UserStory
        /// </summary>
        public List<ScrumTask> ScrumTasks { get { return Tasks; } }
        /// <summary>
        /// Returns the amount of days (can be fractions) that this task supposed to take
        /// </summary>
        public double PlannedEffort
        {
            get
            {
                double effortSum = 0;
                if (this.Tasks != null)
                    foreach (ScrumTask st in this.Tasks)
                        effortSum += st.PlannedEffort;
                return effortSum;
            }
        }
        /// <summary>
        /// Creates new UserStory with the given details
        /// </summary>
        /// <param name="name">The new UserStory name/header</param>
        /// <param name="description">The new UserStory description</param>
        /// <param name="priority">The new UserStory priority</param>
        public UserStory(string name, string description, int priority)
            : base(name, description)
        {
            this.id = ++lastID;
            this.priority = priority;
            Tasks = new List<ScrumTask>();
            allUserStorys.Add(this);
        }
        /// <summary>
        /// Adding a task to this UserStory
        /// </summary>
        /// <param name="tsk">The ne task</param>
        public void AddTask(ScrumTask tsk)
        {
            if (tsk != null)
                Tasks.Add(tsk);
        }
        /// <summary>
        /// Cahnges the status of this UserStory
        /// </summary>
        /// <param name="jSta">The new status. "Impediment" is illegal for UserStory.</param>
        public override void ChangeStatus(JobStatuses jSta)
        {
            try
            {
                if (jSta != JobStatuses.Impediment)
                    allUserStorys.Find((x) => x.ID == this.ID).jobStatus = jSta;
            }
            catch
            {

            }
        }
        /// <summary>
        /// Updates this US details. Not changing the tasks.
        /// </summary>
        /// <param name="newUSDetails">Contains the new details. Every field in the US will be replaced with the corresponding field in newUSDetails object</param>
        public void UpdateDetails(UserStory newUSDetails)
        {
            if (newUSDetails == null) return;
            this.name = newUSDetails.name;
            this.description = newUSDetails.description;
            this.priority = newUSDetails.priority;
            this.jobStatus = newUSDetails.jobStatus;                     
        }
    }
}