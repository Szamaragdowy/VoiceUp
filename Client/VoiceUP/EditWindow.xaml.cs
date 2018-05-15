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
        public String IP { get; set; }
        public String PORT { get; set; }
        public String NAME { get; set; }

        public EditWindow(ServerInfo element)
        {
            InitializeComponent();
            this.Top = System.Windows.SystemParameters.WorkArea.Height - this.Height;
            IP = element.IP;
            PORT = element.Port;
            NAME = element.Name;
            TextBoxIP.Text = IP;
            TextBoxPORT.Text = PORT;
            TextBoxNAME.Text = NAME;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            IP = TextBoxIP.Text;
            PORT = TextBoxPORT.Text;
            NAME = TextBoxNAME.Text;


            this.Close();
        }
    }
}
