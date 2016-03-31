using ScrumMasterWcf;
using System;
using System.Linq;
using System.Windows;

namespace ScrumMasterClient
{
    /// <summary>
    /// The model of the UserStory tab in the UserStoryTabs
    /// </summary>
    public class TabItemViewModel : BaseViewModel, IHaveRefresh
    {
        private TasksView tv;
        private Job.JobStatuses[] jobStatuses;
        private String description = " ";
        private String header = "";
        private int priority = -1;
        public String Description {
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
        public String Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                RaisePropertyChanged("Header");
            }
        }
        /// <summary>
        /// Holds the view of the tasks of this UserStory
        /// </summary>
        public TasksView TasksListView
        {
            get
            {
                return tv;
            }

            set
            {
                tv = value;
                RaisePropertyChanged("TasksListView");
            }
        }
        /// <summary>
        /// Holds the UserStory of this tab
        /// </summary>
        public UserStory OriginalUserStory
        {
            get
            {
                return TasksListView.OriginalUS;
            }

            set
            {
                TasksListView.OriginalUS = value;
                this.Description = value.Description;
                this.Header = value.Header;
                this.Priority = value.Priority;
                RaisePropertyChanged("OriginalUserStory");
            }
        }
        /// <summary>
        /// Determine if show the details of the UserStory - when it existing
        /// OR don't show them when it is new US (priority less than 0)
        /// It convert explicity boolean condition to Visibility value without using
        /// VisibilityConverter class.
        /// </summary>
        public Visibility ShowDetails
        {
            get
            {
                if (OriginalUserStory.Priority >= 0) return Visibility.Visible;
                return Visibility.Collapsed;
            }
            set
            {
                RaisePropertyChanged("ShowDetails");
            }
        }
        /// <summary>
        /// Holds the possible statuses for task/user-story
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
        /// Holds the ScrumTasks of this user-story
        /// </summary>
        public ScrumTask[] ScrumTasks
        {
            get
            {
                return OriginalUserStory.ScrumTasks.ToArray();
            }
            set
            {
                RaisePropertyChanged("ScrumTasks");
            }
        }
        /// <summary>
        /// Holds the status of this user-story
        /// </summary>
        public Job.JobStatuses JobStatus
        {
            get
            {
                if (OriginalUserStory != null)
                    return OriginalUserStory.JobStatus;
                return Job.JobStatuses.ToDo;
            }

            set
            {
                if (OriginalUserStory != null && value != OriginalUserStory.JobStatus)
                    StaticsElements.CurStatElem.SubmitJobStatusChanged(OriginalUserStory, value);
                RaisePropertyChanged("JobStatus");
            }
        }

        public int Priority
        {
            get
            {
                return priority;
            }

            set
            {
                priority = value;
                RaisePropertyChanged("Priority");
            }
        }

        /// <summary>
        /// Creates new object and loads the possible statuses for task/user-story
        /// </summary>
        public TabItemViewModel()
        {
            var ps = Enum.GetValues(typeof(Job.JobStatuses));
            JobStatuses = ps.Cast<Job.JobStatuses>().ToArray();
        }
        public TabItemViewModel(UserStory orgUS)
        {
            var ps = Enum.GetValues(typeof(Job.JobStatuses));
            JobStatuses = ps.Cast<Job.JobStatuses>().ToArray();
            //originalUserStory = orgUS;
        }
        /// <summary>
        /// Not in use, uses for future needs
        /// </summary>
        public void Refresh()
        {
            TasksListView = TasksListView;
            ShowDetails = ShowDetails;
            JobStatus = JobStatus;
        }
    }
}
