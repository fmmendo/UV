using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UV
{
    public class SkinTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && value is int)
                {
                    int type = (int)value;

                    switch (type)
                    {
                        case 1: return new SolidColorBrush(Color.FromArgb(255, 238, 186, 159));
                        case 2: return new SolidColorBrush(Color.FromArgb(255, 226, 165, 128));
                        case 3: return new SolidColorBrush(Color.FromArgb(255, 202, 130, 83));
                        case 4: return new SolidColorBrush(Color.FromArgb(255, 188, 116, 61));
                        case 5: return new SolidColorBrush(Color.FromArgb(255, 158, 86, 58));
                        case 6: return new SolidColorBrush(Color.FromArgb(255, 117, 76, 61));
                    }
                }
            }
            catch (Exception)
            {

            }
            return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ExposureColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    switch (value.ToString())
                    {
                        case "None": return new SolidColorBrush(Color.FromArgb(255, 34, 177, 76));
                        case "Low": return new SolidColorBrush(Color.FromArgb(255, 0, 162, 232));
                        case "Medium": return new SolidColorBrush(Color.FromArgb(255, 255, 201, 12));
                        case "High": return new SolidColorBrush(Color.FromArgb(255, 255, 127, 39));
                        case "Very High": return new SolidColorBrush(Color.FromArgb(255, 237, 28, 36));
                        case "Extreme": return new SolidColorBrush(Color.FromArgb(255, 163, 73, 164));
                    }
                }
            }
            catch (Exception)
            {

            }
            return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
