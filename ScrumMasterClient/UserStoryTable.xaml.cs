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
using ScrumMasterWcf;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for UserStoryTable.xaml
    /// </summary>
    public partial class UserStoryTable : UserControl
    {
        private static Job.JobStatuses[] possibleStatuses;
        public UserStoryTable()
        {
            InitializeComponent();
        }
        internal void LoadUS()
        {
            try
            {
                baseGrid.RowDefinitions.Add(new RowDefinition());
                if (possibleStatuses == null) InitPossibleStatuses();
                for (int i = 0; i < ustvm.UserStorys.Count; i++)
                {
                    var row = new RowDefinition();
                    row.Height = GridLength.Auto;
                    baseGrid.RowDefinitions.Add(row);
                    TextBlock tb = new TextBlock();
                    tb.Text = ustvm.UserStorys[i].Header;
                    Grid.SetRow(tb, i + 1);
                    Grid.SetColumn(tb, 0);
                    baseGrid.Children.Add(tb);

                    for (int j = 0; j < possibleStatuses.Length; j++)
                    {
                        var statSTList = ustvm.UserStorys[i].ScrumTasks.FindAll((x) => x.JobStatus == possibleStatuses[j]);
                        if (statSTList == null || statSTList.Count < 1) continue;
                        TasksView tv = new TasksView();
                        tv.OriginalUS = ustvm.UserStorys[i];
                        tv.tvm.ScrumTasksList = statSTList;
                        tv.tasksLB.ItemsSource = tv.tvm.ScrumTasksList;
                        Grid.SetRow(tv, i + 1);
                        Grid.SetColumn(tv, j + 1);
                        baseGrid.Children.Add(tv);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void InitPossibleStatuses()
        {
            List<Job.JobStatuses> tmpList = new List<Job.JobStatuses>((Job.JobStatuses[])typeof(Job.JobStatuses).GetEnumValues());
            tmpList.Remove(Job.JobStatuses.Accepted);
            possibleStatuses = tmpList.ToArray();
        }

        static internal void LoadUS(Grid baseGrid, UserStory orgUS)
        {
            try
            {
                if (possibleStatuses == null) InitPossibleStatuses();
                if (orgUS != null)
                {
                    double maxTasks = 0;
                    baseGrid.RowDefinitions.Add(new RowDefinition());
                    for (int j = 0; j < possibleStatuses.Length; j++)
                    {
                        var statSTList = orgUS.ScrumTasks.FindAll((x) => x.JobStatus == possibleStatuses[j]);
                        if (statSTList == null || statSTList.Count < 1) continue;
                        if (maxTasks < statSTList.Count)
                            maxTasks = statSTList.Count;
                        TasksView tv = new TasksView();
                        tv.Margin = new Thickness(5);
                        tv.OriginalUS = orgUS;
                        tv.tvm.ScrumTasksList = statSTList;
                        tv.tasksLB.ItemsSource = tv.tvm.ScrumTasksList;
                        Grid.SetRow(tv, 1);
                        Grid.SetColumn(tv, j);
                        baseGrid.Children.Add(tv);
                        baseGrid.Height = 100 * maxTasks;
                    }

                }
            }
            catch (Exception ex)
            {

            }
}
    }
}
