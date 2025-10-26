using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DatabaseClient.Utilities
{
    public class DateTimeToOffsetConverter : IValueConverter
    {
        public object? Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            if (value is DateOnly date)
                return new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue));

            return value;
        }

        public object? ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            if (value is DateTimeOffset dto)
                return DateOnly.FromDateTime(dto.DateTime);

            return value;
        }
    }
}
