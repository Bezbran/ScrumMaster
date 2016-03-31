using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScrumMasterClient
{
    // Important: The view model class is at the end of this file
    /// <summary>
    /// Interaction logic for DetailsBar.xaml.
    /// Showing some details about the scrum-process,
    /// such as current sprint details, option to create new sprint,
    /// connection status, etc.
    /// Most of the server updates in this project are done in seperate
   ///  threads to allow continues work
    /// </summary>
    public partial class DetailsBar : UserControl
    {
        /// <summary>
        /// Creates new object and initialize it's fields
        /// MUST be called after the StaticsElements.CurStatElem!=null
        /// </summary>
        public DetailsBar()
        {
            InitializeComponent();
            if (StaticsElements.CurStatElem == null || StaticsElements.CurStatElem.CurrentSprint == null) return;
            this.dbvm.SprintStart = StaticsElements.CurStatElem.CurrentSprint.StartDate;
            this.dbvm.SprintDuration = StaticsElements.CurStatElem.CurrentSprint.Duration;
            // It's not make sense that spring will begin in the past.
            this.newSprintDP.BlackoutDates.AddDatesInPast();
        }
        /// <summary>
        /// Send to the server the new capacity of this flient user.
        /// (The capacity is limited to the sprint duration) 
        /// </summary>
        /// <param name="sender">The capacity textbox</param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                dbvm.CurUserCapacity = ((TextBox)sender).Text;
        }
        /// <summary>
        /// Creating new sprint on the server.
        /// (Only scrum-master user can do it)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newSprintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitSprintChanged(dbvm.NewSprintStart, dbvm.NewSprintDuration));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In NewSprint:" + ex.Message);
                }

            }
        }
        /// <summary>
        /// Asking from the server the sprint before the client sprint
        /// </summary>
        /// <param name="sender">Previous sprint button</param>
        /// <param name="e"></param>
        private void prevSprintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitGetPrevSprint());
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In PrevSprint:" + ex.Message);
                }

            }
        }
        /// <summary>
        /// Asking from the server the sprint after the client sprint
        /// </summary>
        /// <param name="sender">Next sprint button</param>
        /// <param name="e"></param>
        private void nextSprintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitGetNextSprint());
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In NextSprint:" + ex.Message);
                }

            }
        }
    }
}
