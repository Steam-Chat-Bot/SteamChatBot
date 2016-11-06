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

using SteamKit2;

namespace SteamChatBot.Triggers.TriggerOptions.Windows
{
    /// <summary>
    /// Interaction logic for ChatCommandApiWindow.xaml
    /// </summary>
    public partial class ChatCommandApiWindow : Window
    {
        public ChatCommandApi CCA { get; set; }
        public ChatCommand CC { get; set; }

        ChatCommandWindow ccw = new ChatCommandWindow();
        public ChatCommandApiWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (apiBox.Text == "")
            {
                MessageBox.Show("You must include an API key", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                ccw.ShowDialog();
                if (ccw.DialogResult.HasValue && ccw.DialogResult == true)
                {
                    CC = ccw.CC;
                    CCA = new ChatCommandApi()
                    {
                        Name = CC.Name,
                        ApiKey = "",
                        ChatCommand = new ChatCommand()
                    };

                    if (apiBox.Text == "")
                    {
                        MessageBox.Show("You must include an API key.", "Error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    }
                    else
                    {
                        CCA.ApiKey = apiBox.Text;
                    }

                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
