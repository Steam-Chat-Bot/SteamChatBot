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

using SteamChatBot.Triggers;

namespace SteamChatBot
{
    /// <summary>
    /// Interaction logic for TriggerOptions.xaml
    /// </summary>
    public partial class TriggerOptionsWindow : Window
    {
        public TriggerType Type { get; set; }
        public string Name { get; set; }


        public TriggerOptionsWindow(ListBoxItem trigger)
        {
            InitializeComponent();
            Type = (TriggerType)Enum.Parse(typeof(TriggerType), trigger.Content.ToString());
            typeBox.Text = Type.ToString();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Bot.triggers.Add(new BaseTrigger(Type, Name));
            Console.WriteLine(string.Format("Trigger {0}/{1} saved", nameBox.Text, Type));
            Close();
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(nameBox.Text == "")
            {
                saveButton.IsEnabled = false;
            }
            else
            {
                saveButton.IsEnabled = true;
            }
        }
    }
}
