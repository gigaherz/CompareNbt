using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CompareNbt.ViewModels
{
    public class ChangeToBrushEntry : ObservableObject
    {
        private string? change;
        private IBrush? brush;

        public string? Change
        {
            get => change;
            set
            {
                if (value != change)
                {
                    change = value;
                    OnPropertyChanged();
                }
            }
        }
        public IBrush? Brush 
        {
            get => brush;
            set
            {
                if (value != brush)
                {
                    brush = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
