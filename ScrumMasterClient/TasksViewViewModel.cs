using ScrumMasterWcf;
using System;
using System.Collections.Generic;

namespace ScrumMasterClient
{

    /// <summary>
    /// The model of the view
    /// </summary>
    public class TasksViewViewModel : BaseViewModel
    {
        /// <summary>
        /// Holds the tasks to show
        /// </summary>
        private IEnumerable<ScrumMasterWcf.ScrumTask> scrumTasksList;
        public IEnumerable<ScrumMasterWcf.ScrumTask> ScrumTasksList
        {
            get
            {
                if (scrumTasksList == null&& OriginalUserStory!=null)
                    return OriginalUserStory.ScrumTasks;
                return scrumTasksList;
            }
            set
            {
                scrumTasksList = value;
                RaisePropertyChanged("ScrumTasksList");
            }
        }

        private UserStory originalUserStory;

        public UserStory OriginalUserStory
        {
            get
            {
                return originalUserStory;
            }

            set
            {
                //TasksListView.OriginalUS = value;
                originalUserStory = value;
                RaisePropertyChanged("OriginalUserStory");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TasksViewViewModel()
        {
        }
    }
}
