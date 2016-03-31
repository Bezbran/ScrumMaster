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
        /// Stores the parent UserStory of these tasks
        /// </summary>
        public UserStory us;
        private IEnumerable<ScrumMasterWcf.ScrumTask> stl;
        /// <summary>
        /// Holds the tasks to show
        /// </summary>
        public IEnumerable<ScrumMasterWcf.ScrumTask> ScrumTasksList
        {
            get
            {
                return stl;
            }
            set
            {
                stl = value;
                RaisePropertyChanged("ScrumTasksList");
            }
        }

        #region newUSVariabels
        private bool isNewUS = false;
        private string newUSName;
        private string newUSDescription;
        private int priority;
        /// <summary>
        /// Determine if to show the new US form
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
        /// Holds the new US name/header
        /// </summary>
        public string NewUSName
        {
            get
            {
                return newUSName;
            }

            set
            {
                newUSName = value;
                RaisePropertyChanged("NewUSName");
            }
        }
        /// <summary>
        /// Holds the new US description
        /// </summary>
        public string NewUSDescription
        {
            get
            {
                return newUSDescription;
            }

            set
            {
                newUSDescription = value;

                RaisePropertyChanged("NewUSDescription");
            }
        }
        /// <summary>
        /// Holds the priority of new US
        /// </summary>
        public int Priority
        {
            get
            {
                return priority;
            }

            set
            {
                try
                {
                    priority = Convert.ToInt32(value);
                }
                catch
                {
                    throw new ApplicationException("Priority have to be inteager value.");
                }
                RaisePropertyChanged("Priority");
            }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        public TasksViewViewModel()
        {
        }
    }
}
