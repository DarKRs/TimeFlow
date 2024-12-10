using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.Utils
{
    public class CategoryToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskCategory category)
            {
                return category switch
                {
                    TaskCategory.UrgentImportant => Color.FromArgb("#FF867C"),  // Розовый коралл
                    TaskCategory.NotUrgentImportant => Color.FromArgb("#80C590"),  // Мягкий зелёный
                    TaskCategory.UrgentNotImportant => Color.FromArgb("#F4D06F"),  // Приглушённый жёлтый
                    TaskCategory.NotUrgentNotImportant => Color.FromArgb("#B0BEC5"),  // Серо-голубой
                    _ => Colors.White
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryToHumanReadableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskCategory category)
            {
                return category switch
                {
                    TaskCategory.UrgentImportant => "Срочная и важная",
                    TaskCategory.NotUrgentImportant => "Не срочная, но важная",
                    TaskCategory.UrgentNotImportant => "Срочная, но не важная",
                    TaskCategory.NotUrgentNotImportant => "Не срочная и не важная",
                    _ => "Не определено"
                };
            }
            return "Не определено";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Color.FromArgb("#80C590") : Colors.Transparent; // Мягкий зелёный для активной вкладки
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Colors.White : Colors.Gray; // Белый для активной вкладки, серый для неактивной
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? FontAttributes.Bold : FontAttributes.None; // Жирный для активной вкладки
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateToDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("dd"); 
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FullDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                return date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CompletionStatus status)
            {
                return status switch
                {
                    CompletionStatus.Done => Colors.Green,
                    CompletionStatus.PartiallyDone => Colors.Orange,
                    CompletionStatus.NotDone => Colors.Red,
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
