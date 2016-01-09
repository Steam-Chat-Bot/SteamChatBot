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
        public TriggerOptionsWindow(ListBoxItem trigger)
        {
            InitializeComponent();
            string type = trigger.Content.ToString();
            TriggerType triggerType;
            Enum.TryParse(type, out triggerType);
            typeBox.Text = type;

        }
    }
}
