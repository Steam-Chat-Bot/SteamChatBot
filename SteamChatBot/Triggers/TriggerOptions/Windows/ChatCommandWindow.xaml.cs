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
    /// Interaction logic for ChatCommandWindow.xaml
    /// </summary>
    public partial class ChatCommandWindow : Window
    {
        public ChatCommand CC { get; set; }

        public ChatCommandWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (commandBox.Text == "" || nameBox.Text == "")
            {
                MessageBox.Show("You must include a name and a command.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
            else
            {
                CC = new ChatCommand()
                {
                    Name = "",
                    Command = "",
                    TriggerNumbers = new TriggerNumbers(),
                    TriggerLists = new TriggerLists()
                };

                CC.Command = commandBox.Text;
                CC.Name = nameBox.Text;
                List<SteamID> ignores = new List<SteamID>();
                List<SteamID> rooms = new List<SteamID>();
                List<SteamID> users = new List<SteamID>();
                if (delayBox.Text == "") CC.TriggerNumbers.Delay = null;
                else CC.TriggerNumbers.Delay = Convert.ToInt32(delayBox.Text);

                if (probBox.Text == "") CC.TriggerNumbers.Probability = null;
                else CC.TriggerNumbers.Probability = (float)Convert.ToDouble(probBox.Text);

                if (timeoutBox.Text == "") CC.TriggerNumbers.Timeout = null;
                else CC.TriggerNumbers.Timeout = Convert.ToInt32(timeoutBox.Text);

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

                CC.TriggerLists.Ignore = ignores;
                CC.TriggerLists.Rooms = rooms;
                CC.TriggerLists.User = users;
                DialogResult = true;
                Close();
            }
        }
    }
}
