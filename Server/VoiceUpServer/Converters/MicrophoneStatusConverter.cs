using System;
using System.Globalization;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace VoiceUpServer.Converters
{
    public class MicrophoneStatusConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || !(value is bool)) return null;

            bool MicrophoneStatus = (bool)value;

            if (MicrophoneStatus)
            {
                var x = new PackIcon { Kind = PackIconKind.MicrophoneOff };
                x.Foreground = new SolidColorBrush(Color.FromRgb(System.Convert.ToByte("250"), System.Convert.ToByte("000"), System.Convert.ToByte("000")));

                return x;
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
