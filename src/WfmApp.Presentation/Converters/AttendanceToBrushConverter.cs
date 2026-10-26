using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WfmApp.Presentation.ViewModels;

namespace WfmApp.Presentation.Converters;

public sealed class AttendanceToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AttendanceStatus status)
            return Brushes.Gray;

        return status switch
        {
            AttendanceStatus.OnShift => Brushes.Green,
            AttendanceStatus.Late => Brushes.OrangeRed,
            AttendanceStatus.Finished => Brushes.Gray,
            _ => Brushes.SteelBlue
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
