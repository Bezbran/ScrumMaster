using System;
using System.Windows;
using System.Windows.Controls;
using ScrumMasterWcf;
using System.Threading;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for OneTaskView.xaml,
    /// control that showing details of only one task or
    /// showing form that allowing creating of new task to specific UserStory
    /// Most of the server updates in this project are done in seperate
    /// threads to allow continues work
    /// </summary>
    public partial class OneTaskView : UserControl
    {
        /// <summary>
        /// The UserStory that contains this task
        /// </summary>
        private UserStory orgUS;
        /// <summary>
        /// The 3 class members below storring the ScrumTask object which recived from server
        /// </summary>
        public static readonly DependencyProperty scrumTaskProperty = DependencyProperty.Register("OrginialScrumTask", typeof(ScrumTask), typeof(OneTaskView),
            new PropertyMetadata(new PropertyChangedCallback(OnOrginialScrumTask)));
        private static void OnOrginialScrumTask(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                var depend = (OneTaskView)d;
                var st = (ScrumTask)e.NewValue;
                if (depend != null && st != null)
                    depend.otvm.St = st;
            }
            catch
            {

            }
        }
        /// <summary>
        /// Stores ScrumTask object which recived from server
        /// </summary>
        public ScrumTask OrginialScrumTask
        {
            get
            {
                return (ScrumTask)GetValue(scrumTaskProperty);
            }
            set
            {
                SetValue(scrumTaskProperty, value);

            }
        }
        /// <summary>
        /// Creates new task view and sets the original UserStory of this task.
        /// </summary>
        /// <param name="orgUS">The parent UserStory</param>
        // <param name="isNewUS">Determine the visiability if the new UserStory form</param>
        public OneTaskView(UserStory orgUS)
        {
            InitializeComponent();
            this.orgUS = orgUS;
        }
        /// <summary>
        /// Creates new object
        /// </summary>
        public OneTaskView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Adding this task to current user tasks list, which means he will handle it.
        /// </summary>
        /// <param name="sender">Add task button</param>
        /// <param name="e"></param>
        private void addTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if (otvm.St != null)
                if (StaticsElements.CurStatElem != null)
                {
                    try
                    {
                        Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitTaskToUser(otvm.St));
                        connThread.Start();
                    }
                    catch (Exception ex)
                    {
                        StaticsElements.CurStatElem.MainWindow.UpdateStatus("In TaskToUser:" + ex.Message);
                    }
                }
        }
        /// <summary>
        /// Updating the server about this task state when user changing it
        /// </summary>
        /// <param name="sender">ComboBox of the task states</param>
        /// <param name="e"></param>
        private void taskStateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (otvm.St != null && (Job.JobStatuses)e.AddedItems[0] != otvm.St.JobStatus)
                if (StaticsElements.CurStatElem != null)
                {
                    try
                    {
                        Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitJobStatusChanged(otvm.St, (Job.JobStatuses)e.AddedItems[0]));
                        connThread.Start();
                    }
                    catch (Exception ex)
                    {
                        StaticsElements.CurStatElem.MainWindow.UpdateStatus("In TaskStatusChanged:" + ex.Message);
                    }
                }
        }
        private void detailsExpander_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Tag != null && e.ClickCount > 1)
                if (hearderTBX.Visibility != Visibility.Visible)
                {
                    updateBtn.Visibility = deleteBtn.Visibility = descTBX.Visibility = effortTBX.Visibility = hearderTBX.Visibility = Visibility.Visible;
                    descTBL.Visibility = effortTBL.Visibility = hearderTBL.Visibility = Visibility.Collapsed;
                }
                else
                {
                    updateBtn.Visibility = deleteBtn.Visibility = descTBX.Visibility = effortTBX.Visibility = hearderTBX.Visibility = Visibility.Collapsed;
                    descTBL.Visibility = effortTBL.Visibility = hearderTBL.Visibility = Visibility.Visible;
                }
        }
        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null && Tag != null)
            {
                var nst = new ScrumTask(otvm.Name, otvm.Description, otvm.PlannedEffort, otvm.St.HandleUserID);
                orgUS = (UserStory)Tag;
                int existSTID = OrginialScrumTask.ID;
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitUpdateTask(orgUS, existSTID, nst));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In TaskUpdateBtn_Click: " + ex.Message);
                }
            }
        }
        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null && Tag != null)
            {

                var mbr = MessageBox.Show("Are you sure you want to delete this task?", "ScrumMaster", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbr != MessageBoxResult.Yes) return;
                int existSTID = OrginialScrumTask.ID;
                orgUS = (UserStory)Tag;

                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitRemoveTask(orgUS, existSTID));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In TaskDeleteBtn_Click: " + ex.Message);
                }
            }
        }
        private void detailsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            updateBtn.Visibility = deleteBtn.Visibility = descTBX.Visibility = effortTBX.Visibility = hearderTBX.Visibility = Visibility.Collapsed;
            descTBL.Visibility = effortTBL.Visibility = hearderTBL.Visibility = Visibility.Visible;
        }
    }
}
