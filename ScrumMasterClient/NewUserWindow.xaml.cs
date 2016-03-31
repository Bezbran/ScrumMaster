using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScrumMasterWcf;
using System.Threading;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml,
    /// allow the client to register new member to the team
    /// (Only manager can adding new member)
    /// In future it will can change user details as well.
    /// Most of the server updates in this project are done in seperate
    ///  threads to allow continues work
    /// </summary>
    public partial class NewUserWindow : Window
    {
        /// <summary>
        /// Creates window to create new user
        /// </summary>
        public NewUserWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Creates new window to update details of exsiting user.
        /// </summary>
        public NewUserWindow(User exstUser)
        {
            InitializeComponent();
            
            this.Title = "Updating " + exstUser.Name;
            numv.Name = exstUser.Name;
            numv.Password = "";
            foreach (var pos in exstUser.Positions)
                positionslistBox.SelectedItems.Add(pos);
            numv.ID = exstUser.ID;

            crtBtn.Content = "Update";
            delBtn.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Passing the focus to the next UI element.
        /// Helping to fill the details in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                switch (((Control)sender).Name)
                {
                    case "userNameTB":
                        this.passwordBox.Focus();
                        break;
                    case "passwordBox":
                        this.positionslistBox.Focus();
                        break;
                    case "positionslistBox":
                        this.crtBtn.Focus();
                        break;
                }
        }
        /// <summary>
        /// Creating new user or updating an existing user in the server with the detail in this form
        /// </summary>
        /// <param name="sender">Create user button</param>
        /// <param name="e"></param>
        private void crtBtn_Click(object sender, RoutedEventArgs e)
        {
            // Some UI element don't support binding so well
            this.numv.Password = passwordBox.Password;
            this.numv.Positions = this.positionslistBox.SelectedItems.Cast<User.Position>().ToArray();

            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => { StaticsElements.CurStatElem.SubmitUserChanged(numv); });
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In UserChanged:" + ex.Message);
                }
                this.Close();
            }
        }
        /// <summary>
        /// Deleting an existing user from the server
        /// </summary>
        /// <param name="sender">Delete user button</param>
        /// <param name="e"></param>
        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            if (numv.ID == -1) return;

            MessageBoxResult rslt = MessageBox.Show("Are you sure you want to delete this user?", "Delete user", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rslt != MessageBoxResult.Yes) return;

            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => { StaticsElements.CurStatElem.SubmitUserRemove(numv); });
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In UserChanged:" + ex.Message);
                }
                this.Close();
            }
        }
    }
}
