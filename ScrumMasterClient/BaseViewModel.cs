using System.ComponentModel;

namespace ScrumMasterClient
{
    /// <summary>
    /// Using to simplify the ViewModel binding by create one method that
    /// will inform  the system when binded property is changed
    /// </summary>
    public class BaseViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Will called whenever property in the ViewModel changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Every deriving class will call it when one of it's properties is changing
        /// </summary>
        /// <param name="field">The name of the property which changed </param>
        protected void RaisePropertyChanged(string field)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(field));
            }
        }
    }
}
