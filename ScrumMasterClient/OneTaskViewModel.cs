using ScrumMasterWcf;
using System;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ScrumMasterClient
{

    /// <summary>
    /// The ViewModel class.
    /// Contains some fields and properties which necessary for OneTaskView UI
    /// </summary>
    public class OneTaskViewModel : BaseViewModel
    {
        ScrumTask st;
        /// <summary>
        /// Stores the ScrumTask object which recived from server
        /// </summary>
        public ScrumTask St
        {
            get
            {
                return st;
            }

            set
            {
                st = value;

                //Update "boolean" visual
                ShowDetails = Visibility.Visible;
                IsBindToUser = IsBindToUser;
                IsCurUserTask = IsCurUserTask;
                JobStatus = JobStatus;
                Name = st.Header;
                Description = st.Description;
                PlannedEffort = st.PlannedEffort;
                JobStatus = st.JobStatus;
                RaisePropertyChanged("St");
            }
        }
        private bool isNewUS = false;
        string name = "";
        string description = "";
        double plannedEffort = 0;
        /// <summary>
        /// Holds the all possible states
        /// </summary>
        private Job.JobStatuses[] jobStatuses;
        /// <summary>
        /// Stores the new task name
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        /// <summary>
        /// Stores the new task Description
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }
        /// <summary>
        /// Stores the new task name
        /// </summary>
        public double PlannedEffort
        {
            get
            {
                return plannedEffort;
            }

            set
            {
                plannedEffort = value;
                RaisePropertyChanged("PlannedEffort");
            }
        }
        /// <summary>
        /// Stores the new task Planned Effort
        /// </summary>
        public bool IsNewUS
        {
            get
            {
                return isNewUS;
            }

            set
            {
                isNewUS = value;
                RaisePropertyChanged("IsNewUS");
            }
        }
        /// <summary>
        /// The Visibility property below actualy converting bool values to appropriate Visibility values.
        /// </summary>
        public Visibility ShowDetails
        {
            get
            {
                if (st != null) return Visibility.Visible;
                return Visibility.Collapsed;
            }
            set
            {
                RaisePropertyChanged("ShowDetails");
            }
        }
        /// <summary>
        /// The Visibility property below actualy converting bool values to appropriate Visibility values.
        /// </summary>
        public Visibility IsBindToUser
        {
            get
            {
                if (St != null)
                    return St.HandleUserID > -1 ? Visibility.Collapsed : Visibility.Visible;
                else
                    return Visibility.Visible;
            }
            set
            {
                RaisePropertyChanged("IsBindToUser");
            }
        }
        /// <summary>
        /// The Visibility property below actualy converting bool values to appropriate Visibility values.
        /// </summary>
        public Visibility IsCurUserTask
        {
            get
            {
                if (St != null)
                    if (St.HandleUserID == StaticsElements.CurStatElem.CurrentUser.ID)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            set
            {
                RaisePropertyChanged("IsCurUserTask");
            }
        }
        /// <summary>
        /// Holds the all possible states
        /// </summary>
        public Job.JobStatuses[] JobStatuses
        {
            get
            {
                return jobStatuses;
            }

            set
            {
                jobStatuses = value;
                RaisePropertyChanged("JobStatuses");
            }
        }
        /// <summary>
        /// Holds the state of this task
        /// </summary>
        public Job.JobStatuses JobStatus
        {
            get
            {
                if (St != null)
                    return St.JobStatus;
                else
                    return Job.JobStatuses.ToDo;
            }

            set
            {
                if (St != null && value != St.JobStatus)
                    if (StaticsElements.CurStatElem != null)
                    {
                        try
                        {
                            Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitJobStatusChanged(St, value));
                            connThread.Start();
                        }
                        catch (Exception ex)
                        {
                            StaticsElements.CurStatElem.MainWindow.UpdateStatus("In TaskStatusChanged:" + ex.Message);
                        }
                    }
                RaisePropertyChanged("JobStatus");
            }
        }
        /// <summary>
        /// Creates new object and initialize list of possible states for tasks
        /// </summary>
        public OneTaskViewModel()
        {
            // Showing the all possible states for tasks.
            var ps = Enum.GetValues(typeof(Job.JobStatuses));
            JobStatuses = ps.Cast<Job.JobStatuses>().ToArray();
        }
    }
}
