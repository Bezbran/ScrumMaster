using System;
using System.Runtime.Serialization;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Abstract class which contain methods and members for scrum-jobs (like UserStorys and Tasks) managment
    /// </summary>
    [KnownType(typeof(ScrumTask))]
    [KnownType(typeof(UserStory))]
    [DataContract]
    public abstract class Job : IHaveID
    {
        /// <summary>
        /// The possible states of job. Each kind of job can use another set of states.
        /// </summary>
        public enum JobStatuses : int
        {
            /// <summary>
            /// Marks the job as initialized job. The default status.
            /// </summary>
            ToDo,
            /// <summary>
            /// Marks the job as in progrres
            /// </summary>
            InProgress,
            /// <summary>
            /// Marks the job as impediment
            /// </summary>
            Impediment,
            /// <summary>
            /// Mark the job as done
            /// </summary>
            Done,
            /// <summary>
            /// Marks the job as accepted by the product-owner
            /// </summary>
            Accepted
        };
        /// <summary>
        /// Stores the job ID
        /// </summary>
        [DataMember]
        protected int id;
        /// <summary>
        /// Returns the job's id
        /// </summary>
        public int ID
        {
            get { return this.id; }
        }
        /// <summary>
        /// Stores the job name/header
        /// </summary>
        [DataMember]
        protected string name;
        /// <summary>
        /// Stores the job description
        /// </summary>
        [DataMember]
        protected string description;
        /// <summary>
        /// Stores the job status
        /// </summary>
        [DataMember]
        protected JobStatuses jobStatus;
        /// <summary>
        /// Returns the job's name/header
        /// </summary>
        public string Header
        {
            get
            {
                return this.name;
            }
        }
        /// <summary>
        /// Returns the job's description
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }
        /// <summary>
        /// Returns the job's status
        /// </summary>
        public JobStatuses JobStatus
        {
            get
            {
                return jobStatus;
            }
        }
        /// <summary>
        /// Create empty job
        /// </summary>
        public Job() { }
        /// <summary>
        /// Fills the fields of new job object
        /// </summary>
        /// <param name="name">The job's name/header </param>
        /// <param name="description">The job's description</param>
        public Job(string name, string description)
        {

            this.name = name;
            this.description = description;
            this.jobStatus = JobStatuses.ToDo;
        }
        /// <summary>
        /// Sets the status of the job. Not every job accepts every status.
        /// </summary>
        /// <param name="jSta">The new status to asign to the job</param>
        public abstract void ChangeStatus(JobStatuses jSta);
    }
}