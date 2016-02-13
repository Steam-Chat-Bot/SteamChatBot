using System;
using System.Linq;
using System.Text;
using System.Xaml;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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

        Log Log;
        string logFile;
        string sentryFile;
        string autoJoinFile;
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
                autoJoinFile = _data.autoJoinFile;
                username = _data.username;
                password = _data.password;
                displayName = _data.displayName;
                cll = _data.cll;
                fll = _data.fll;

                Log = Log.CreateInstance(logFile, username, (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll, true), (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll, true));

                Close();

                Log.Instance.Silly("Successfully read login data from file");
                AddTriggersToList();
                Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
            }
            else
            {

                if (usernameBox.Text != "" && passwordBox.Password != "")
                {
                    object cll = ((ListBoxItem)consoleLLBox.SelectedValue).Content;
                    object fll = ((ListBoxItem)fileLLBox.SelectedValue).Content;

                    Log = Log.CreateInstance((logFileTextBox.Text == "" ? usernameBox.Text + ".log" : logFileTextBox.Text), usernameBox.Text,
                        (cll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) :
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), cll.ToString(), true)),
                        (fll == null ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) :
                        (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fll.ToString(), true)));

                    Log.Instance.Silly("Console started successfully!");
                    if (passwordBox.Password != "" && displaynameBox.Text != "")
                    {
                        Close();
                        AddTriggersToList();
                        Bot.Start(usernameBox.Text, passwordBox.Password, (cll == null ? "Silly" :
                            cll.ToString()), (fll == null ? "Silly" :
                            fll.ToString()), (logFile == null ? usernameBox.Text + ".log" : logFile),
                            displaynameBox.Text, (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile));
                    }
                }
                else
                {
                    MessageBox.Show("Missing either username or password.");
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

        private void isUpTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            commandBox.Visibility = Visibility.Visible;
            commandLabel.Visibility = Visibility.Visible;
            commandDoneButton.Visibility = Visibility.Visible;
            selectedElement = TriggerType.IsUpTrigger;
        }

        private void commandDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.commandList.ContainsKey(TriggerType.IsUpTrigger))
            {
                MessageBox.Show("You already have a command for this trigger type.", "Error");
                commandBox.Visibility = Visibility.Hidden;
                commandLabel.Visibility = Visibility.Hidden;
                commandDoneButton.Visibility = Visibility.Hidden;
            }
            else
            {
                Bot.commandList.Add(selectedElement, commandBox.Text);
                MessageBox.Show("Trigger command added successfully.", "Success");
                commandBox.Visibility = Visibility.Hidden;
                commandLabel.Visibility = Visibility.Hidden;
                commandDoneButton.Visibility = Visibility.Hidden;
            }
        }

        private void chatReplyTriggerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            matchesLabel.Visibility = Visibility.Visible;
            responsesLabel.Visibility = Visibility.Visible;
            matchesBox.Visibility = Visibility.Visible;
            responsesBox.Visibility = Visibility.Visible;
            matchesDoneButton.Visibility = Visibility.Visible;
            responsesDoneButton.Visibility = Visibility.Visible;
            selectedElement = TriggerType.ChatReplyTrigger;
        }

        private void matchesDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if(Bot.matchesList.ContainsKey(TriggerType.ChatReplyTrigger))
            {
                MessageBox.Show("You already have matches for this trigger type.", "Error");
                matchesLabel.Visibility = Visibility.Hidden;
                matchesBox.Visibility = Visibility.Hidden;
                matchesDoneButton.Visibility = Visibility.Hidden;
            }
            else
            {
                string[] matches = matchesBox.Text.Split(',');
                List<string> _matches = new List<string>();
                foreach(string match in matches)
                {
                    _matches.Add(match);
                }
                Bot.matchesList.Add(TriggerType.ChatReplyTrigger, _matches);
                MessageBox.Show("Trigger matches added successfully", "Success");
                matchesLabel.Visibility = Visibility.Hidden;
                matchesBox.Visibility = Visibility.Hidden;
                matchesDoneButton.Visibility = Visibility.Hidden;
            }
        }

        private void responsesDoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Bot.responsesList.ContainsKey(TriggerType.ChatReplyTrigger))
            {
                MessageBox.Show("You already have responses for this trigger type.", "Error");
                responsesLabel.Visibility = Visibility.Hidden;
                responsesBox.Visibility = Visibility.Hidden;
                responsesDoneButton.Visibility = Visibility.Hidden;
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
                responsesLabel.Visibility = Visibility.Hidden;
                responsesBox.Visibility = Visibility.Hidden;
                responsesDoneButton.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        /*
        #region trigger drag-drop

        private ListBoxItem _dragged;
        
        private void inactiveTriggers_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_dragged != null)
            {
                return;
            }

            UIElement element = inactiveTriggers.InputHitTest(e.GetPosition(inactiveTriggers)) as UIElement;

            while(element != null)
            {
                if(element is ListBoxItem)
                {
                    _dragged = (ListBoxItem)element;
                    break;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
        }

        private void activeTriggers_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_dragged != null)
            {
                return;
            }

            UIElement element = activeTriggers.InputHitTest(e.GetPosition(activeTriggers)) as UIElement;

            while (element != null)
            {
                if (element is ListBoxItem)
                {
                    _dragged = (ListBoxItem)element;
                    break;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if(_dragged == null)
            {
                return;
            }
            if(e.LeftButton == MouseButtonState.Released)
            {
                _dragged = null;
                return;
            }

            DataObject obj = new DataObject(DataFormats.Text, _dragged.ToString());
            DragDrop.DoDragDrop(_dragged, obj, DragDropEffects.Move);
        }

        private void activeTriggers_DragEnter(object sender, DragEventArgs e)
        {
            if(_dragged == null || e.Data.GetDataPresent(DataFormats.Text, true) == false)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.All;
            }
        }

        private void activeTriggers_Drop(object sender, DragEventArgs e)
        {
            inactiveTriggers.Items.Remove(_dragged);
            try
            {
                activeTriggers.Items.Add(_dragged);
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException("You cannot drag a trigger to its own box.");
            }
                _dragged = null;
        }

        private void inactiveTriggers_Drop(object sender, DragEventArgs e)
        {
            activeTriggers.Items.Remove(_dragged);
            try
            {
                inactiveTriggers.Items.Add(_dragged);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("You cannot drag a trigger to its own box.");
            }
            _dragged = null;
        }

        private void inactiveTriggers_DragEnter(object sender, DragEventArgs e)
        {
            if (_dragged == null || e.Data.GetDataPresent(DataFormats.Text, true) == false)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.All;
            }
        }

        #endregion
        */
    }
}