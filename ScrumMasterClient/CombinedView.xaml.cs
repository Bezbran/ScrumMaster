using ScrumMasterWcf;
using System.Windows;
using System.Windows.Controls;

namespace ScrumMasterClient
{
    /// <summary>
    /// Shows the tasks in table order by them status
    /// </summary>
    public partial class CombinedView : UserControl
    {
        /// <summary>
        /// Initializing new combined view of userstorys.
        /// One need to set the Tag property to UserStory object in order to make this view available,
        ///  otherwise the UserControl_Loaded will failed.
        /// </summary>

        public CombinedView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Init the Userstorys into the view, working only if Tag property is UserStory object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag != null && this.Tag is UserStory)
                UserStoryTable.LoadUS(this.baseGrid, (UserStory)this.Tag);
        }
    }
}
