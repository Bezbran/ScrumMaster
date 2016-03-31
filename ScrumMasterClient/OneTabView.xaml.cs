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
    /// Interaction logic for OneTabView.xaml
    /// </summary>
    public partial class OneTabView : UserControl
    {

        public OneTabView()
        {
            InitializeComponent();
            tluc.Height = cvuc.Height = StaticsElements.CurStatElem.MainWindow.ActualHeight - StaticsElements.CurStatElem.MainWindow.detailsView.ActualHeight - 10;
            tluc.Width = cvuc.Width = StaticsElements.CurStatElem.MainWindow.ActualWidth - StaticsElements.CurStatElem.MainWindow.usersView.ActualWidth - StaticsElements.CurStatElem.MainWindow.curUserTasksView.ActualWidth - 10;
        }

        private void detailsGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
                SwitchEditingMode();
        }

        private void SwitchEditingMode()
        {
            if (descriptionTB.Visibility != Visibility.Visible)
            {
                UpdateBtn.Visibility = DeleteBtn.Visibility = descriptionTB.Visibility = headerTB.Visibility = priorityTB.Visibility = Visibility.Visible;
                descriptionBlock.Visibility = priorityBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                UpdateBtn.Visibility = DeleteBtn.Visibility = descriptionTB.Visibility = headerTB.Visibility = priorityTB.Visibility = Visibility.Collapsed;
                descriptionBlock.Visibility = priorityBlock.Visibility = Visibility.Visible;
            }

        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var orgTIVM = (TabItemViewModel)Tag;
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitUpdateUS(orgTIVM));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In UpdateBtn_Click: " + ex.Message);
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Are you sure you want to delete this UserStory?", "ScrumMaster", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.No) return;
            var orgTIVM = (TabItemViewModel)Tag;
            if (StaticsElements.CurStatElem != null)
            {
                try
                {
                    Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitRemoveUS(orgTIVM));
                    connThread.Start();
                }
                catch (Exception ex)
                {
                    StaticsElements.CurStatElem.MainWindow.UpdateStatus("In DeleteBtn_Click: " + ex.Message);
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            tluc.Height = cvuc.Height = StaticsElements.CurStatElem.MainWindow.ActualHeight - StaticsElements.CurStatElem.MainWindow.detailsView.ActualHeight - 10;
            tluc.Width = cvuc.Width = StaticsElements.CurStatElem.MainWindow.ActualWidth - StaticsElements.CurStatElem.MainWindow.usersView.ActualWidth - StaticsElements.CurStatElem.MainWindow.curUserTasksView.ActualWidth - 10;
        }
    }
}
