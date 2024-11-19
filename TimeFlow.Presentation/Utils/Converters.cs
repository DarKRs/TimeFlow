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


}
