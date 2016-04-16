﻿using System;
using System.Linq;
using System.Text;
using System.Xaml;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;

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

using SteamChatBot;
using SteamChatBot.Triggers;
using SteamKit2;

namespace SteamChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisableAll();
        }

        Log Log;
        string logFile;
        string sentryFile;
        string username;
        string password;
        string displayName;
        string fll;
        string cll;
        TriggerType selectedElement;

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
            if (File.Exists("login.json") && usernameBox.Text == "" && passwordBox.Password == "" && sentryFileTextBox.Text == "" &&
                logFileTextBox.Text == "" && displaynameBox.Text == "" && consoleLLBox.SelectedValue == null && fileLLBox.SelectedValue == null)
            {
                var _data = Bot.ReadData();
                logFile = _data.logFile;
                sentryFile = _data.sentryFile;
                username = _data.username;
                password = _data.password;
                displayName = _data.displayName;
                cll = _data.cll;
                fll = _data.fll;

                LoggerWindow logWindow = new LoggerWindow();
                Bot.logWindow = logWindow;
                Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true),
                    (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true), logWindow);

                Log.Instance.Silly("Successfully read login data from file");
                AddTriggersToList();
                logWindow.Show();
                Close();
                Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
            }
            else
            {

                if (usernameBox.Text != "" && passwordBox.Password != "")
                {
                    object cll = ((ListBoxItem)consoleLLBox.SelectedValue).Content;
                    object fll = ((ListBoxItem)fileLLBox.SelectedValue).Content;
                    LoggerWindow logWindow = new LoggerWindow();
                    Bot.logWindow = logWindow;

                    Log = Log.CreateInstance((logFileTextBox.Text == "" ? usernameBox.Text + ".log" : logFileTextBox.Text), usernameBox.Text,
                        (cll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) :
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll.ToString(), true)),
                        (fll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) :
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll.ToString(), true)), logWindow);

                    Log.Instance.Silly("Console started successfully!");
                    if (displaynameBox.Text != "")
                    {
                        AddTriggersToList();
                        logWindow.Show();
                        Close();
                        Bot.Start(usernameBox.Text, passwordBox.Password, (cll == null ? "Silly" :
                            cll.ToString()), (fll == null ? "Silly" :
                            fll.ToString()), (logFile == null ? usernameBox.Text + ".log" : logFile),
                            displaynameBox.Text, (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile));

                    }
                    else
                    {
                        MessageBox.Show("Missing Display Name!", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Missing either username or password.", "Error");
                }
            }
        }

        private void AddTriggersToList()
        {
            foreach (CheckBox box in triggerCheckBoxList.Items)
            {
                Bot.checkBoxes.Add(box);
            }
        }

        #region dynamic trigger options

        private void DisableAll()
        {
            commandBox.IsEnabled = false;
            commandLabel.IsEnabled = false;
            commandDoneButton.IsEnabled = false;

            matchesLabel.IsEnabled = false;
            matchesBox.IsEnabled = false;
            matchesDoneButton.IsEnabled = false;

            responsesLabel.IsEnabled = false;
            responsesBox.IsEnabled = false;
            responsesDoneButton.IsEnabled = false;

            timeoutLabel.IsEnabled = false;
            timeoutBox.IsEnabled = false;
            timeoutDoneButton.IsEnabled = false;

            delayLabel.IsEnabled = false;
            delayBox.IsEnabled = false;
            delayDoneButton.IsEnabled = false;

            probLabel.IsEnabled = false;
            probBox.IsEnabled = false;
            probDoneButton.IsEnabled = false;

            roomsLabel.IsEnabled = false;
            roomsBox.IsEnabled = false;
            roomsDoneButton.IsEnabled = false;

            usersLabel.IsEnabled = false;
            usersBox.IsEnabled = false;
            usersDoneButton.IsEnabled = false;

            ignoresLabel.IsEnabled = false;
            ignoresBox.IsEnabled = false;
            ignoresDoneButton.IsEnabled = false;

            apiLabel.IsEnabled = false;
            apiBox.IsEnabled = false;
            apiDoneButton.IsEnabled = false;
        }

        private void isUpTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();
            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;


            selectedElement = TriggerType.IsUpTrigger;

        }

        private void chatReplyTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            matchesLabel.IsEnabled = true;
            matchesBox.IsEnabled = true;
            matchesDoneButton.IsEnabled = true;

            responsesLabel.IsEnabled = true;
            responsesBox.IsEnabled = true;
            responsesDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.ChatReplyTrigger;
        }

        private void acceptFriendRequestTrigger_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.AcceptFriendRequestTrigger;
        }

        private void autojoinChatTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            selectedElement = TriggerType.AutojoinChatTrigger;
        }

        private void kickTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;
            selectedElement = TriggerType.KickTrigger;
        }

        private void banTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.BanTrigger;
        }

        private void acceptChatInviteTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.AcceptChatInviteTrigger;
        }

        private void leaveChatTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.LeaveChatTrigger;
        }

        private void linkNameTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.LinkNameTrigger;
        }

        private void doormatTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.DoormatTrigger;
        }

        private void lockChatTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.LockChatTrigger;
        }

        private void unlockChatTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.UnlockChatTrigger;
        }

        private void unmoderateTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.UnmoderateChatTrigger;
        }

        private void moderateTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.ModerateChatTrigger;
        }

        private void unbanTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            selectedElement = TriggerType.UnbanTrigger;

        }

        private void banCheckTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            apiLabel.IsEnabled = true;
            apiBox.IsEnabled = true;
            apiDoneButton.IsEnabled = true;

            selectedElement = TriggerType.BanCheckTrigger;
        }

        private void weatherTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DisableAll();

            commandBox.IsEnabled = true;
            commandLabel.IsEnabled = true;
            commandDoneButton.IsEnabled = true;

            timeoutLabel.IsEnabled = true;
            timeoutBox.IsEnabled = true;
            timeoutDoneButton.IsEnabled = true;

            delayLabel.IsEnabled = true;
            delayBox.IsEnabled = true;
            delayDoneButton.IsEnabled = true;

            probLabel.IsEnabled = true;
            probBox.IsEnabled = true;
            probDoneButton.IsEnabled = true;

            roomsLabel.IsEnabled = true;
            roomsBox.IsEnabled = true;
            roomsDoneButton.IsEnabled = true;

            usersLabel.IsEnabled = true;
            usersBox.IsEnabled = true;
            usersDoneButton.IsEnabled = true;

            ignoresLabel.IsEnabled = true;
            ignoresBox.IsEnabled = true;
            ignoresDoneButton.IsEnabled = true;

            apiLabel.IsEnabled = true;
            apiBox.IsEnabled = true;
            apiDoneButton.IsEnabled = true;

            selectedElement = TriggerType.WeatherTrigger;
        }

        #endregion

        #region button label box after-button click

        private void commandDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.commandList.ContainsKey(TriggerType.IsUpTrigger))
            {
                MessageBox.Show("You already have a command for this trigger type.", "Error");
                commandBox.IsEnabled = false;
                commandLabel.IsEnabled = false;
                commandDoneButton.IsEnabled = false;
                commandBox.Text = "";
            }
            else
            {
                Bot.commandList.Add(selectedElement, commandBox.Text);
                MessageBox.Show("Trigger command added successfully.", "Success");
                commandBox.IsEnabled = false;
                commandLabel.IsEnabled = false;
                commandDoneButton.IsEnabled = false;
                commandBox.Text = "";
            }
        }

        private void roomsDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.rooms.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have a list of rooms for this trigger type.", "Error");
                roomsLabel.IsEnabled = false;
                roomsBox.IsEnabled = false;
                roomsDoneButton.IsEnabled = false;
                roomsBox.Text = "";
            }
            else
            {
                if (roomsBox.Text != "")
                {
                    string[] rooms = roomsBox.Text.Split(',');
                    List<SteamID> _rooms = new List<SteamID>();
                    foreach (string room in rooms)
                    {
                        _rooms.Add(new SteamID(Convert.ToUInt64(room)));
                    }
                    Bot.rooms.Add(selectedElement, _rooms);

                    MessageBox.Show("Trigger rooms added successfully.", "Success");
                }
                else
                {
                    MessageBox.Show("You did not enter a room ID.", "Error");
                }
                roomsLabel.IsEnabled = false;
                roomsBox.IsEnabled = false;
                roomsDoneButton.IsEnabled = false;
                roomsBox.Text = "";
            }
        }

        private void usersDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.users.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have a list of users for this trigger type.", "Error");
                usersLabel.IsEnabled = false;
                usersBox.IsEnabled = false;
                usersDoneButton.IsEnabled = false;
                usersBox.Text = "";
            }
            else
            {
                string[] users = usersBox.Text.Split(',');
                List<SteamID> _users = new List<SteamID>();
                foreach (string user in users)
                {
                    _users.Add(new SteamID(Convert.ToUInt64(user)));
                }
                Bot.users.Add(selectedElement, _users);

                MessageBox.Show("Trigger users added successfully.", "Success");
                usersLabel.IsEnabled = false;
                usersBox.IsEnabled = false;
                usersDoneButton.IsEnabled = false;
                usersBox.Text = "";
            }
        }

        private void ignoresDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.ignores.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have a list of ignores for this trigger type.", "Error");
                ignoresLabel.IsEnabled = false;
                ignoresBox.IsEnabled = false;
                ignoresDoneButton.IsEnabled = false;
                ignoresBox.Text = "";
            }
            else
            {
                string[] ignores = ignoresBox.Text.Split(',');
                List<SteamID> _ignores = new List<SteamID>();
                foreach (string ignore in ignores)
                {
                    _ignores.Add(new SteamID(Convert.ToUInt64(ignore)));
                }
                Bot.ignores.Add(selectedElement, _ignores);

                MessageBox.Show("Trigger ignores added successfully.", "Success");
                ignoresLabel.IsEnabled = false;
                ignoresBox.IsEnabled = false;
                ignoresDoneButton.IsEnabled = false;
                ignoresBox.Text = "";
            }
        }

        private void apiDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.apiKeys.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have an API key for this trigger type.", "Error");
                apiLabel.IsEnabled = false;
                apiBox.IsEnabled = false;
                apiDoneButton.IsEnabled = false;
                apiBox.Text = "";
            }
            else
            {
                Bot.apiKeys.Add(selectedElement, apiBox.Text);
                MessageBox.Show("Trigger API key added successfully.", "Success");
                apiLabel.IsEnabled = false;
                apiBox.IsEnabled = false;
                apiDoneButton.IsEnabled = false;
                apiBox.Text = "";
            }
        }

        private void matchesDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.matchesList.ContainsKey(TriggerType.ChatReplyTrigger))
            {
                MessageBox.Show("You already have matches for this trigger type.", "Error");
                matchesLabel.IsEnabled = false;
                matchesBox.IsEnabled = false;
                matchesDoneButton.IsEnabled = false;
                matchesBox.Text = "";
            }
            else
            {
                string[] matches = matchesBox.Text.Split(',');
                List<string> _matches = new List<string>();
                foreach (string match in matches)
                {
                    _matches.Add(match);
                }
                Bot.matchesList.Add(TriggerType.ChatReplyTrigger, _matches);
                MessageBox.Show("Trigger matches added successfully", "Success");
                matchesLabel.IsEnabled = false;
                matchesBox.IsEnabled = false;
                matchesDoneButton.IsEnabled = false;
                matchesBox.Text = "";
            }
        }

        private void responsesDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.responsesList.ContainsKey(TriggerType.ChatReplyTrigger))
            {
                MessageBox.Show("You already have responses for this trigger type.", "Error");
                responsesLabel.IsEnabled = false;
                responsesBox.IsEnabled = false;
                responsesDoneButton.IsEnabled = false;
                responsesBox.Text = "";
            }
            else
            {
                string[] responses = responsesBox.Text.Split(',');
                List<string> _responses = new List<string>();
                foreach (string response in responses)
                {
                    _responses.Add(response);
                }
                Bot.responsesList.Add(TriggerType.ChatReplyTrigger, _responses);
                MessageBox.Show("Trigger responses added successfully", "Success");
                responsesLabel.IsEnabled = false;
                responsesBox.IsEnabled = false;
                responsesDoneButton.IsEnabled = false;
                responsesBox.Text = "";
            }
        }

        private void delayDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.delays.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have a delay for this trigger type.", "Error");
                delayLabel.IsEnabled = false;
                delayBox.IsEnabled = false;
                delayDoneButton.IsEnabled = false;
                delayBox.Text = "";
            }
            else
            {
                Bot.delays.Add(selectedElement, Convert.ToInt32(delayBox.Text));
                MessageBox.Show("Trigger delay added successfully.", "Success");
                delayLabel.IsEnabled = false;
                delayBox.IsEnabled = false;
                delayDoneButton.IsEnabled = false;
                delayBox.Text = "";
            }
        }

        private void timeoutDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.timeouts.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have a timeout for this trigger type.", "Error");
                timeoutLabel.IsEnabled = true;
                timeoutBox.IsEnabled = true;
                timeoutDoneButton.IsEnabled = true;
                timeoutBox.Text = "";
            }
            else
            {
                Bot.timeouts.Add(selectedElement, Convert.ToInt32(timeoutBox.Text));
                MessageBox.Show("Trigger timeout added successfully.", "Success");
                timeoutLabel.IsEnabled = true;
                timeoutBox.IsEnabled = true;
                timeoutDoneButton.IsEnabled = true;
                timeoutBox.Text = "";
            }
        }


        private void probDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.probs.ContainsKey(selectedElement))
            {
                MessageBox.Show("You already have a probability for this trigger type.", "Error");
                probLabel.IsEnabled = false;
                probBox.IsEnabled = false;
                probDoneButton.IsEnabled = false;
                probBox.Text = "";
            }
            else
            {
                if (probBox.Text != "")
                {
                    Bot.probs.Add(selectedElement, Convert.ToInt32(probBox.Text));
                    MessageBox.Show("Trigger probability added successfully.", "Success");
                    probLabel.IsEnabled = false;
                    probBox.IsEnabled = false;
                    probDoneButton.IsEnabled = false;
                    probBox.Text = "";
                }
            }
        }

        #endregion

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
        }
    }
}