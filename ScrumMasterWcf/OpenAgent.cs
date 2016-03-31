using System;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Contains statics method to handle openning of scrum-proccess which saved to the disc
    /// in JSON format by using SaveAgent class.
    /// </summary>
    public class OpenAgent
    {
        /// <summary>
        /// Reads from stream to exclude the savd JSON ScrumMasterService data
        /// </summary>
        /// <param name="fileStream">Opening stream object</param>
        /// <returns>The ScrumMasterService object which readed from the stream</returns>
        static public ScrumMasterService OpenWholeScrum(Stream fileStream)
        {
            return DeserializeScrum(fileStream);
        }
        /// <summary>
        /// Reads savd JSON ScrumMasterService data from file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>The ScrumMasterService object which readed from the file</returns>
        static public ScrumMasterService OpenWholeScrum(string fileName)
        {
            ScrumMasterService sms;
            try
            {
                // Create a stream to serialize the object with.
                using (StreamReader fileStream = new StreamReader(fileName))
                {
                    sms = DeserializeScrum(fileStream.BaseStream);
                }
            }
            catch
            {
                sms = null;
            }
            return sms;
        }
        /// <summary>
        /// Attempt to convert data in the stream to data of ScrumMasterService proccess
        /// </summary>
        /// <param name="baseStream">opening stream object </param>
        /// <returns>The ScrumMasterService object which converted from the stream</returns>
        private static ScrumMasterService DeserializeScrum(Stream baseStream)
        {
            SaveAgent sa;
            try
            {
                ScrumMasterService sms;
                DataContractJsonSerializer jseServ = new DataContractJsonSerializer(typeof(SaveAgent));
                // Deserializer the object to the stream.
                sa = (SaveAgent)jseServ.ReadObject(baseStream);
                sms = sa.ScrumMasterServiceSaved;

                // Now we need to deserialize the lastIDs so we can 
                // procceed where we stoped. To sets the LastId fields, they fulfill LastId==0!!!
                User.LastId = sa.UserLastId;
                UserStory.LastId = sa.UserStoryLastId;
                ScrumTask.LastId = sa.ScrumTaskLastId;
                Sprint.LastId = sa.SprintLastId;
                // User class must to know where the user encrypted passwords are saved
                // so the ScrumMasterService have to update it about them
                sms.UpdateUsersPasswords();
                return sms;
            }
            catch
            {
                return null;
            }

        }
    }
}
