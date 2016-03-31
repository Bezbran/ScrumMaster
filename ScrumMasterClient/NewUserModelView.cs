using ScrumMasterWcf;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScrumMasterClient
{
    /// <summary>
    /// The ViewModel class.
    /// Contains some fields and properties which necessary for NewUserWindow UI
    /// </summary>
    public class NewUserModelView : BaseViewModel
    {
        private string name = "";
        private string password = "";
        private User.Position[] positions;
        private ObservableCollection<User.Position> posValues;
        private int id = -1;

        /// <summary>
        /// Creates new object and initialize the possible positions for users.
        /// </summary>
        public NewUserModelView()
        {
            // Getting the possible positions for users.
            // There is only one scrum master user in each team.
            var ps = Enum.GetValues(typeof(User.Position));
            PosValues = new ObservableCollection<User.Position>(ps.OfType<User.Position>());
        }
        /// <summary>
        /// Stores the name of the new user
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        /// <summary>
        /// Stores the Password of the new user
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
        /// Stores the Positions array of the new user
        /// </summary>
        public User.Position[] Positions
        {
            get
            {
                return positions;
            }

            set
            {
                positions = value;
                RaisePropertyChanged("Positions");
            }
        }
        /// <summary>
        /// Stores the possible positions for users.
        /// </summary>
        public ObservableCollection<User.Position> PosValues
        {
            get
            {
                return posValues;
            }

            set
            {
                posValues = value;
                RaisePropertyChanged("PosValues");
            }
        }
        public int ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                RaisePropertyChanged("ID");
            }

        }
    }
}
