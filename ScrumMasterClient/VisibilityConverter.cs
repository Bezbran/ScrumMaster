using System;
using System.Windows;
using System.Windows.Data;

namespace ScrumMasterClient
{

    /// <summary>
    /// Convert boolean value to Visibility and viseversa.
    /// Used in XAML Visibility values
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Constant string "Invert"
        /// </summary>
        public const string Invert = "Invert";

        #region IValueConverter Members
        /// <summary>
        /// Convert boolean value to Visibility value and viseversa
        /// </summary>
        /// <param name="value">The original value</param>
        /// <param name="targetType">The type to convert the value to</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The converted value</returns>
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility.");

            bool? bValue = (bool?)value;

            if (parameter != null && parameter as string == Invert)
                bValue = !bValue;

            return bValue.HasValue && bValue.Value ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
