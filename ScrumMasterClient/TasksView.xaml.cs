using System;
using System.Windows;
using System.Windows.Controls;
using ScrumMasterWcf;
using System.Threading;

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
        /// <summary>
        /// Holds the US that all the tasks belong to
        /// </summary>
        private UserStory originalUS;
        /// <summary>
        /// Holds the US that all the tasks belong to
        /// </summary>
        public UserStory OriginalUS
        {
            get
            {
                return originalUS;
            }

            set
            {
                originalUS = value;
                AddNewTaskForm();
            }
        }
        /// <summary>
        /// Holds the form of the new task (used in refresh methods)
        /// </summary>
        private OneTaskView newTaskOTV = null;
        /// <summary>
        /// Holds the form of the new task (used in refresh methods)
        /// </summary>
        public OneTaskView NewTaskOTV
        {
            get { return newTaskOTV; }
            set
            {
                newTaskOTV = value;

            }
        }
        /// <summary>
        /// Determine if the control needs to show the new US form or not
        /// </summary>
        public static readonly DependencyProperty isNewUsProperty = DependencyProperty.Register("IsNewUs", typeof(bool), typeof(TasksView),
            new PropertyMetadata(new PropertyChangedCallback(OnIsNewUs)));
        private static void OnIsNewUs(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                var depend = (TasksView)d;
                var st = (bool)e.NewValue;
                if (depend != null)
                    depend.tvm.IsNewUS = st;
            }
            catch
            {

            }
        }
        /// <summary>
        /// Determine if it's new UserStory
        /// </summary>
        public bool IsNewUs
        {
            get
            {
                var propertyVal = (bool)GetValue(isNewUsProperty);
                return tvm.IsNewUS && propertyVal;
            }
            set
            {
                SetValue(isNewUsProperty, value);
                tvm.IsNewUS = value;
            }
        }
        /// <summary>
        /// Init new view of tasks, the father control that create the TasksView control
        /// needs to have the necessary fields so the XAML can handle the view
        /// </summary>
        public TasksView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Adds the form that allow the user to add task to this US
        /// </summary>
        private void AddNewTaskForm()
        {
            if (originalUS == null || originalUS.Priority < 0) return;
            // if the NewTaskOTV!= null, we need to recover it
            var newOnv = NewTaskOTV == null ? new OneTaskView(originalUS) : NewTaskOTV;
            baseGrid.Children.Add(newOnv);
            var rd = new RowDefinition();
            rd.Height = GridLength.Auto;
            baseGrid.RowDefinitions.Add(rd);
            Grid.SetColumn(newOnv, 0);
            Grid.SetRow(newOnv, 2);
            NewTaskOTV = newOnv;
            this.Resources.Add("OriginalUS", this.originalUS);
        }
        /// <summary>
        /// Handle the create US form action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitNewUS(tvm));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In NewUS:" + ex.Message);
                }
            }
        }
    }
}