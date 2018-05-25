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
    public class MicrophoneStatusConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || !(value is bool))
                return null;

            bool MicrophoneStatus = (bool)value;

            if (MicrophoneStatus)
            {
 
                return new PackIcon { Kind = PackIconKind.MicrophoneOff };
            }
            else
            {
                return new PackIcon { Kind = PackIconKind.Microphone };
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || !(value is PackIcon))
                return null;

            if (((PackIcon)value).Kind == PackIconKind.MicrophoneOff)
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
