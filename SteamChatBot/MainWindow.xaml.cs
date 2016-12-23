using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xaml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using SteamKit2;
using SteamChatBot.Triggers;
using SteamChatBot.Triggers.TriggerOptions;
using SteamChatBot.Triggers.TriggerOptions.Windows;

namespace SteamChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Log Log;
        string logFile;
        string sentryFile;
        string username;
        string password;
        string displayName;
        string fll;
        string cll;
        string chatbots;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream("chatbots.txt", FileMode.Open))
                {
                    using (TextReader tr = new StreamReader(fs))
                    {
                        chatbots = tr.ReadToEnd();
                    }
                }
                if (chatbots.Length > 0)
                {
                    chatbotsBox.ItemsSource = chatbots.Split('\n');
                }
            }
            catch (FileNotFoundException err) { }
        }

        #region file browse dialogs

        private void sentryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                filename = ofd.FileName;
                sentryFileTextBox.Text = filename;
                sentryFile = filename;
            }
        }

        private void logFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                filename = ofd.FileName;
                logFileTextBox.Text = filename;
                logFile = filename;
            }
        }

        #endregion

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text == "" && chatbotsBox.SelectedIndex != -1 && chatbotsBox.SelectedValue.ToString() != "")
            {
                username = chatbotsBox.SelectedValue.ToString();
                if (File.Exists(username + "/login.json") && passwordBox.Password == "" && sentryFileTextBox.Text == "" &&
                    logFileTextBox.Text == "" && displaynameBox.Text == "" && consoleLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose" &&
                    fileLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose")
                {
                    var _data = Bot.ReadData(username);
                    logFile = _data.logFile;
                    sentryFile = _data.sentryFile;
                    username = _data.username;
                    password = _data.password;
                    displayName = _data.displayName;
                    cll = _data.cll;
                    fll = _data.fll;

                    if (sharedSecretBox.Text != "")
                    {
                        Bot.sharedSecret = sharedSecretBox.Text;
                    }

                    Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true),
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true));

                    Log.Instance.Silly("Successfully read login data from file");
                    Close();
                    Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
                }
            }
            else if (usernameBox.Text != "")
            {
                username = usernameBox.Text;
                if (File.Exists(username + "/login.json") && passwordBox.Password == "" && sentryFileTextBox.Text == "" &&
                    logFileTextBox.Text == "" && displaynameBox.Text == "" && consoleLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose" &&
                    fileLLBox.SelectedItem.ToString() == "System.Windows.Controls.ListBoxItem: Verbose")
                {
                    var _data = Bot.ReadData(username);
                    logFile = _data.logFile;
                    sentryFile = _data.sentryFile;
                    username = _data.username;
                    password = _data.password;
                    displayName = _data.displayName;
                    cll = _data.cll;
                    fll = _data.fll;

                    if (sharedSecretBox.Text != "")
                    {
                        Bot.sharedSecret = sharedSecretBox.Text;
                    }

                    Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true),
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true));

                    Log.Instance.Silly("Successfully read login data from file");
                    Close();
                    Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
                }
                else
                {
                    if (passwordBox.Password != "")
                    {
                        object cll = ((ListBoxItem)consoleLLBox.SelectedValue).Content;
                        object fll = ((ListBoxItem)fileLLBox.SelectedValue).Content;

                        Log = Log.CreateInstance((logFileTextBox.Text == "" ? usernameBox.Text + ".log" : logFileTextBox.Text), usernameBox.Text,
                            (cll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Verbose", true) :
                            (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll.ToString(), true)),
                            (fll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Verbose", true) :
                            (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll.ToString(), true)));

                        Log.Instance.Silly("Console started successfully!");
                        if (displaynameBox.Text != "")
                        {
                            Close();
                            if (sharedSecretBox.Text != "")
                            {
                                Bot.sharedSecret = sharedSecretBox.Text;
                            }
                            Bot.Start(usernameBox.Text, passwordBox.Password, (cll == null ? "Verbose" :
                                cll.ToString()), (fll == null ? "Verbose" :
                                fll.ToString()), (logFile == null ? usernameBox.Text + ".log" : logFile),
                                displaynameBox.Text, (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile));

                        }
                        else
                        {
                            MessageBox.Show("Missing Display Name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Missing password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Missing username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Steam-Chat-Bot/SteamChatBot/issues");
        }

        private void plusTriggerButton_Click(object sender, RoutedEventArgs e)
        {
            string selected = "";
            TriggerType type;
            try
            {
                selected = ((ListBoxItem)triggerListBox.SelectedValue).Name;
            }
            catch (Exception err) { return; }


            ChatCommand cc = new ChatCommand();
            ChatCommandApi cca = new ChatCommandApi();
            ChatReply cr = new ChatReply();
            NoCommand nc = new NoCommand();
            TriggerLists tl = new TriggerLists();
            TriggerNumbers tn = new TriggerNumbers();
            AntiSpamTriggerOptions asto = new AntiSpamTriggerOptions();
            DiscordOptions _do = new DiscordOptions(); // "do" is a keyword
            NoteTriggerOptions nto = new NoteTriggerOptions();
            NotificationOptions no = new NotificationOptions();
            MessageIntervalOptions mio = new MessageIntervalOptions();

            TriggerOptionsBase tob = new TriggerOptionsBase();

            if (selected == "isUpTrigger" || selected == "leaveChatTrigger" || selected == "kickTrigger"
                || selected == "banTrigger" || selected == "unbanTrigger" || selected == "lockTrigger"
                || selected == "unlockTrigger" || selected == "moderateTrigger" || selected == "unmoderateTrigger"
                || selected == "playGameTrigger" || selected == "changeNameTrigger" || selected == "googleTrigger"
                || selected == "chooseTrigger" || selected == "translateTrigger")
            {
                ChatCommandWindow ccw = new ChatCommandWindow();
                ccw.ShowDialog();
                if (ccw.DialogResult.HasValue && ccw.DialogResult.Value)
                {
                    cc = ccw.CC;

                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", cc.Name, type.ToString()));

                    tob.ChatCommand = cc;
                    tob.Name = cc.Name;
                    tob.Type = type;
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, cc.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "chatReplyTrigger")
            {
                ChatReplyWindow crw = new ChatReplyWindow();
                crw.ShowDialog();
                if (crw.DialogResult.HasValue && crw.DialogResult.Value)
                {
                    cr = crw.CR;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.ChatReply = cr;
                    tob.Name = cr.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", cr.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, cr.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "linkNameTrigger" || selected == "doormatTrigger")
            {
                NoCommandWindow ncw = new NoCommandWindow();
                ncw.ShowDialog();
                if (ncw.DialogResult.HasValue && ncw.DialogResult.Value)
                {
                    nc = ncw.NC;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.NoCommand = nc;
                    tob.Name = nc.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", nc.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, nc.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "banCheckTrigger" || selected == "weatherTrigger" || selected == "youtubeTrigger")
            {
                ChatCommandApiWindow ccaw = new ChatCommandApiWindow();
                switch (selected)
                {
                    case "banCheckTrigger":
                        ccaw.apiBlock.PreviewMouseDown += (sender1, e1) => Ccaw_PreviewMouseDown_Steam(sender1, e1, ccaw);
                        break;
                    case "weatherTrigger":
                        ccaw.apiBlock.PreviewMouseDown += (sender1, e1) => Ccaw_PreviewMouseDown_Wunderground(sender1, e1, ccaw);
                        break;
                    case "youtubeTrigger":
                        MessageBox.Show("You MUST name your project \"Steam Chat Bot\" or else this trigger will not work!", "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        ccaw.apiBlock.PreviewMouseDown += (sender1, e1) => Ccaw_PreviewMouseDown1_Google(sender1, e1, ccaw);
                        break;
                }
                ccaw.ShowDialog();
                if (ccaw.DialogResult.HasValue && ccaw.DialogResult.Value)
                {
                    cca = ccaw.CCA;
                    cc = ccaw.CC;

                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.ChatCommandApi = cca;
                    tob.ChatCommandApi.ChatCommand = cc;
                    tob.Name = cca.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", cc.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, cc.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "acceptFriendRequestTrigger" || selected == "autojoinChatTrigger" || selected == "acceptChatInviteTrigger")
            {
                TriggerListWindow tlw = new TriggerListWindow(selected);
                tlw.ShowDialog();
                if(tlw.DialogResult.HasValue && tlw.DialogResult.Value)
                {
                    tl = tlw.TL;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.TriggerLists = tl;
                    tob.Name = tl.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", tl.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, tl.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if(selected == "antispamTrigger")
            {
                AntiSpamTriggerOptionsWindow astow = new AntiSpamTriggerOptionsWindow();
                astow.ShowDialog();
                if(astow.DialogResult.HasValue && astow.DialogResult.Value)
                {
                    asto = astow.ASTO;
                    nc = astow.NC;

                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.AntiSpamTriggerOptions = asto;
                    tob.AntiSpamTriggerOptions.NoCommand = nc;
                    tob.Name = asto.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", asto.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, asto.Name, tob);
                    Bot.triggers.Add(trigger);

                }
            }
            else if(selected == "discordTrigger")
            {
                DiscordTriggerOptionsWindow dtow = new DiscordTriggerOptionsWindow();
                dtow.ShowDialog();
                if(dtow.DialogResult.HasValue && dtow.DialogResult.Value)
                {
                    _do = dtow.DO;
                    nc = dtow.NC;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.DiscordOptions = _do;
                    tob.DiscordOptions.NoCommand = nc;
                    tob.Name = _do.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", _do.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, _do.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if(selected == "noteTrigger")
            {
                NoteTriggerOptionsWindow ntow = new NoteTriggerOptionsWindow();
                ntow.ShowDialog();
                if(ntow.DialogResult.HasValue && ntow.DialogResult == true)
                {
                    nc = ntow.NC;
                    nto = ntow.NTO;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.NoteTriggerOptions = nto;
                    tob.NoteTriggerOptions.NoCommand = nc;
                    tob.Name = nto.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", nto.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, nto.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if(selected == "notificationTrigger")
            {
                NotificationOptionsWindow now = new NotificationOptionsWindow();
                now.ShowDialog();
                if(now.DialogResult.HasValue && now.DialogResult == true)
                {
                    nc = now.NC;
                    no = now.NO;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.NotificationOptions = no;
                    tob.NotificationOptions.NoCommand = nc;
                    tob.Name = Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", no.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, no.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if(selected == "messageIntervalTrigger")
            {
                MessageIntervalOptionsWindow miow = new MessageIntervalOptionsWindow();
                miow.ShowDialog();
                if(miow.DialogResult.HasValue && miow.DialogResult == true)
                {
                    mio = miow.MIO;
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.MessageIntervalOptions = mio;
                    tob.Name = Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", mio.Name, type));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, mio.Name, tob);
                    Bot.triggers.Add(trigger);

                }
            }
            else
            {
                MessageBox.Show("Unknown Trigger. Please contact the developer.", "Error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void Ccaw_PreviewMouseDown1_Google(object sender, MouseButtonEventArgs e, ChatCommandApiWindow ccaw)
        {
            Process.Start("https://developers.google.com/youtube/v3/");
        }

        private void Ccaw_PreviewMouseDown_Wunderground(object sender, MouseButtonEventArgs e, ChatCommandApiWindow ccaw)
        {
            Process.Start("https://www.wunderground.com/weather/api/");
        }

        private void Ccaw_PreviewMouseDown_Steam(object sender, MouseButtonEventArgs e, ChatCommandApiWindow ccaw)
        {
            Process.Start("http://steamcommunity.com/dev/");
        }

        private void minusTriggerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedString = ((string)addedTriggersListBox.SelectedValue);
                addedTriggersListBox.Items.Remove(addedTriggersListBox.SelectedValue);
                IEnumerable<BaseTrigger> triggers = Bot.triggers.Where(x => x.Name == selectedString.Substring(0, selectedString.IndexOf(" -")));
                for (int i = 0; i < triggers.Count(); i++)
                {
                    Bot.triggers.Remove(triggers.ElementAt(i));
                }
            }
            catch (Exception err) { throw err; }
        }
    }
}