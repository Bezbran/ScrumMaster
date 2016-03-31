namespace ScrumMasterClient
{
    /// <summary>
    /// Saving the parameters that the user supllied
    /// </summary>
    public class ConnectModelView : BaseViewModel
    {
        string srvurl;
        string username;
        string password;
        /// <summary>
        /// Stores the URL of the server
        /// </summary>
        public string SrvURL
        {
            get { return srvurl; }
            set
            {
                srvurl = value;
                RaisePropertyChanged("SrvURL");
            }
        }
        /// <summary>
        /// Stores the name of the user who logs on
        /// </summary>
        public string UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                RaisePropertyChanged("UserName");
            }

        }
        /// <summary>
        /// Stores the password of the user who logs on.
        /// Not encrypted
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }
        /// <summary>
        /// Creates new empty object
        /// </summary>
        public ConnectModelView()
        {

        }
    }
}
