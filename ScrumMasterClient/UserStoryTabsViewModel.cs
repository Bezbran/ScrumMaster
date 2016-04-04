using ScrumMasterWcf;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScrumMasterClient
{
    /// <summary>
    /// The model of the main part of the scrum work
    /// </summary>
    public class UserStoryTabsViewModel : BaseViewModel, IHaveRefresh
    {
        private ObservableCollection<TabItemViewModel> tabs = null;
        /// <summary>
        /// Holds the tabs collection, each tab represent one UserStory
        /// </summary>
        public ObservableCollection<TabItemViewModel> Tabs
        {
            get
            {
                return tabs;
            }
            set
            {
                tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }
        /// <summary>
        /// Loads the USs from the server into the model so the view can proceed
        /// </summary>
        /// <returns>The UserStorys as tab collection, ordered by priority</returns>
        private ObservableCollection<TabItemViewModel> InitUss()
        {
            IEnumerable uss;
            var tabs = new ObservableCollection<TabItemViewModel>();
            if (StaticsElements.CurStatElem != null && StaticsElements.CurStatElem.CurrentSprint != null && StaticsElements.CurStatElem.CurrentSprint.UserStorys != null)
                uss = StaticsElements.CurStatElem.CurrentSprint.UserStorys.OrderBy((x) => x.Priority);
            else
                return null;
            if (uss != null)
                foreach (UserStory us in uss)
                {
                    var ti = new TabItemViewModel(us);
                    //ti.TasksListView = new TasksView();
                    ti.OriginalUserStory = us;
                    tabs.Add(ti);
                }
            // Adding the form of new US if needed
            //var tbivm = new TabItemViewModel { TasksListView = new TasksView() };
            //tbivm.OriginalUserStory = new UserStory("Create new UserStory", "Here you can create new UserStory", -1);
            //tbivm.TasksListView.IsNewUs = true;
            //tabs.Add(tbivm);
            return tabs;
        }
        /// <summary>
        /// Refresh the model and cause the view be refreshing
        /// </summary>
        public void Refresh()
        {
            Tabs = InitUss();
        }
        /// <summary>
        /// Creates new empty object
        /// </summary>
        public UserStoryTabsViewModel()
        {

        }
    }
}
