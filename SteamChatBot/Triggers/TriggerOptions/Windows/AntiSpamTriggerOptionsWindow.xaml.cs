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
    /// Interaction logic for AntiSpamTriggerOptionsWindow.xaml
    /// </summary>
    public partial class AntiSpamTriggerOptionsWindow : Window
    {
        public NoCommand NC { get; set; }
        public AntiSpamTriggerOptions ASTO { get; set; }

        NoCommandWindow ncw = new NoCommandWindow();

        public AntiSpamTriggerOptionsWindow()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            ncw.ShowDialog();
            if (ncw.DialogResult == true)
            {
                NC = ncw.NC;
                ASTO = new AntiSpamTriggerOptions
                {
                    Name = NC.Name,
                    admins = new List<SteamID>(),
                    warnMessage = "Spamming is against the rules!",
                    msgPenalty = 1,
                    score = new Score
                    {
                        warn = 3,
                        warnMax = 5,
                        kick = 6,
                        ban = 8,
                        tattle = 4,
                        tattleMax = 5
                    },
                    timers = new Timers
                    {
                        messages = 5000,
                        unban = 1000 * 60 * 5
                    },
                    ptimer = new PTimer
                    {
                        resolution = 1000,
                        amount = 1
                    },
                    groups = new Dictionary<SteamID, Dictionary<SteamID, int>>(),
                    NoCommand = NC
                };

                List<SteamID> admins = new List<SteamID>();

                if (adminsBox.Text.Split(',').Length > 0 && adminsBox.Text != "")
                {
                    foreach (string admin in adminsBox.Text.Split(','))
                    {
                        admins.Add(new SteamID(Convert.ToUInt64(admin)));
                    }
                }

                if (warnMsgBox.Text != "") ASTO.warnMessage = warnMsgBox.Text;
                if (penaltyBox.Text != "") ASTO.msgPenalty = Convert.ToInt32(penaltyBox.Text);
                if (resolutionBox.Text != "") ASTO.ptimer.resolution = Convert.ToInt32(resolutionBox.Text);
                if (amountBox.Text != "") ASTO.ptimer.amount = Convert.ToInt32(amountBox.Text);
                if (unbanBox.Text != "") ASTO.timers.unban = Convert.ToInt32(unbanBox.Text);

                if (warn.Text != "") ASTO.score.warn = Convert.ToInt32(warn.Text);
                if (warnMax.Text != "") ASTO.score.warnMax = Convert.ToInt32(warnMax.Text);
                if (ban.Text != "") ASTO.score.ban = Convert.ToInt32(ban.Text);
                if (kick.Text != "") ASTO.score.kick = Convert.ToInt32(kick.Text);
                if (tattle.Text != "") ASTO.score.tattle = Convert.ToInt32(tattle.Text);
                if (tattleMax.Text != "") ASTO.score.tattleMax = Convert.ToInt32(tattleMax.Text);

                ASTO.admins = admins;
                DialogResult = true;
                Close();
            }
        }
    }
}
