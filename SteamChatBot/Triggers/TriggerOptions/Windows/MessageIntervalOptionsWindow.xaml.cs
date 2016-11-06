using SteamKit2;
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
    /// Interaction logic for MessageIntervalOptionsWindow.xaml
    /// </summary>
    public partial class MessageIntervalOptionsWindow : Window
    {
        public MessageIntervalOptions MIO { get; set; }
        
        public MessageIntervalOptionsWindow()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text == "" || destinationsBox.Text == "" || messageBox.Text == "")
            {
                MessageBox.Show("You must include a name, destinations, and a message.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            }
            else {
                MIO = new MessageIntervalOptions
                {
                    Name = nameBox.Text,
                    Interval = 1000 * 60 * 5,
                    Destinations = new List<SteamID>(),
                    Message = messageBox.Text
                };

                List<SteamID> destinations = new List<SteamID>();

                if (intervalBox.Text != "") MIO.Interval = Convert.ToInt32(intervalBox.Text);
                if (destinationsBox.Text.Split(',').Length > 0)
                {
                    foreach (string destination in destinationsBox.Text.Split(','))
                    {
                        destinations.Add(new SteamID(Convert.ToUInt64(destination)));
                    }
                }

                MIO.Destinations = destinations;
                DialogResult = true;
                Close();
            }
        }
    }
}
