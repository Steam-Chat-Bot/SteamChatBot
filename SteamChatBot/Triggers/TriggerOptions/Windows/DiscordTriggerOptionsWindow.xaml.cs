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

namespace SteamChatBot.Triggers.TriggerOptions.Windows
{
    /// <summary>
    /// Interaction logic for DiscordTriggerOptionsWindow.xaml
    /// </summary>
    public partial class DiscordTriggerOptionsWindow : Window
    {
        public NoCommand NC { get; set; }
        public DiscordOptions DO { get; set; }

        NoCommandWindow ncw = new NoCommandWindow();

        public DiscordTriggerOptionsWindow()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            ncw.ShowDialog();
            if(ncw.DialogResult == true)
            {
                NC = ncw.NC;
                DO = new DiscordOptions
                {
                    Name = NC.Name,
                    Token = "",
                    SteamChat = new SteamKit2.SteamID(),
                    DiscordServerID = 0
                };
            }

            if(tokenBox.Text == "")
            {
                MessageBox.Show("Token is required!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                DO.Token = tokenBox.Text;
            }

            if (steamchatBox.Text == "")
            {
                MessageBox.Show("Steam Chat ID is required!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                DO.SteamChat = new SteamKit2.SteamID(Convert.ToUInt64(steamchatBox.Text));
            }

            if (serverIDBox.Text == "")
            {
                MessageBox.Show("Discord Server ID is required!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                DO.DiscordServerID = Convert.ToUInt64(serverIDBox.Text);
            }

            DialogResult = true;
            Close();
        }
    }
}
