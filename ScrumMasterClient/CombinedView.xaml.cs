using ScrumMasterWcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for CombinedView.xaml
    /// </summary>
    public partial class CombinedView : UserControl
    {
        public CombinedView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Tag != null && this.Tag is UserStory)
                UserStoryTable.LoadUS(this.baseGrid, (UserStory)this.Tag);
        }
    }
}
