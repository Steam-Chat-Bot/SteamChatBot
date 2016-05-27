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
    /// Interaction logic for NoCommandWindow.xaml
    /// </summary>
    public partial class NoCommandWindow : Window
    {
        public NoCommand NC { get; set; }

        public NoCommandWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(nameBox.Text == "")
            {
                MessageBox.Show("You must include a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                NC = new NoCommand
                {
                    Name = "",
                    TriggerNumbers = new TriggerNumbers(),
                    TriggerLists = new TriggerLists()
                };

                NC.Name = nameBox.Text;
                List<SteamID> ignores = new List<SteamID>();
                List<SteamID> rooms = new List<SteamID>();
                List<SteamID> users = new List<SteamID>();
                if (delayBox.Text == "") NC.TriggerNumbers.Delay = null;
                else NC.TriggerNumbers.Delay = Convert.ToInt32(delayBox.Text);

                if (probBox.Text == "") NC.TriggerNumbers.Probability = null;
                else NC.TriggerNumbers.Probability = (float)Convert.ToDouble(probBox.Text);

                if (timeoutBox.Text == "") NC.TriggerNumbers.Timeout = null;
                else NC.TriggerNumbers.Timeout = Convert.ToInt32(timeoutBox.Text);

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

                NC.TriggerLists.Ignore = ignores;
                NC.TriggerLists.Rooms = rooms;
                NC.TriggerLists.User = users;
                DialogResult = true;
                Close();
            }
        }
    }
}
