using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoiceUP.Structures;

namespace VoiceUP
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        String IP { get; set; }
        String PORT { get; set; }
        String NAME { get; set; }

        public EditWindow(ServerInfo element)
        {
            InitializeComponent();
            this.Top = System.Windows.SystemParameters.WorkArea.Height - this.Height;
            IP = element.IPAdres.Address.ToString();
            PORT = element.IPAdres.Port.ToString();
            NAME = element.Name;
            TextBoxIP.Text = IP;
            TextBoxPORT.Text = PORT;
            TextBoxNAME.Text = NAME;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
