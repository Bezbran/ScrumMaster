using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for CurrentUserTasksView.xaml,
    /// showing the tasks which belongs to the user of the current client
    /// </summary>
    public partial class CurrentUserTasksView : UserControl, IHaveRefresh
    {
        /// <summary>
        /// Create new object and load the tasks into it.
        /// </summary>
        public CurrentUserTasksView()
        {
            InitializeComponent();
            Refresh();
        }
        /// <summary>
        /// Refreshing the tasks view
        /// </summary>
        public void Refresh()
        {
            // We need to save the active task and the expanded tasks
            // in order to prevent jumps in the GUI.
            List<ScrumMasterWcf.ScrumTask> curUserTasksList = new List<ScrumMasterWcf.ScrumTask>();
            var expandedTasksIds = ExpendedItems();
            int activeItemID = curTasksListViewLV.SelectedIndex;
            if (activeItemID > -1)
            {
                activeItemID = ((ScrumMasterWcf.ScrumTask)curTasksListViewLV.SelectedItem).ID;
            }
            // Refreshing the list
            if (StaticsElements.CurStatElem != null && StaticsElements.CurStatElem.CurrentUser != null && StaticsElements.CurStatElem.CurrentSprint != null)
                curUserTasksList = StaticsElements.CurStatElem.CurrentUser.GetTasks(StaticsElements.CurStatElem.CurrentSprint);
            curTasksListViewLV.ItemsSource = curUserTasksList;
            curTasksListViewLV.Items.Refresh();
            // Refreshing the view (otherwise we can't recover the expanded items)
            curTasksListViewLV.UpdateLayout();
            // Recoverthe active task and the expanded tasks
            ExpandItems(expandedTasksIds);
            if (activeItemID == -1) return;
            var recoverItem = curUserTasksList.FirstOrDefault((x) => x.ID == activeItemID);
            curTasksListViewLV.SelectedIndex = curTasksListViewLV.Items.IndexOf(recoverItem);

        }
        /// <summary>
        /// Search in the curTasksListViewLV.Items collection to find all
        /// the tasks which are expanded now
        /// </summary>
        /// <returns>Return the Id's of the tasks which are expanded now</returns>
        private int[] ExpendedItems()
        {
            List<int> items = new List<int>();
            foreach(var item in curTasksListViewLV.Items)
            {
                // Extracting the DependencyObject to search the OneTaskView object in it
                var itemContainer = curTasksListViewLV.ItemContainerGenerator.ContainerFromItem(item);
                var otv = FindControl<OneTaskView>.FindControlInViewTree(itemContainer);
                if (otv != null && otv.detailsExpander.IsExpanded)
                    items.Add(((ScrumMasterWcf.ScrumTask)item).ID);
            }
            return items.ToArray();
        }
        /// <summary>
        /// Expand the tasks with the given ids.
        /// To achive good result, curTasksListViewLV.UpdateLayout() have to call before. 
        /// </summary>
        /// <param name="tasksIds">The tasks IDs</param>
        private void ExpandItems(int[] tasksIds)
        {
            foreach (var item in curTasksListViewLV.Items)
            {
                if(tasksIds.ToList().Contains(((ScrumMasterWcf.ScrumTask)item).ID))
                {
                    // Extracting the DependencyObject to search the OneTaskView object in it
                    var itemContainer = curTasksListViewLV.ItemContainerGenerator.ContainerFromItem(item);
                    //var otv = FindOtv(itemContainer);
                    var otv = FindControl<OneTaskView>.FindControlInViewTree(itemContainer);
                    if (otv != null)
                        otv.detailsExpander.IsExpanded = true;
                }
            }
        }
    }
}
