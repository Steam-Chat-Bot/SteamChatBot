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

        public ChatCommandApiWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (commandBox.Text == "" || nameBox.Text == "" || apiBox.Text == "")
            {
                MessageBox.Show("You must include a name, API key, and a command.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                CCA = new ChatCommandApi()
                {
                    Name = "",
                    ApiKey = "",
                    ChatCommand = new ChatCommand(),
                };

                CCA.ChatCommand.Command = commandBox.Text;
                CCA.Name = nameBox.Text;
                CCA.ApiKey = apiBox.Text;

                List<SteamID> ignores = new List<SteamID>();
                List<SteamID> rooms = new List<SteamID>();
                List<SteamID> users = new List<SteamID>();
                if (delayBox.Text == "") CCA.ChatCommand.TriggerNumbers.Delay = null;
                else CCA.ChatCommand.TriggerNumbers.Delay = Convert.ToInt32(delayBox.Text);

                if (probBox.Text == "") CCA.ChatCommand.TriggerNumbers.Probability = null;
                else CCA.ChatCommand.TriggerNumbers.Probability = (float)Convert.ToDouble(probBox.Text);

                if (timeoutBox.Text == "") CCA.ChatCommand.TriggerNumbers.Timeout = null;
                else CCA.ChatCommand.TriggerNumbers.Timeout = Convert.ToInt32(timeoutBox.Text);

                if (ignoresBox.Text.Split(',').Length > 0 && ignoresBox.Text != "")
                {
                    foreach (string ignore in ignoresBox.Text.Split(','))
                    {
                        ignores.Add(new SteamID(Convert.ToUInt64(ignore)));
                    }
                }
                if (roomsBox.Text.Split(',').Length > 0 && roomsBox.Text != "")
                {
                    foreach (string room in roomsBox.Text.Split(','))
                    {
                        rooms.Add(new SteamID(Convert.ToUInt64(room)));
                    }
                }
                if (usersBox.Text.Split(',').Length > 0 && usersBox.Text != "")
                {
                    foreach (string user in usersBox.Text.Split(','))
                    {
                        users.Add(new SteamID(Convert.ToUInt64(user)));
                    }
                }

                CCA.ChatCommand.TriggerLists.Ignore = ignores;
                CCA.ChatCommand.TriggerLists.Rooms = rooms;
                CCA.ChatCommand.TriggerLists.User = users;
                DialogResult = true;
                Close();
            }
        }
    }
}
