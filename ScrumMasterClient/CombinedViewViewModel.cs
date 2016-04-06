using ScrumMasterWcf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumMasterClient
{
    class CombinedViewViewModel : BaseViewModel
    {
        private ObservableCollection<ScrumTask> todoTasks = new ObservableCollection<ScrumTask>();
        private ObservableCollection<ScrumTask> inprogressTasks = new ObservableCollection<ScrumTask>();
        private ObservableCollection<ScrumTask> impedimentTasks = new ObservableCollection<ScrumTask>();
        private ObservableCollection<ScrumTask> doneTasks = new ObservableCollection<ScrumTask>();
        private UserStory originalUserStory;
        public ObservableCollection<ScrumTask> ToDoTasks
        {
            get
            {
                return todoTasks;
            }

            set
            {
                todoTasks = value;
                RaisePropertyChanged("ToDoTasks");
            }
        }
        public ObservableCollection<ScrumTask> InProgressTasks
        {
            get
            {
                return inprogressTasks;
            }

            set
            {
                inprogressTasks = value;
                RaisePropertyChanged("InProgressTasks");
            }
        }
        public ObservableCollection<ScrumTask> ImpedimentTasks
        {
            get
            {
                return impedimentTasks;
            }

            set
            {
                impedimentTasks = value;
                RaisePropertyChanged("ImpedimentTasks");
            }
        }
        public ObservableCollection<ScrumTask> DoneTasks
        {
            get
            {
                return doneTasks;
            }

            set
            {
                doneTasks = value;
                RaisePropertyChanged("DoneTasks");
            }
        }
        public UserStory OriginalUserStory
        {
            get
            {
                return originalUserStory;
            }

            set
            {
                originalUserStory = value;
                RaisePropertyChanged("OriginalUserStory");
            }
        }

        public CombinedViewViewModel()
        {

        }
        public CombinedViewViewModel(UserStory orgUS)
        {
            LoadTasks(orgUS);
        }
        void LoadTasks(UserStory orgUS)
        {
            if (orgUS == null) return;
            this.originalUserStory = orgUS;
            foreach (var st in orgUS.ScrumTasks)
            {
                switch (st.JobStatus)
                {
                    case Job.JobStatuses.Done:
                        this.DoneTasks.Add(st);
                        break;
                    case Job.JobStatuses.Impediment:
                        this.ImpedimentTasks.Add(st);
                        break;
                    case Job.JobStatuses.InProgress:
                        this.InProgressTasks.Add(st);
                        break;
                    case Job.JobStatuses.ToDo:
                        this.ToDoTasks.Add(st);
                        break;
                }
            }
        }
    }
}
