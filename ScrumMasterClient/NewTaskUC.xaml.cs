using ScrumMasterWcf;
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
    /// Interaction logic for NewTaskUC.xaml
    /// </summary>
    public partial class NewTaskUC : UserControl
    {
        private OneTaskViewModel otvm = new OneTaskViewModel();
        public NewTaskUC()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (this.Tag != null && this.Tag is UserStory)
            {
                this.otvm.OriginalUserStory = this.Tag as UserStory;
            }
            this.DataContext = otvm;
        }

        /// <summary>
        /// Creates new task with the details in the form
        /// </summary>
        /// <param name="sender">The create button</param>
        /// <param name="e"></param>
        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    if (this.otvm == null || this.otvm.OriginalUserStory == null)
                    {
                            return;
                    }
                    // -1 in HandleUserId value represent future assigment
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitNewTask(this.otvm.OriginalUserStory, otvm.Name, otvm.Description, otvm.PlannedEffort, -1));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In NewTask:" + ex.Message);
                }
            }
        }
    }
}
