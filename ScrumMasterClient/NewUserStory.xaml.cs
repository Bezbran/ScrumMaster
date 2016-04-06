using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for NewUserStory.xaml
    /// </summary>
    public partial class NewUserStory : UserControl
    {
        public NewUserStory()
        {
            InitializeComponent();
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
