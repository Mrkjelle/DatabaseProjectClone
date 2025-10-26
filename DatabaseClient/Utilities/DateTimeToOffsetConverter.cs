using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace DatabaseClient.Utilities
{
    public class DateTimeToOffsetConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue || dt == default)
                    return null; // skip invalid defaults
                return new DateTimeOffset(dt, TimeSpan.Zero);
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset dto)
                return dto.DateTime;
            return null;
        }
    }
}