using System;
using ScrumMasterWcf;
using System.IO;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Globalization;
using System.Net;

namespace ScrumMasterServGUI
{
    /// <summary>
    /// Represent the main window of the program
    /// </summary>
    public partial class ScrumMasterWorkForm : Form
    {
        /// <summary>
        /// The host object of this server
        /// </summary>
        ServiceHost host = null;
        /// <summary>
        /// The service adrress
        /// </summary>
        Uri baseAddress;
        /// <summary>
        /// The service object
        /// </summary>
        ScrumMasterService sms;
        /// <summary>
        /// New form constructor
        /// </summary>
        public ScrumMasterWorkForm()
        {
            InitializeComponent();
            // To prevent language problems in errors, we will change the CultureInfo to english
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            InitIpAdrresses();
            //GenerateTestSM();
        }
        /// <summary>
        /// Alllows the user to run automatic created team for test purpose
        /// </summary>
        /// <param name="isTest"></param>
        public ScrumMasterWorkForm(bool isTest)
        {
            InitializeComponent();
            // To prevent language problems in errors, we will change the CultureInfo to english
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            InitIpAdrresses();
            if (isTest)
                GenerateTestSM();
        }
        /// <summary>
        /// Make a list of all IP's adrresses of the current machine and show them in ComboBox
        /// </summary>
        private void InitIpAdrresses()
        {
            string myHostName = Dns.GetHostName().ToString();
            var IPAdrresses = Dns.GetHostEntry(myHostName).AddressList;
            ipCB.Items.AddRange(IPAdrresses);
            // Check which of the ip's is exposed to the local net 
            for (int i = 0; i < IPAdrresses.Length; i++)
                if (IPAdrresses[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipCB.SelectedIndex = i;

                }


        }

        /// <summary>
        /// When user tap the "Enter" key, it will move the focus to the next control
        /// </summary>
        /// <param name="sender">The textBox</param>
        /// <param name="e">Contains necessary data</param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender.GetType() != typeof(TextBox)) return;
            if (e.KeyCode == Keys.Enter)
            {
                var senderTB = (TextBox)sender;
                switch (senderTB.Name)
                {
                    case "scrumNameTB":
                        teamManagerNameTB.Focus();
                        break;
                    case "teamManagerNameTB":
                        teamManagerPasswordTB.Focus();
                        break;
                    case "teamManagerPasswordTB":
                        createTeamBTN.Focus();
                        break;
                }
            }
        }
        /// <summary>
        /// Create a automatic text ScrumService with users, userStorys and tasks for testing purpose
        /// </summary>
        private void GenerateTestSM()
        {
            this.WindowState = FormWindowState.Minimized;
            scrumNameTB.Text = "TEST_Run";
            teamManagerNameTB.Text = "ManagerUser";
            teamManagerPasswordTB.Text = "1234";
            createTeamBTN_Click(scrumNameTB, new KeyEventArgs(Keys.Enter));
            User.Position[] posA = { User.Position.ScrumMaster, User.Position.ProductOwner };
            User manager = sms.UsersList[0];
            manager.Password = teamManagerPasswordTB.Text;
            sms.CreateNewUser("TeamMember_0", posA, teamManagerPasswordTB.Text, sms.UsersList[0]);
            User scrumMasterANDProductOwner = sms.UsersList[1];
            scrumMasterANDProductOwner.Password = teamManagerPasswordTB.Text;
            Sprint sprnt = sms.CreateNewSprint(DateTime.Now, 14, scrumMasterANDProductOwner);
            User.Position[] optionalPoss = { User.Position.Architect, User.Position.Customer, User.Position.Developer, User.Position.QA, User.Position.QC };
            posA = new User.Position[] { User.Position.Developer };
            for (int i = 1; i < 10; i++)
            {
                int j = i % 5;
                posA = new User.Position[] { optionalPoss[j], optionalPoss[(j + 1) % 4] };
                sms.CreateNewUser("TeamMember_" + i, posA, teamManagerPasswordTB.Text, manager);
                sms.CreateNewUserStory("UserStory_" + (10 - i), "UserStory TEST number" + i, 4, scrumMasterANDProductOwner);

            }

            foreach (UserStory us in sms.CurrentSprint.UserStorys)
            {
                for (int i = 0; i < 15; i++)
                    us.AddTask(new ScrumTask("TEST ScrumTsk " + i, "Test Desc " + i + " ..!!!", i * 1.5, 1 + i % 7));
            }
        }
        /// <summary>
        /// Handle the "open" / "stop and save" button action
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e"></param>
        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (host != null)
            // There is running server object and we need stop&save
            {
                // If any exception occurd, fileName will contain it's details
                string fileName = SaveAgent.SaveWholeScrum(sms);
                stopService();
                fileNameTB.Text += "The scrum saved to: " + fileName;

            }
            else
            // There is no running server so we will open existing one
            {
                // Open exist scrum. Showing OpenFileDialog to the user
                OpenFileDialog ofd = new OpenFileDialog();
                var ofdRslt = ofd.ShowDialog();
                if (ofdRslt == DialogResult.OK)
                {
                    using (Stream file = ofd.OpenFile())
                    {
                        sms = OpenAgent.OpenWholeScrum(file);
                        RunService(sms);
                    }
                    scrumNameTB.Text = sms.TeamName;
                    OpenBtn.Click += OpenBtn_Click;
                }
            }
        }
        /// <summary>
        /// Stopping the host of the service.
        /// </summary>
        private void stopService()
        {
            // Close the ServiceHost.
            host.Close();
            host = null;
            turnStopStart(false);
        }
        /// <summary>
        /// Starting the host of the service.
        /// </summary>
        /// <param name="sms">The service object which represent the wanted scrum proccess</param>
        private void RunService(ScrumMasterService sms)
        {
            // Create the ServiceHost.
            try
            {
                // Creating UriBuilder object to manage the url easily
                var hostExt = new UriBuilder(urlTB.Text);
                // Register the host to all IPs adrresses of the machine
                hostExt.Host = "0.0.0.0";
                host = new ServiceHost(sms, hostExt.Uri);
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                // Register the custom behavior which implemented in ScrumMasterService class.
                // This behavior enable us to run before and after call methods
                host.AddServiceEndpoint(typeof(IScrumMasterService), new BasicHttpBinding(), hostExt.Uri);
                host.Description.Endpoints[0].EndpointBehaviors.Add(sms);

                // Open the ServiceHost to start listening for messages.
                host.Open();

                // Setting back the service url to the user choice
                hostExt.Host = ipCB.Text;
                // Showing the service url
                urlTB.Text = hostExt.Uri.AbsoluteUri;
                turnStopStart(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Handle the stop without saving button action
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e"></param>
        private void stpBtn_Click(object sender, EventArgs e)
        {
            stopService();
        }
        /// <summary>
        /// Manage GUI elements, according to isStart value.
        /// We need it because many GUI elements play few roles
        /// </summary>
        /// <param name="isStart">true - if the service is running. false - if the service stoped</param>
        private void turnStopStart(bool isStart)
        {
            OpenBtn.Text = isStart ? "Click to save and stop" : "OR open an existing team";
            scrumNameTB.Enabled = !isStart;
            teamManagerNameTB.ReadOnly = isStart;
            teamManagerPasswordTB.Enabled = !isStart;
            createTeamBTN.Enabled = !isStart;
            urlTB.ReadOnly = isStart;
            stpBtn.Visible = isStart;
            scrumNameTB.Text = !isStart ? "Choose a name to the scrum team" : scrumNameTB.Text;
            fileNameTB.Visible = !isStart;
        }
        /// <summary>
        /// Handle IPs ComboBox SelectedChanged event.
        /// Update the url TextBox respectively
        /// </summary>
        /// <param name="sender">The ComboBox</param>
        /// <param name="e">Necessary data</param>
        private void ipCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Creating UriBuilder object to manage the url easily
                var hostExt = new UriBuilder(urlTB.Text);
                hostExt.Host = ipCB.SelectedItem.ToString();
                urlTB.Text = hostExt.Uri.AbsoluteUri;
            }
            catch
            {

            }
        }
        /// <summary>
        /// Creates new running ScrumMasterService with the detail in the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createTeamBTN_Click(object sender, EventArgs e)
        {
            if (teamManagerPasswordTB.Text == "")
            {
                MessageBox.Show("The manager password can't be empty", "ScrumMaster server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Convert.ToInt32(teamManagerNameTB.Text);
                MessageBox.Show("The manager name can't be number", "ScrumMaster server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch
            {

            }
            // Running the server
            baseAddress = new Uri(urlTB.Text);
            sms = new ScrumMasterService(scrumNameTB.Text);
            RunService(sms);
            // Creating the manager user
            User.Position[] posA = { User.Position.Manager };
            User managerUser = new User(teamManagerNameTB.Text, teamManagerPasswordTB.Text, posA);
            managerUser.Password = teamManagerPasswordTB.Text;
            // We need to inject it to the list in ordr the authentiction will success, and after
            // add the manager with authentiction check we will remove the injection
            sms.UsersList.Add(managerUser);
            User rslt = sms.CreateNewUser(teamManagerNameTB.Text, posA, teamManagerPasswordTB.Text, sms.UsersList[0]);
            int exCode = -1;
            if (int.TryParse(rslt.Name, out exCode))
            // Something got wrong
            {
                MessageBox.Show("Error occured when adding manager user\nPlease try again", "Scrum Master Server GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Once we added the manager with authentication, we can remove the injected user
            sms.UsersList.RemoveAt(0);
        }
    }
}
