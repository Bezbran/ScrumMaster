namespace ScrumMasterWcf
{
    /// <summary>
    /// Interface to enforce the deriving classes to have ID filed. 
    /// The SaveAgent and OpenAgent utilizing it
    /// </summary>
    public interface IHaveID
    {
        /// <summary>
        /// The ID number of the object
        /// </summary>
        int ID { get; }

    }
}
