using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Used for save the objects in the scrum-procces in JSON format to the disc.
    /// Contains various methods to handle that.
    /// </summary>
    [DataContract]
    public class SaveAgent
    {
        /// <summary>
        /// Stores the current ScrumMasterService running object
        /// </summary>
        [DataMember]
        public ScrumMasterService ScrumMasterServiceSaved;
        /// <summary>
        /// Stores the lastID value of the User class
        /// </summary>
        [DataMember]
        public int UserLastId;
        /// <summary>
        /// Stores the lastID value of the UserStory class
        /// </summary>
        [DataMember]
        public int UserStoryLastId;
        /// <summary>
        /// Stores the lastID value of the ScrumTask class
        /// </summary>
        [DataMember]
        public int ScrumTaskLastId;
        /// <summary>
        /// Stores the lastID value of the Sprint class
        /// </summary>
        [DataMember]
        public int SprintLastId;
        /// <summary>
        /// create object which contains the whole necessary information about
        /// the current running scrum-proccess.
        /// The last ID values needs to be saved in order to prevent duplicates in the proccess.
        /// </summary>
        /// <param name="sms">Current ScrumMasterService running object</param>
        /// <param name="userLastId">The lastID value of the User class</param>
        /// <param name="userStoryLastId">The last ID value of the UserStory class</param>
        /// <param name="scrumTaskLastId">The last ID value of the ScrumTask class</param>
        /// <param name="sprintLastId">The last ID value of the Sprint class</param>
        public SaveAgent(ScrumMasterService sms, int userLastId, int userStoryLastId, int scrumTaskLastId, int sprintLastId)
        {
            this.ScrumMasterServiceSaved = sms;
            this.UserLastId = userLastId;
            this.UserStoryLastId = userStoryLastId;
            this.ScrumTaskLastId = scrumTaskLastId;
            this.SprintLastId = sprintLastId;
        }
        /// <summary>
        /// Saving the whole data of the running ScrumMasterService object, and
        /// more necessary information from statics fields
        /// </summary>
        /// <param name="sms">The scrum-proccess object to save to disc</param>
        /// <returns>The saved JSON file-name></returns>
        public static string SaveWholeScrum(ScrumMasterService sms)
        {
            string fileName = sms.TeamName + "Saved.json";
            try
            {
                // Create a stream to serialize the object to.
                using (StreamWriter file = new StreamWriter(fileName))
                {
                    DataContractJsonSerializer jseServ = new DataContractJsonSerializer(typeof(SaveAgent));
                    // Create one object which will contain the whole needed information
                    SaveAgent sa = new SaveAgent(sms, User.LastId, UserStory.LastId, ScrumTask.LastId, Sprint.LastId);
                    // Serializer the object to the stream.
                    jseServ.WriteObject(file.BaseStream, sa);
                    fileName = Directory.GetCurrentDirectory() + "\\" + fileName;
                }
            }
            catch (Exception ex)
            {
                fileName = "An error occurred when saving" + ex.Message;
            }
            return fileName;
        }
        /// <summary>
        /// Right now the OpenAgent don't support in opening such object.
        /// </summary>
        /// <param name="savedObj">The object to save to disc</param>
        /// <returns>The saved JSON file-name</returns>
        public static string SaveIHaveID(IHaveID savedObj)
        {
            string fileName = savedObj.ID + "Saved" + savedObj.GetType() + ".json";
            //Create a stream to serialize the Task to.
            try
            {
                using (StreamWriter file = new StreamWriter(fileName))
                {
                    DataContractJsonSerializer jseServ = new DataContractJsonSerializer(savedObj.GetType());
                    // Serializer the object to the stream.
                    jseServ.WriteObject(file.BaseStream, savedObj);
                }
            }
            catch
            {

            }
            return fileName;
        }
    }
}
