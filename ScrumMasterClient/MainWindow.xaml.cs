using System;
using System.Windows;
using System.Windows.Threading;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// The window that showing all of the scrum elements.
    /// Most of the server updates in this project are done in seperate
    ///  threads to allow continues work
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Saves the status until the DetailsBarView will be initialized.
        /// </summary>
        string status = "OK";
        /// <summary>
        /// Timer that responsible to update the UI according to the changes in the server.
        /// </summary>
        DispatcherTimer checkUpdateTimer;
        /// <summary>
        /// Create new object and starting the connecting to the server.
        /// (If the connection can't established the program will exit)
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // The order of the initialization it critical
            mwmv.UserStorysView = new UserStoryTabs();
            // ConnectWindow can turn off the program
            // If connection was established, the needed information is
            // in StaticsElements.CurStatElem object, which is responsible
            // to communicate the server to update to/from it.
            new ConnectWindow(this).ShowDialog();
            mwmv.UsersListView = new UsersListView();
            mwmv.UserTasksView = new CurrentUserTasksView();
            mwmv.DetailsBarView = new DetailsBar();
            mwmv.DetailsBarView.dbvm.Status = this.status;
            this.usvp.ustblv.ustvm.UserStorys = StaticsElements.CurStatElem.CurrentSprint.UserStorys;
            this.usvp.ustblv.LoadUS();
            StartCheckUpdateTimer();
        }
        /// <summary>
        /// Initializing the timer that keep the UI synchronizing with the
        /// StaticsElements.CurStatElem data every 400 miliseconds.
        /// </summary>
        private void StartCheckUpdateTimer()
        {
            checkUpdateTimer = new DispatcherTimer();
            checkUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            checkUpdateTimer.Tick += StaticsElements.CurStatElem.CheckUpdate;
            checkUpdateTimer.Start();
        }
        /// <summary>
        /// Adding the given message to  DetailsBarView status line.       
        /// </summary>
        /// <param name="message">The message to add to status line.</param>
        internal void UpdateStatus(string message)
        {
            if (mwmv.DetailsBarView != null)
                mwmv.DetailsBarView.dbvm.Status += "\n" + message;
            else
                // The DetailsBarView is not ready, so we saves the
                // status in temporal buffer until the DetailsBarView will be initialized.
                this.status += "\n" + message;
        }
    }
}
