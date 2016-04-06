using System;
using System.Windows;
using System.Windows.Controls;
using ScrumMasterWcf;
using System.Threading;
using System.Collections.ObjectModel;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for TasksView.xaml,
    /// which responsible to show the tasks of the originalUS
    /// beside the option to create new task,
    /// OR in case of new US to show form to create new US
    /// </summary>
    public partial class TasksView : UserControl
    {
        TasksViewViewModel tvvm = new TasksViewViewModel();
        /// <summary>
        /// Init new view of tasks, the father control that create the TasksView control
        /// needs to have the necessary fields so the XAML can handle the view
        /// </summary>
        public TasksView()
        {
            InitializeComponent();
        }

        public TasksView(TasksViewViewModel tvvm) : this()
        {
            this.DataContext = this.tvvm = tvvm;
        }
        private void tvc_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag != null)
            {
                if (this.Tag is UserStory)
                    this.tvvm.OriginalUserStory = (UserStory)this.Tag;
                if (this.Tag is ObservableCollection<ScrumTask>)
                    this.tvvm.ScrumTasksList = this.Tag as ObservableCollection<ScrumTask>;
            }
            this.DataContext = this.tvvm;
        }
    }
}