using ScrumMasterWcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumMasterClient
{
    class UserStoryTableViewModel:BaseViewModel
    {
        private List<UserStory> userStorys = null;
        public List<UserStory> UserStorys
        {
            get
            {
                return userStorys;
            }
            set
            {
                this.userStorys = value;                
                RaisePropertyChanged("UserStorys");
            }
        }
    }
}
