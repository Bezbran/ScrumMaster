using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace ScrumMasterWcf
{
    /// <summary>
    /// Representing a task (tasks are the elements of UserStory) in scrum process
    /// </summary>
    [DataContract]
    public class ScrumTask : Job
    {
        /// <summary>
        /// Stores the all scrumtasks which created.Is needed for some methods.
        /// </summary>
        static List<ScrumTask> allScrumTasks = new List<ScrumTask>();
        /// <summary>
        /// Stores the last ID number that was given to task.
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
        /// The ID of the user that doing this task.
        /// </summary>
        [DataMember]
        private int handlerUserID = -1;
        /// <summary>
        /// Stores the ID of the user that doing this task.
        /// </summary>
        public int HandleUserID
        {
            get { return handlerUserID; }
            set { handlerUserID = value; }
        }
        /// <summary>
        /// The amount of days (can be fractions) that this task supposed to take
        /// </summary>
        [DataMember]
        private double plannedEffort;
        /// <summary>
        /// Stores the amount of days (can be fractions) that this task supposed to take
        /// </summary>
        public double PlannedEffort
        {
            get
            {
                return plannedEffort;
            }
        }
        /// <summary>
        /// Stores the amount of days that this task actually taked. (NOT in use right now)
        /// </summary>
        [DataMember]
        private double actualEffort = -1;
        /// <summary>
        /// Stores the amount of days that this task actually taked. (NOT in use right now)
        /// </summary>
        public double ActualEffort { get { return actualEffort; } }
        /// <summary>
        /// Creating new task with the given details
        /// </summary>
        /// <param name="name">The name/header of task</param>
        /// <param name="description">The description of task</param>
        /// <param name="plannedEffort">The amount of days (can be fractions) that this task supposed to take</param>
        /// <param name="handlerUserID">The user that will do this task (Can be re-setted later by BindTaskToUser method)</param>
        public ScrumTask(string name, string description, double plannedEffort, int handlerUserID)
            : base(name, description)
        {
            this.handlerUserID = handlerUserID;
            this.id = ++lastID;
            this.plannedEffort = plannedEffort;
            allScrumTasks.Add(this);
        }
        /// <summary>
        /// Fill this task actualEffort value. (NOT in use right now)
        /// </summary>
        /// <param name="actualEffort"></param>
        public void MarkDone(double actualEffort)
        {
            this.actualEffort = actualEffort;
        }
        /// <summary>
        /// Binding this task to the given user
        /// </summary>
        /// <param name="user">The User who doing this task</param>
        public void BindTaskToUser(User user)
        {
            try
            {
                allScrumTasks.Find((x) => x.ID == this.ID).HandleUserID = user.ID;
            }
            catch
            {

            }
        }
        /// <summary>
        /// Cahnges the status of this task
        /// </summary>
        /// <param name="jSta">The new status. "Accepted" is illegal for tasks.</param>
        public override void ChangeStatus(JobStatuses jSta)
        {
            try
            {
                if (jSta != JobStatuses.Accepted)
                {
                    ScrumTask rsltTask = allScrumTasks.Find((x) => x.ID == this.ID);
                    rsltTask.jobStatus = jSta;
                    //if (jSta == JobStatuses.Done) rsltTask.MarkDone(); //(NOT in use right now)
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// Updates this ScrumTask details.
        /// </summary>
        /// <param name="newSTDetails">Contains the new details. Every field in the US will be replaced with the corresponding field in newUSDetails object</param>
        public void UpdateDetails(ScrumTask newSTDetails)
        {
            if (newSTDetails == null) return;
            this.name = newSTDetails.name;
            this.plannedEffort = newSTDetails.plannedEffort;
            this.jobStatus = newSTDetails.jobStatus;
            this.handlerUserID = newSTDetails.handlerUserID;
            this.description = newSTDetails.description;
            this.actualEffort = newSTDetails.actualEffort;            
        }
    }
}