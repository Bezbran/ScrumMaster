using ScrumMasterWcf;
using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;

namespace ScrumMasterClient
{
    /// <summary>
    /// Contains the necessary properties and methods for connecting to
    /// the server and to keep the client synchronised with the server
    /// </summary>
    partial class StaticsElements
    {
        /// <summary>
        /// Updates details of user in the server
        /// </summary>
        /// <param name="numv">Contains the user ID and the new details for the user</param>
        public void SubmitUserRemove(NewUserModelView numv)
        {
            IScrumMasterService client = InitConnection();
            bool delUser = false;
            if (client != null)
            {

                delUser = client.RemoveUser(numv.ID, CurrentUser);

                if (delUser)
                {
                    usersList = client.GetUsersList(CurrentUser);
                }
                else
                {
                    MainWindow.UpdateStatus("Server users manager error. Remember that only manager can change users details");
                }
                ((ICommunicationObject)client).Close();
            }
            isUsersChanged = true;
            updateFlag = DateTime.Now;

        }
        /// <summary>
        /// Upadting the server with the current user capacity in the current sprint
        /// </summary>
        /// <param name="newCap">The new capacity in days</param>
        public void SubmitUpdateUserCapacity(double newCap)
        {
            if (serverCurrentSprintId != CurrentSprint.ID) return;

            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                client.SetUserCapacity(newCap, CurrentUser);
                CurrentSprint = client.GetCurrentSprint(CurrentUser);
                ((ICommunicationObject)client).Close();
                isDetailsChanged = true;
                updateFlag = DateTime.Now;
            }
        }
        /// <summary>
        /// Upadting the server that the current user wants to work on task
        /// </summary>
        /// <param name="st">The task to bind to current user</param>
        public void SubmitTaskToUser(ScrumTask st)
        {
            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                client.BindTaskToUser(st, this.CurrentUser);
                this.CurrentSprint = client.GetCurrentSprint(CurrentUser);
                ((ICommunicationObject)client).Close();
                isUSSChanged = true;
                isTasksChanged = true;
                updateFlag = DateTime.Now;
            }

        }
        /// <summary>
        /// Upadting the server that job (task OR userstory) has new status
        /// </summary>
        /// <param name="job">The job to update</param>
        /// <param name="jobStat">The new status</param>
        public void SubmitJobStatusChanged(Job job, Job.JobStatuses jobStat)
        {
            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                client.ChangeJobStatus(job, jobStat, CurrentUser);
                this.CurrentSprint = client.GetCurrentSprint(CurrentUser);
                ((ICommunicationObject)client).Close();
                isUSSChanged = true;
                isTasksChanged = true;
                updateFlag = DateTime.Now;
            }
        }
        /// <summary>
        /// Upadting the server with new UserStory
        /// </summary>
        /// <param name="tvm">Contains the details of the new UserStory</param>
        public void SubmitNewUS(TasksViewViewModel tvm)
        {
            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                UserStory nus = client.CreateNewUserStory(tvm.NewUSName, tvm.NewUSDescription, tvm.Priority, CurrentUser);
                CurrentSprint = client.GetCurrentSprint(CurrentUser);
                ((ICommunicationObject)client).Close();
                isUSSChanged = true;
                updateFlag = DateTime.Now;
            }
        }
        /// <summary>
        /// Upadting the server;  updating details of existing UserStory
        /// </summary>
        /// <param name="tivm">Contains the new details of the UserStory</param>
        public void SubmitUpdateUS(TabItemViewModel tivm)
        {
            IScrumMasterService client = InitConnection();

            if (client != null && tivm != null && tivm.OriginalUserStory != null)
            {
                UserStory nus = client.UpdateUserStory(tivm.OriginalUserStory.ID, CurrentSprint.ID, new UserStory(tivm.Header, tivm.Description, tivm.Priority), CurrentUser);

                ((ICommunicationObject)client).Close();
                isUSSChanged = true;

            }
        }
        /// <summary>
        /// Removes specific UserStory from the server
        /// </summary>
        /// <param name="tivm">Contains the details of the UserStory to remove</param>
        public void SubmitRemoveUS(TabItemViewModel tivm)
        {
            IScrumMasterService client = InitConnection();

            if (client != null && tivm != null && tivm.OriginalUserStory != null)
            {
                bool nus = client.RemoveUserStory(tivm.OriginalUserStory.ID, CurrentSprint.ID, CurrentUser);
                ((ICommunicationObject)client).Close();
                isUSSChanged = true;
            }
        }
        /// <summary>
        /// Upadting the server to create new user or updating details of existing user
        /// </summary>
        /// <param name="numv">Contains the details of the new user</param>
        /// <returns>The new user or NULL in error</returns>
        public User SubmitUserChanged(NewUserModelView numv)
        {
            IScrumMasterService client = InitConnection();
            User newUser = null;
            if (client != null)
            {
                if (numv.ID == -1)
                    newUser = client.CreateNewUser(numv.Name, numv.Positions, numv.Password, CurrentUser);
                else
                {
                    User exstUser = UsersList.FirstOrDefault((x) => x.ID == numv.ID);
                    exstUser.Name = numv.Name;
                    exstUser.Password = numv.Password;
                    exstUser.Positions = numv.Positions;

                    newUser = client.UpdateExistingUser(numv.ID, exstUser, CurrentUser);
                }

                if (newUser != null)
                {
                    usersList = client.GetUsersList(CurrentUser);
                    newUser = CheckUserErrors(newUser);
                }
                else
                {
                    MainWindow.UpdateStatus("Server users manager error. Remember that only manager can change users details");
                }
                ((ICommunicationObject)client).Close();
            }
            isUsersChanged = true;
            updateFlag = DateTime.Now;
            return newUser;
        }
        /// <summary>
        /// Upadting the server with new task
        /// </summary>
        /// <param name="nus">The UserStory which the new task belong to</param>
        /// <param name="name">The new task name/header</param>
        /// <param name="description">The new task description</param>
        /// <param name="plannedEffort">The effort (in days, can be fractions) that new task suppose to take</param>
        /// <param name="handlerUserID">The user that will work on this task. (-1 if it not setted now)</param>
        public void SubmitNewTask(UserStory nus, string name, string description, double plannedEffort, int handlerUserID)
        {
            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                ScrumTask st = client.CreateNewScrumTask(nus, name, description, plannedEffort, handlerUserID, CurrentUser);
                CurrentSprint = client.GetCurrentSprint(CurrentUser);
                ((ICommunicationObject)client).Close();

                isUSSChanged = true;
                updateFlag = DateTime.Now;
            }
        }
        /// <summary>
        /// Upadting the server;  updating details of existing ScrumTask
        /// </summary>
        /// <param name="nus">The UserStory which the task belong to</param>
        /// <param name="st">Contains the updated details. The ID field MUST be the id of the original ScrumTask</param>
        public void SubmitUpdateTask(UserStory nus, int existSTID, ScrumTask st)
        {
            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                st = client.UpdateScrumTask(nus.ID, CurrentSprint.ID, existSTID, st, CurrentUser);
                ((ICommunicationObject)client).Close();

                isUSSChanged = true;
            }
            string tasksHeaders = "";
            isUSSChanged = true;
            isTasksChanged = true;
        }
        /// <summary>
        /// Removes specific ScrumTask from the server
        /// </summary>
        /// <param name="nus">The UserStory which the task belong to</param>
        /// <param name="st">Contains the updated details. The ID field MUST be the id of the original ScrumTask</param>
        public void SubmitRemoveTask(UserStory nus, int existSTID)
        {
            IScrumMasterService client = InitConnection();

            if (client != null)
            {
                client.RemoveScrumTask(nus.ID, CurrentSprint.ID, existSTID, CurrentUser);
                ((ICommunicationObject)client).Close();

                isUSSChanged = true;
                isTasksChanged = true;
            }
        }
        /// <summary>
        /// Asking the server for the sprint which preceded to the current client sprint,
        /// and set it to be the new current client sprint
        /// </summary>
        public void SubmitGetPrevSprint()
        {
            IScrumMasterService client = InitConnection();
            if (client != null)
            {
                ChangeSprint(client.GetPreviosSprint, client);
            }
        }
        /// <summary>
        /// Asking the server for the sprint which following to the current client sprint,
        /// and set it to be the new current client sprint
        /// </summary>
        public void SubmitGetNextSprint()
        {
            IScrumMasterService client = InitConnection();
            if (client != null)
            {
                ChangeSprint(client.GetNextSprint, client);
            }

        }
        /// <summary>
        /// Upadting the server with new sprint
        /// </summary>
        /// <param name="newSprintStart">The new sprint starting date</param>
        /// <param name="newSprintDuration">The new sprint duration (in days, can be fractions)</param>
        public void SubmitSprintChanged(DateTime? newSprintStart, double newSprintDuration)
        {
            IScrumMasterService client = InitConnection();
            if (client != null)
            {
                ChangeSprint(client.CreateNewSprint, newSprintStart, newSprintDuration, client);
            }
        }
        /// <summary>
        /// Handles the change of the current sprint in accordance to the given parameters
        /// </summary>
        /// <param name="newSprint">Function of the server which takes an ID of sprint and user certificates and returns another sprint</param>
        /// <param name="client">The object that returned by InitConnection method</param>
        private void ChangeSprint(Func<int, User, Sprint> newSprint, IScrumMasterService client)
        {
            if (client != null)
            {
                if (CurrentSprint == null)
                {
                    ((ICommunicationObject)client).Close();
                    return;
                }
                Sprint tmpSprint = CurrentSprint;
                CurrentSprint = newSprint(CurrentSprint.ID, CurrentUser);
                ((ICommunicationObject)client).Close();
                if (CurrentSprint != null)
                {
                    isUSSChanged = true;
                    isTasksChanged = true;
                    isDetailsChanged = true;
                    updateFlag = DateTime.Now;
                }
                else
                    CurrentSprint = tmpSprint;
            }
        }
        /// <summary>
        /// Handles the change of the current sprint in accordance to the given parameters
        /// </summary>
        /// <param name="newSprint">Function that creates new sprint in the server.Takes details for the new sprint and user certificates and returns the new sprint</param>
        /// <param name="newSprintStart">The new sprint starting date</param>
        /// <param name="newSprintDuration">The new sprint duration (in days, can be fractions)</param>
        /// <param name="client">The object that returned by InitConnection method</param>
        private void ChangeSprint(Func<Nullable<DateTime>, double, User, Sprint> newSprint, Nullable<DateTime> newSprintStart, double newSprintDuration, IScrumMasterService client)
        {
            if (client != null)
            {
                Sprint tmpSprint = CurrentSprint;
                CurrentSprint = newSprint(newSprintStart, newSprintDuration, CurrentUser);
                ((ICommunicationObject)client).Close();
                if (CurrentSprint != null)
                {

                    isUSSChanged = true;
                    isTasksChanged = true;
                    isDetailsChanged = true;
                    serverCurrentSprintId = CurrentSprint.ID;
                    updateFlag = DateTime.Now;
                }
                else
                    CurrentSprint = tmpSprint;
            }
        }
        /// <summary>
        /// Check if User object which was returned by the server indicates error or not
        /// </summary>
        /// <param name="newUser">The object which returns from the server</param>
        /// <returns>NULL - if any error has been identified. The newUser object if no error has been identified</returns>
        private User CheckUserErrors(User newUser)
        {
            int exCode;
            try
            {
                // If user.Name is int it's indicating error
                exCode = Convert.ToInt32(newUser.Name);
                string msgText = "You choosed a name which contains only numbers.\nPlease choose another name";
                switch (exCode)
                {
                    case 112:
                        msgText = "Manager user is already exist.";
                        break;
                    case 114:
                        msgText = "The name is already in use.\nPlease choose another one.";
                        break;
                    case 115:
                        msgText = "Scrum-master user is already exist.";
                        break;
                    case 116:
                        msgText = "Product-owner user is already exist.";
                        break;
                    case 113:
                        msgText = "Authentication failed.\nCheck the user name and the password again.";
                        break;
                    default:
                        break;
                }

                if (this.MainWindow != null && CurrentUser != null)
                    this.MainWindow.UpdateStatus("In CheckUserErrors:\n" + msgText);
                else
                    MessageBox.Show(msgText, "Scrum Master Users Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146233033)
                    // This code for Convert.ToInt32(CurrentUser.Name) error
                    // which indicates that the user.Name is legal and not indicates error
                    this.MainWindow.UpdateStatus("In CheckUserErrors:\n" + ex.Message);
                return newUser;
            }
        }
    }
}
