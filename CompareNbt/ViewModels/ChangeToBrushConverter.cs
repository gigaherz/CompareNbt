using Avalonia.Data.Converters;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace CompareNbt.ViewModels
{
    public class ChangeToBrushConverter : ObservableObject, IValueConverter
    {
        public ObservableCollection<ChangeToBrushEntry> Entries { get; set; } = [];

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var entry = Entries.FirstOrDefault(e => e.Change == str);
                if (entry != null)
                    return entry.Brush;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Brush br)
            {
                var entry = Entries.FirstOrDefault(e => e.Brush == br);
                if (entry != null)
                    return entry.Change;
            }
            return null;
        }
    }
}
