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
                    TaskCategory.UrgentImportant => Colors.LightCoral, 
                    TaskCategory.NotUrgentImportant => Colors.LightGreen,
                    TaskCategory.UrgentNotImportant => Colors.LightGoldenrodYellow,
                    TaskCategory.NotUrgentNotImportant => Colors.LightGray,
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

    public class BlockTypeToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeBlockType blockType)
            {
                return blockType switch
                {
                    TimeBlockType.Work => Color.FromArgb("#E8F5E9"), // Зеленоватый
                    TimeBlockType.Break => Color.FromArgb("#FFF3E0"), // Оранжеватый
                    TimeBlockType.Other => Color.FromArgb("#E3F2FD"), // Голубоватый
                    _ => Color.FromArgb("#EEEEEE") // Светло-серый
                };
            }
            return Color.FromArgb("#EEEEEE");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
