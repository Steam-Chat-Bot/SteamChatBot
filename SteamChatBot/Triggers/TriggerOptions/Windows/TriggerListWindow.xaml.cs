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
    /// Interaction logic for TriggerListWindow.xaml
    /// </summary>
    public partial class TriggerListWindow : Window
    {
        public TriggerLists TL { get; set; }

        public TriggerListWindow(string triggername)
        {
            InitializeComponent();
            if(triggername == "acceptFriendRequestTrigger")
            {
                roomsLabel.IsEnabled = false;
                roomsBox.IsEnabled = false;
            }
            else if(triggername == "autojoinChatTrigger")
            {
                usersBox.IsEnabled = false;
                usersLabel.IsEnabled = false;
                ignoresBox.IsEnabled = false;
                ignoresLabel.IsEnabled = false;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(nameBox.Text == "")
            {
                MessageBox.Show("You must include a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            TL = new TriggerLists()
            {
                Name = "",
                User = new List<SteamID>(),
                Rooms = new List<SteamID>(),
                Ignore = new List<SteamID>(),
            };

            TL.Name = nameBox.Text;
            List<SteamID> ignores = new List<SteamID>();
            List<SteamID> rooms = new List<SteamID>();
            List<SteamID> users = new List<SteamID>();

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

            TL.Ignore = ignores;
            TL.Rooms = rooms;
            TL.User = users;
            DialogResult = true;
            Close();
        }
    }
}
