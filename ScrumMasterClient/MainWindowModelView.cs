using ScrumMasterWcf;
using System.Collections.Generic;

namespace ScrumMasterClient
{
    /// <summary>
    /// The ViewModel class.
    /// Contains some fields and properties which necessary for MainWindow UI
    /// </summary>
    public class MainWindowModelView : BaseViewModel
    {
        private UserStoryTabs usstr;
        private UsersListView usv;
        private CurrentUserTasksView cutv;
        private DetailsBar detailsBarView;
        private string teamName;
        /// <summary>
        /// Stores the UserStoryTabs control of the view
        /// </summary>
        public UserStoryTabs UserStorysView
        {
            get
            {
                return usstr;
            }

            set
            {
                usstr = value;
                RaisePropertyChanged("UserStorysView");
            }
        }
        /// <summary>
        /// Stores the UsersListView control of the  view
        /// </summary>
        public UsersListView UsersListView
        {
            get
            {
                return usv;
            }

            set
            {
                usv = value;
                RaisePropertyChanged("UsersListView");
            }
        }
        /// <summary>
        /// Stores the CurrentUserTasksView control of the  view
        /// </summary>
        public CurrentUserTasksView UserTasksView
        {
            get
            {
                return cutv;
            }

            set
            {
                cutv = value;
                RaisePropertyChanged("UserTasksView");
            }
        }
        /// <summary>
        /// Stores the name of the scruum - team
        /// </summary>
        public string TeamName
        {
            get
            {
                return teamName;
            }

            set
            {
                teamName = value;
                RaisePropertyChanged("TeamName");
            }
        }
        /// <summary>
        /// Stores the DetailsBar control of the  view
        /// </summary>
        public DetailsBar DetailsBarView
        {
            get
            {
                return detailsBarView;
            }

            set
            {
                detailsBarView = value;
                RaisePropertyChanged("DetailsBarView");
            }
        }
        /// <summary>
        /// Creates new empty object
        /// </summary>
        public MainWindowModelView()
        {

        }
    }
}
