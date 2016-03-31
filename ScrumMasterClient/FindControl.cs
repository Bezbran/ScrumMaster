using System.Windows;
using System.Windows.Media;

namespace ScrumMasterClient
{
    /// <summary>
    /// Contains recursive functions which navigeting down the visual tree
    /// to find control/s from type T
    /// </summary>
    /// <typeparam name="T">The type of the required control</typeparam>
    public static class FindControl<T> where T : DependencyObject
    {
        /// <summary>
        /// A recursive function which navigeting down the visual tree
        ///  to find the T which is inherite from DependencyObject of given task of the itemContainer.
        /// </summary>
        /// <param name="itemContainer">Container from item in curTasksListViewLV.Items</param>
        /// <returns>The T object</returns>
        public static T FindControlInViewTree(DependencyObject itemContainer)
        {
            T rslt = default(T);
            if (itemContainer == null) return rslt;
            int childCount = VisualTreeHelper.GetChildrenCount(itemContainer);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(itemContainer, i);
                if (child != null && child is T)
                    return child as T;
                else
                {
                    T childOfChild = FindControlInViewTree(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return rslt;
        }
    }
}
