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
    /// Interaction logic for ChatReplyWindow.xaml
    /// </summary>
    public partial class ChatReplyWindow : Window
    {
        public ChatReply CR { get; set; }

        public ChatReplyWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(matchesBox.Text == "" || responsesBox.Text == "" || nameBox.Text == "")
            {
                MessageBox.Show("You must include matches, responses, and a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                CR = new ChatReply
                {
                    Name = "",
                    Matches = new List<string>(),
                    Responses = new List<string>(),
                    TriggerLists = new TriggerLists(),
                    TriggerNumbers = new TriggerNumbers()
                };

                CR.Name = nameBox.Text;
                CR.Matches = matchesBox.Text.Split(',').ToList();
                CR.Responses = responsesBox.Text.Split(',').ToList();

                List<SteamID> ignores = new List<SteamID>();
                List<SteamID> rooms = new List<SteamID>();
                List<SteamID> users = new List<SteamID>();
                if (delayBox.Text == "") CR.TriggerNumbers.Delay = null;
                else CR.TriggerNumbers.Delay = Convert.ToInt32(delayBox.Text);

                if (probBox.Text == "") CR.TriggerNumbers.Probability = null;
                else CR.TriggerNumbers.Probability = (float)Convert.ToDouble(probBox.Text);

                if (timeoutBox.Text == "") CR.TriggerNumbers.Timeout = null;
                else CR.TriggerNumbers.Timeout = Convert.ToInt32(timeoutBox.Text);

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

                CR.TriggerLists.Ignore = ignores;
                CR.TriggerLists.Rooms = rooms;
                CR.TriggerLists.User = users;
                DialogResult = true;
                Close();
            }
        }
    }
}
