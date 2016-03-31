using System.Windows;
using System.Windows.Controls;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for UsersListView.xaml,
    /// responsible to show the members of the scrum team
    /// and their roles
    /// </summary>
    public partial class UsersListView : UserControl, IHaveRefresh
    {
        //TODO: Save state before refresh
        /// <summary>
        /// Creates new object and initialize it's fields
        /// </summary>
        public UsersListView()
        {
            InitializeComponent();
            Refresh();
        }
        /// <summary>
        /// Load the list of the users that returned by the server
        /// </summary>
        public void Refresh()
        {
            if (StaticsElements.CurStatElem != null && StaticsElements.CurStatElem.UsersList != null)
                usersListViewLV.ItemsSource = StaticsElements.CurStatElem.UsersList;
        }
        /// <summary>
        /// Show the window that creates new team-member
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newUsrBtn_Click(object sender, RoutedEventArgs e)
        {
            new NewUserWindow().ShowDialog();
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2) return;
            FrameworkElement fe = sender as FrameworkElement;
            if (fe.Tag is ScrumMasterWcf.User)
                new NewUserWindow((ScrumMasterWcf.User)fe.Tag).ShowDialog();
        }
    }
}
