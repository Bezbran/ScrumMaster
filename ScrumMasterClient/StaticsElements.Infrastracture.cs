using ScrumMasterWcf;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ScrumMasterClient
{
    partial class StaticsElements
    {
        /// <summary>
        /// Holds StaticsElements object as static method so we can establish the connection once
        /// and use it when needed
        /// </summary>
        public static StaticsElements CurStatElem = null;
        /// <summary>
        /// Allows all the components access the MainWindow to update it
        /// </summary>
        public MainWindow MainWindow;
        /// <summary>
        /// Data of the scrum
        /// </summary>
        private User currentUser;
        private Sprint currentSprint;
        private List<User> usersList;
        /// <summary>
        /// Data for synchronizing the client and the server
        /// </summary>
        private int serverCurrentSprintId = -1;
        private DateTime updateFlag;
        private DateTime lastRefresh;
        private bool isUsersChanged = false;
        private bool isUSSChanged = false;
        private bool isTasksChanged = false;
        private bool isDetailsChanged = false;
        private Timer checkServerChangedTimer;
        /// <summary>
        /// Connection fields
        /// </summary>
        private BasicHttpBinding myBinding;
        private EndpointAddress myEndpoint;
        private ChannelFactory<IScrumMasterService> myChannelFactory;
        /// <summary>
        /// Public properties
        /// </summary>
        public List<User> UsersList
        {
            get
            {
                return this.usersList;
            }
        }
        public User CurrentUser
        {
            get
            {
                return currentUser;
            }

            set
            {
                currentUser = value;
            }
        }
        public Sprint CurrentSprint
        {
            get
            {
                return currentSprint;
            }

            set
            {
                currentSprint = value;
            }
        }
        public DateTime UpdateFlag
        {
            get
            {
                return updateFlag;
            }
        }
        public DateTime LastRefresh
        {
            get
            {
                return lastRefresh;
            }

            set
            {
                lastRefresh = value;
            }
        }
        /// <summary>
        /// Initialize the connection to the server when needed
        /// </summary>
        /// <returns>Object that contains the methods which had exposed by the server</returns>
        private IScrumMasterService InitConnection()
        {
            IScrumMasterService rtrnServ = null;
            try
            {
                rtrnServ = myChannelFactory.CreateChannel();
                return rtrnServ;
            }
            catch (Exception ex)
            {
                this.MainWindow.UpdateStatus("In InitConnection:\n" + ex.Message);
                if (rtrnServ != null)
                    ((ICommunicationObject)rtrnServ).Abort();
                return null;
            }
        }

        /// <summary>
        /// Create new object of StaticsElements by using the given parameters
        /// </summary>
        /// <param name="srvUri">The URI that contains the server URl</param>
        /// <param name="userName">The client user name</param>
        /// <param name="password">The client password</param>
        /// <param name="mw">MainWindow object to update when needed</param>
        public StaticsElements(Uri srvUri, string userName, string password, MainWindow mw)
        {
            myBinding = new BasicHttpBinding();
            myEndpoint = new EndpointAddress(srvUri.OriginalString);
            myChannelFactory = new ChannelFactory<IScrumMasterService>(myBinding, myEndpoint);
            MainWindow = mw;
            IScrumMasterService client = InitConnection();
            try
            {
                if (client != null)
                {
                    CurStatElem = this;
                    // Getting the information from the server
                    Login(client, userName, password);
                    this.CurrentSprint = client.GetCurrentSprint(CurrentUser);
                    MainWindow.mwmv.TeamName = client.GetTeamName(CurrentUser);
                    usersList = client.GetUsersList(CurrentUser);
                    ((ICommunicationObject)client).Close();
                    // We need to send the password to the server to authentication in some methods
                    CurrentUser.Password = password;
                    // Setting some data to the synchronization 
                    LastRefresh = DateTime.Now;
                    if (CurrentSprint != null)
                        serverCurrentSprintId = CurrentSprint.ID;
                    Thread serverCheckThread = new Thread(new ThreadStart(InitRefresh));
                    serverCheckThread.Start();
                    updateFlag = DateTime.Now;
                    isUSSChanged = true;
                    InitRefresh();
                }
            }
            catch (Exception ex)
            {
                this.MainWindow.UpdateStatus("In constructor:\n" + ex.Message);
            }
        }
        /// <summary>
        /// Initializing the synchronization thread
        /// </summary>
        private void InitRefresh()
        {
            checkServerChangedTimer = new Timer(CheckServerChanged, null, 1000, 500);

        }
        /// <summary>
        /// The main routine of the synchronization thread, 
        /// checks the server status
        /// </summary>
        /// <param name="state"></param>
        private void CheckServerChanged(object state)
        {
            if (CurrentSprint != null && serverCurrentSprintId != CurrentSprint.ID) return;
            try
            {
                checkServerChangedTimer.Dispose();
                IScrumMasterService client = InitConnection();

                if (client != null)
                {
                    if (client.IsSprintChangedSince(UpdateFlag) || client.IsUssChangedSince(UpdateFlag))
                    {
                        CurrentSprint = client.GetCurrentSprint(CurrentUser);
                        if (CurrentSprint != null)
                        {
                            serverCurrentSprintId = CurrentSprint.ID;
                            isUSSChanged = true;
                            isTasksChanged = true;
                            isDetailsChanged = true;
                        }
                    }
                    if (client.IsUsersChangedSince(UpdateFlag))
                    {
                        usersList = client.GetUsersList(CurrentUser);
                        isUsersChanged = true;
                    }
                    ((ICommunicationObject)client).Close();
                }
                updateFlag = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("In CheckServerChanged: " + ex.Message);
            }
            InitRefresh();
        }
        /// <summary>The secondary routine of the synchronization thread, 
        /// checks if the CheckServerChanged method changed something and update
        /// the GUI in accordance.
        /// Mostly the MainWindow will call it, because only the main thread of program can
        /// change the UI elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CheckUpdate(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer)sender;
            timer.Stop();
            try
            {

                if (LastRefresh <= UpdateFlag)
                {
                    if (isUsersChanged)
                    {
                        try
                        {
                            MainWindow.mwmv.UsersListView.Refresh();
                            isUsersChanged = false;
                        }
                        catch (Exception ex)
                        {
                            MainWindow.UpdateStatus("In CheckUpdate-Users: " + ex.Message);
                        }
                    }
                    if (isUSSChanged)
                    {
                        try
                        {
                            MainWindow.mwmv.UserStorysView.Refresh();
                            MainWindow.usvp.ustblv = new UserStoryTable();
                            MainWindow.usvp.ustblv.ustvm.UserStorys = StaticsElements.CurStatElem.CurrentSprint.UserStorys;
                            MainWindow.usvp.ustblv.LoadUS();
                            isUSSChanged = false;
                        }
                        catch (Exception ex)
                        {
                            MainWindow.UpdateStatus("In CheckUpdate-USS: " + ex.Message);
                        }
                    }
                    if (isTasksChanged)
                    {
                        try
                        {
                            MainWindow.mwmv.UserTasksView.Refresh();
                            isTasksChanged = false;
                        }
                        catch (Exception ex)
                        {
                            MainWindow.UpdateStatus("In CheckUpdate-Tasks: " + ex.Message);
                        }
                    }
                    if (isDetailsChanged)
                    {
                        try
                        {
                            MainWindow.mwmv.DetailsBarView.dbvm.Refresh();
                            isDetailsChanged = false;
                        }
                        catch (Exception ex)
                        {
                            MainWindow.UpdateStatus("In CheckUpdate-Details: " + ex.Message);
                        }
                    }
                    LastRefresh = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MainWindow.UpdateStatus("In CheckUpdate: " + ex.Message);
            }
            finally
            {

                timer.Start();
            }
        }

        /// <summary>
        /// Login to the server with the given parameters
        /// </summary>
        /// <param name="client">The object that returned by InitConnection method</param>
        /// <param name="userName">The user-name to login with</param>
        /// <param name="password">The password of the user</param>
        private void Login(IScrumMasterService client, string userName, string password)
        {
            try
            {
                // The login action
                CurrentUser = client.GetUser(userName, password);
                // The user.Name property can indicates errors
                CheckLoginErrors(client, userName, password);

                if (CurrentUser != null)
                    // The login successed so we need to save the user password for future actions
                    CurrentUser.Password = password;
                this.MainWindow.UpdateStatus("Logged in as: " + userName);
            }
            catch (Exception ex)
            {
                this.MainWindow.UpdateStatus("In login:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Check if the login process failed or not
        /// </summary>
        /// <param name="client">The object that returned by InitConnection function</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        private void CheckLoginErrors(IScrumMasterService client, string userName, string password)
        {
            try
            {
                int exCode = -1;
                if (CurrentUser != null)
                    exCode = Convert.ToInt32(CurrentUser.Name);
                string msgText = "Can't login with these name and password.";
                CurrentUser = null;

                switch (exCode)
                {
                    case 113:
                        msgText = "Tha password incorrect.\nDo you want to try again?";
                        return;
                }
                MessageBox.Show(msgText, "Scrum Master Users Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146233033)
                    // This code for Convert.ToInt32(CurrentUser.Name) error
                    // which indicates that the user.Name is legal and not indicates error
                    this.MainWindow.UpdateStatus("In CheckLoginErrors:\n" + ex.Message);
            }
        }
    }
}
