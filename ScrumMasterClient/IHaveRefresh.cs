namespace ScrumMasterClient
{
    /// <summary>
    /// Forces the deriving classes to implement Refresh method.
    /// </summary>
    public interface IHaveRefresh
    {
        /// <summary>
        /// Refreshing the content of the object
        /// </summary>
        void Refresh();
    }
}
