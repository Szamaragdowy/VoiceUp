using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VoiceUpServer.Converters;
using MaterialDesignThemes.Wpf;

namespace VoiceUpServer.Converters
{
    public class SoundStatusConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || !(value is bool))
                return null;

            bool SoundStatus = (bool)value;

            if (SoundStatus)
            {
                return new PackIcon { Kind = PackIconKind.HeadphonesOff };
            }
            else
            {
                return new PackIcon { Kind = PackIconKind.Headphones };
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || !(value is PackIcon))
                return null;

            if (((PackIcon)value).Kind == PackIconKind.HeadphonesOff)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
