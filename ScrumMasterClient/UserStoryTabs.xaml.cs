using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ScrumMasterWcf;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for UserStoryTable.xaml,
    /// the main part of the scrum work
    /// </summary>
    public partial class UserStoryTabs : UserControl
    {
        //TODO: Add view as table by tasks state
        /// <summary>
        /// Creates new and empty UseStoryTable
        /// </summary>
        public UserStoryTabs()
        {
            InitializeComponent();
            
        }
        /// <summary>
        /// Load the view of the UserStories and their tasks in the strating,
        /// and refresh the view when it necessary, with effort to save the UI stable
        /// to allow continues working
        /// </summary>
        public void Refresh()
        {
            // We need to save the active tab of the user, if it's not "create new US" tab, to prevent jumps in the GUI.
            int activeUSId = -1;
            ListBox activeUSTasksLV = null;
            int[] expandedTasksIds = null;
            TabItemViewModel tivm = null;
            // If the user creating task, we need to save it's state
            OneTaskView activeNewTaskOTV = null;

            if (userStorysTC.SelectedIndex > -1 && userStorysTC.SelectedIndex != (userStorysTC.Items.Count - 1))
            // The user expand some tasks and maybe started to create new task
            {
                tivm = ((TabItemViewModel)userStorysTC.SelectedItem);
                activeUSId = tivm.OriginalUserStory.ID;
                // In addition, we need to save the expanded tasks of this US
                activeUSTasksLV = FindControl<ListBox>.FindControlInViewTree(tivm.TasksListView);
                expandedTasksIds = ExpendedItems(activeUSTasksLV);
                if (tivm.TasksListView.NewTaskOTV != null && tivm.TasksListView.NewTaskOTV.newTaskExpander.IsExpanded)
                    // The user started to create new task
                    activeNewTaskOTV = tivm.TasksListView.NewTaskOTV;
            }
            // If the user wrote somthing in newUserStory tab Textboxs, we want it to stay (just product-owner can create new USs)
            if (userStorysTC.SelectedIndex == (userStorysTC.Items.Count - 1) && !StaticsElements.CurStatElem.CurrentUser.Positions.Contains(User.Position.ProductOwner))
            {
                tivm = ((TabItemViewModel)userStorysTC.SelectedItem);
            }
            // The actual refreshing
            ustvm.Refresh();
            // Recover if the user wrote somthing in newUserStory tab Textboxs
            if (activeUSId == -1)
            {
                if (tivm == null) return;
                else
                {
                    var tivmNewUS = ((TabItemViewModel)userStorysTC.Items[(userStorysTC.Items.Count - 1)]);
                    userStorysTC.SelectedIndex = (userStorysTC.Items.Count - 1);
                    tivmNewUS.TasksListView = tivm.TasksListView;
                }
            }
            // Recover the active tab of the user, if it's not "create new US" tab
            var activeUS = ustvm.Tabs.FirstOrDefault((x) => x.OriginalUserStory.ID == activeUSId);
            if (activeUS != null)
                userStorysTC.SelectedIndex = ustvm.Tabs.IndexOf(activeUS);
            // Recover the expanded tasks                
            if (expandedTasksIds != null)
            {
                tivm = ((TabItemViewModel)userStorysTC.SelectedItem);
                tivm.TasksListView.UpdateLayout();
                activeUSTasksLV = FindControl<ListBox>.FindControlInViewTree(tivm.TasksListView);
                ExpandItems(activeUSTasksLV, expandedTasksIds);
            }
            // Recover the NewTaskOTV details if the user wrote something there
            if (activeNewTaskOTV != null)
            {
                tivm.TasksListView.NewTaskOTV.UpdateLayout();
                tivm.TasksListView.NewTaskOTV.TaskHeaderTB.Text = activeNewTaskOTV.TaskHeaderTB.Text;
                tivm.TasksListView.NewTaskOTV.TaskPlannedEffortTB.Text = activeNewTaskOTV.TaskPlannedEffortTB.Text;
                tivm.TasksListView.NewTaskOTV.TaskDescTB.Text = activeNewTaskOTV.TaskDescTB.Text;
                tivm.TasksListView.NewTaskOTV.newTaskExpander.IsExpanded = true;
                tivm.TasksListView.NewTaskOTV.Focus();
            }
        }
        /// <summary>
        /// Checks which tasks was expanded by the user
        /// to use in the ExpandItems method
        /// </summary>
        /// <param name="tasksLV">The ListBox of the tasks view</param>
        /// <returns>The Id numbers of the tasks which expanded</returns>
        private int[] ExpendedItems(ListBox tasksLV)
        {
            if (tasksLV == null) return null;
            List<int> items = new List<int>();
            foreach (var item in tasksLV.Items)
            {
                var itemContainer = tasksLV.ItemContainerGenerator.ContainerFromItem(item);
                var otv = FindControl<OneTaskView>.FindControlInViewTree(itemContainer);
                if (otv != null && otv.detailsExpander.IsExpanded)
                    items.Add(((ScrumMasterWcf.ScrumTask)item).ID);
            }

            return items.ToArray();
        }
        /// <summary>
        /// Expand the tasks in tasksLV with the given IDs
        /// </summary>
        /// <param name="tasksLV">The ListBox of the tasks view</param>
        /// <param name="tasksIds">The Id numbers of the tasks which expanded and returned by ExpendedItems method</param>
        private void ExpandItems(ListBox tasksLV, int[] tasksIds)
        {
            if (tasksLV == null) return;
            tasksLV.UpdateLayout();
            foreach (var item in tasksLV.Items)
            {
                if (tasksIds.ToList().Contains(((ScrumMasterWcf.ScrumTask)item).ID))
                {
                    var itemContainer = tasksLV.ItemContainerGenerator.ContainerFromItem(item);
                    var otv = FindControl<OneTaskView>.FindControlInViewTree(itemContainer);
                    if (otv != null)
                        otv.detailsExpander.IsExpanded = true;
                }
            }
        }
        
    }    
}
