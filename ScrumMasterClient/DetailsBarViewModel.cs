using System;
using System.Threading;

namespace ScrumMasterClient
{
    /// <summary>
    /// The ViewModel class.
    /// Contains some fields and properties which necessary for DetailsBar UI
    /// </summary>
    public class DetailsBarViewModel : BaseViewModel, IHaveRefresh
    {
        private DateTime sprintStart;
        private double sprintDuration;
        private string status = "OK";

        private Nullable<DateTime> newSprintStart;
        private double newSprintDuration = 14;
        /// <summary>
        /// Stores the date when the current sprint started
        /// </summary>
        public DateTime SprintStart
        {
            get
            {
                return sprintStart;
            }

            set
            {
                sprintStart = value;
                RaisePropertyChanged("SprintStart");
            }
        }
        /// <summary>
        /// Stores the duration of the current sprint (in days)
        /// </summary>
        public double SprintDuration
        {
            get
            {
                return sprintDuration;
            }

            set
            {
                sprintDuration = value;
                RaisePropertyChanged("SprintDuration");
            }
        }
        /// <summary>
        /// Stores the capacity of the current user (in days)
        /// </summary>
        public string CurUserCapacity
        {
            get
            {
                if (StaticsElements.CurStatElem != null && StaticsElements.CurStatElem.CurrentSprint != null && StaticsElements.CurStatElem.CurrentUser != null)
                    return StaticsElements.CurStatElem.CurrentSprint.GetUserCapacity(StaticsElements.CurStatElem.CurrentUser.ID).ToString();
                return "Unknown";
            }
            set
            {
                // We need to notify the server about this change
                try
                {
                    double newCap = Convert.ToDouble(value);
                    if (newCap >= 0 && StaticsElements.CurStatElem != null && StaticsElements.CurStatElem.CurrentSprint.GetUserCapacity(StaticsElements.CurStatElem.CurrentUser.ID) != newCap)
                        if (StaticsElements.CurStatElem != null)
                        {
                            try
                            {
                                Thread connThread = new Thread(() => StaticsElements.CurStatElem.SubmitUpdateUserCapacity(newCap));
                                connThread.Start();
                            }
                            catch (Exception ex)
                            {
                                StaticsElements.CurStatElem.MainWindow.UpdateStatus("In UpdateUserCapacity:" + ex.Message);
                            }

                        }
                    RaisePropertyChanged("CurUserCapacity");
                }
                catch
                // Illegal value
                {
                    CurUserCapacity = "-1.111";
                }

            }
        }
        /// <summary>
        /// Stores the date when the new sprint will starting
        /// </summary>
        public Nullable<DateTime> NewSprintStart
        {
            get
            {
                return newSprintStart;
            }

            set
            {
                newSprintStart = value;
                RaisePropertyChanged("NewSprintStart");
            }
        }
        /// <summary>
        /// Stores the duration of the new sprint (in days)
        /// </summary>
        public double NewSprintDuration
        {
            get
            {
                return newSprintDuration;
            }

            set
            {
                newSprintDuration = value;
                RaisePropertyChanged("NewSprintDuration");
            }
        }
        /// <summary>
        /// Stores the status of the connection to the server
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
                RaisePropertyChanged("Status");
            }
        }
        /// <summary>
        /// Refreshing the ViewModel object on demand.
        /// Cause the control UI to change.
        /// </summary>
        public void Refresh()
        {
            if (StaticsElements.CurStatElem != null && StaticsElements.CurStatElem.CurrentSprint != null)
            {
                this.SprintStart = StaticsElements.CurStatElem.CurrentSprint.StartDate;
                this.SprintDuration = StaticsElements.CurStatElem.CurrentSprint.Duration;
                CurUserCapacity = CurUserCapacity;
            }
            NewSprintStart = DateTime.Now;
        }
        /// <summary>
        /// Create new object and initializing it's fields
        /// </summary>
        public DetailsBarViewModel()
        {
            Refresh();
        }
    }
}
