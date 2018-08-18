using System;
using System.Windows;
using VoiceUP.Models;

namespace VoiceUP.Windows
{
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
