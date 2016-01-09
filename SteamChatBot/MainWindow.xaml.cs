using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

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

using SteamChatBot.Triggers;
using SteamChatBot;
using System.Collections;

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
        List<TriggerType> triggers = new List<TriggerType>();

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

            if(ofd.ShowDialog() == true)
            {
                filename = ofd.FileName;
                logFileTextBox.Text = filename;
                logFile = filename;
            }
        }

        #endregion

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (activeTriggers.Items.Count > 0)
            {
                foreach (ListBoxItem trigger in activeTriggers.Items)
                {
                    triggers.Add((TriggerType)Enum.Parse(typeof(TriggerType), trigger.Content.ToString()));
                }
            }

            if(File.Exists("login.json") && usernameBox.Text == "" && passwordBox.Password == "" && sentryFileTextBox.Text == "" && 
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

                Log.Instance.Silly("Successfully read login data from file");
                Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile, triggers);
                Close();
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
                        Bot.Start(usernameBox.Text, passwordBox.Password, (cll == null ? "Silly" : 
                            cll.ToString()), (fll == null ? "Silly" : 
                            fll.ToString()), (logFile == null ? usernameBox.Text + ".log" : logFile), 
                            displaynameBox.Text, (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile), triggers);
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Missing either username or password.");
                }
            }
        }

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

        private void triggerOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            TriggerOptionsWindow tow = new TriggerOptionsWindow((ListBoxItem)activeTriggers.Items[activeTriggers.SelectedIndex]);
            tow.Show();
        }

        private void activeTriggers_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = ItemsControl.ContainerFromElement(activeTriggers, e.OriginalSource as DependencyObject) as ListBoxItem;
            if(item != null)
            {
                triggerOptionsButton.IsEnabled = true;
            }
        }
    }
}