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

            TriggerOptionsBase tob = new TriggerOptionsBase();

            if (selected == "isUpTrigger" || selected == "leaveChatTrigger" || selected == "kickTrigger"
                || selected == "banTrigger" || selected == "unbanTrigger" || selected == "lockTrigger"
                || selected == "unlockTrigger" || selected == "moderateTrigger" || selected == "unmoderateTrigger"
                || selected == "playGameTrigger")
            {
                ChatCommandWindow ccw = new ChatCommandWindow();
                ccw.ShowDialog();
                if (ccw.DialogResult.HasValue && ccw.DialogResult.Value)
                {
                    ChatCommand ccw_cc = ccw.CC;
                    cc = GetChatCommandOptions(ccw_cc);

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
                    ChatReply crw_cr = crw.CR;
                    cr = GetChatReplyOptions(crw_cr);
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
                    NoCommand ncw_nc = ncw.NC;
                    nc = GetNoCommandOptions(ncw_nc);
                    type = (TriggerType)Enum.Parse(typeof(TriggerType), char.ToUpper(selected[0]) + selected.Substring(1));

                    tob.NoCommand = nc;
                    tob.Name = nc.Name;
                    tob.Type = type;
                    addedTriggersListBox.Items.Add(string.Format("{0} - {1}", nc.Name, type.ToString()));
                    BaseTrigger trigger = (BaseTrigger)Activator.CreateInstance(Type.GetType("SteamChatBot.Triggers." + type.ToString()), type, nc.Name, tob);
                    Bot.triggers.Add(trigger);
                }
            }
            else if (selected == "banCheckTrigger" || selected == "weatherTrigger")
            {
                ChatCommandApiWindow ccaw = new ChatCommandApiWindow();
                ccaw.ShowDialog();
                if (ccaw.DialogResult.HasValue && ccaw.DialogResult.Value)
                {
                    ChatCommandApi ccaw_cca = ccaw.CCA;
                    ChatCommand ccaw_cc = ccaw.CC;

                    cca = GetChatCommandApiOptions(ccaw_cca);
                    cc = GetChatCommandOptions(ccaw_cc);

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
                    TriggerLists tlw_tl = tlw.TL;
                    tl = GetTriggerListOptions(tlw_tl);
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
                    AntiSpamTriggerOptions astow_asto = astow.ASTO;
                    NoCommand astow_nc = astow.NC;
                    asto = GetAntispamTriggerOptions(astow_asto);
                    nc = GetNoCommandOptions(astow_nc);

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
                    DiscordOptions dtow_do = dtow.DO;
                    NoCommand dtow_nc = dtow.NC;
                    _do = GetDiscordOptions(dtow_do);
                    nc = GetNoCommandOptions(dtow_nc);
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
                    NoteTriggerOptions ntow_nto = ntow.NTO;
                    NoCommand ntow_nc = ntow.NC;
                    nc = GetNoCommandOptions(ntow_nc);
                    nto = GetNoteTriggerOptions(ntow_nto);
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
                    NotificationOptions now_no = now.NO;
                    NoCommand now_nc = now.NC;
                    nc = GetNoCommandOptions(now_nc);
                    no = GetNotificationOptions(now_no);
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
            else
            {
                MessageBox.Show("Unknown Trigger. Please contact the developer.", "Error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
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

        #region option helper methods

        private ChatCommand GetChatCommandOptions(ChatCommand c)
        {
            try
            {
                return new ChatCommand
                {
                    Name = c.Name,
                    Command = c.Command,
                    TriggerLists = c.TriggerLists,
                    TriggerNumbers = c.TriggerNumbers
                };
            }
            catch (Exception e) { return null; }
        }

        private ChatReply GetChatReplyOptions(ChatReply c)
        {
            try
            {
                return new ChatReply
                {
                    Name = c.Name,
                    Matches = c.Matches,
                    Responses = c.Responses,
                    TriggerLists = c.TriggerLists,
                    TriggerNumbers = c.TriggerNumbers
                };
            }
            catch (Exception e) { return null; }
        }

        private NoCommand GetNoCommandOptions(NoCommand n)
        {
            try
            {
                return new NoCommand
                {
                    Name = n.Name,
                    TriggerLists = n.TriggerLists,
                    TriggerNumbers = n.TriggerNumbers
                };
            }
            catch (Exception e) { return null; }
        }

        private ChatCommandApi GetChatCommandApiOptions(ChatCommandApi c)
        {
            try
            {
                return new ChatCommandApi
                {
                    ApiKey = c.ApiKey,
                    Name = c.Name,
                    ChatCommand = c.ChatCommand
                };
            }
            catch (Exception e) { return null; }
        }

        private TriggerLists GetTriggerListOptions(TriggerLists tl)
        {
            try
            {
                return new TriggerLists
                {
                    Name = tl.Name,
                    Rooms = tl.Rooms,
                    User = tl.User,
                    Ignore = tl.Ignore
                };
            }
            catch (Exception e) { return null; }
        }

        private AntiSpamTriggerOptions GetAntispamTriggerOptions(AntiSpamTriggerOptions asto)
        {
            try
            {
                return new AntiSpamTriggerOptions
                {
                    Name = asto.Name,
                    admins = asto.admins,
                    NoCommand = asto.NoCommand,
                    msgPenalty = asto.msgPenalty,
                    ptimer = asto.ptimer,
                    timers = asto.timers,
                    score = asto.score,
                    warnMessage = asto.warnMessage
                };
            }
            catch (Exception e) { return null; }
        }

        private DiscordOptions GetDiscordOptions(DiscordOptions _do)
        {
            try
            {
                return new DiscordOptions
                {
                    Name = _do.Name,
                    NoCommand = _do.NoCommand,
                    Token = _do.Token,
                    DiscordServerID = _do.DiscordServerID,
                    SteamChat = _do.SteamChat
                };
            }
            catch (Exception e) { return null; }
        }

        private NoteTriggerOptions GetNoteTriggerOptions(NoteTriggerOptions nto)
        {
            try
            {
                return new NoteTriggerOptions
                {
                    Name = nto.Name,
                    NoCommand = nto.NoCommand,
                    DeleteCommand = nto.DeleteCommand,
                    InfoCommand = nto.InfoCommand,
                    NoteCommand = nto.NoteCommand,
                    SaveTimer = nto.SaveTimer,
                    NoteFile = nto.NoteFile,
                    NotesCommand = nto.NotesCommand
                };
            }
            catch (Exception e) { return null; }
        }

        private NotificationOptions GetNotificationOptions(NotificationOptions no)
        {
            try
            {
                return new NotificationOptions
                {
                    Name = no.Name,
                    NoCommand = no.NoCommand,
                    APICommand = no.APICommand,
                    ClearCommand = no.ClearCommand,
                    DBFile = no.DBFile,
                    FilterCommand = no.FilterCommand,
                    SeenCommand = no.SeenCommand,
                    SaveTimer = no.SaveTimer
                };
            }
            catch (Exception e) { return null; }
        }

        #endregion
    }
}