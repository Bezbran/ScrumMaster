using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScrumMasterClient
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml,
    /// resposible to gathering information to create connection to the server
    /// </summary>
    public partial class ConnectWindow : Window
    {
        Uri serverAddress = null;
        MainWindow mw;
        /// <summary>
        /// Create new object which linked to MainWindow object.
        /// There is no ConnectWindow that not linked to MainWindow object
        /// because of the StaticsElements object that need it.
        /// </summary>
        /// <param name="mw">MainWindow object</param>
        public ConnectWindow(MainWindow mw)
        {
            if(mw==null)
            {
                return;
            }
            InitializeComponent();
            this.mw = mw;
            grid1.DataContext = cnv;
            cnv.SrvURL = "http://localhost:10002/ScrumMaster";
            cnv.UserName = "TeamMember_1";
            connectBtn.Focus();            
        }
        /// <summary>
        /// Helps the user experience by advanced the focus according to "Enter" taps
        /// </summary>
        /// <param name="sender">The control that raise the event</param>
        /// <param name="e">Contains helpfull information</param>
        private void TB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                switch (((Control)sender).Name)
                {
                    case "urlTB":
                        this.userNameTB.Focus();
                        break;
                    case "userNameTB":
                        this.passwordBox.Focus();
                        break;
                    case "passwordBox":
                        this.connectBtn.Focus();
                        break;
                }
        }
        /// <summary>
        /// Establishes connection to the server by using the parameters which diven by user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // password property in passwordbox control don't support binding
            // so we have to connect it by code
            cnv.Password = passwordBox.Password;
            try
            {
                // Checking if the URL adrress which given by user is legit
                serverAddress = new Uri(cnv.SrvURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Trying establishing connection error:\n"+ex.Message);
                cnv.SrvURL = null;
                return;
            }
            // Creating StaticsElements object, it will be the bridge from all client components to the server
            StaticsElements se = new StaticsElements(serverAddress, cnv.UserName, cnv.Password, mw);
            if (StaticsElements.CurStatElem == null || StaticsElements.CurStatElem.CurrentUser == null)
                // The connection trying failed
                return;
            // The connection established
            this.Close();
        }
        /// <summary>
        /// Prevent the MainWindow from being showing without valide connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (StaticsElements.CurStatElem == null || StaticsElements.CurStatElem.CurrentUser == null)                
                Environment.Exit(0);
        }
    }
}
