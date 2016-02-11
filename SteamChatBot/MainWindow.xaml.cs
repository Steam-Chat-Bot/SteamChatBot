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
                AddTriggersToList();
                Bot.Start(username, password, cll, fll, logFile, displayName, sentryFile);
                this.Hide();
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
                        AddTriggersToList();
                        Bot.Start(usernameBox.Text, passwordBox.Password, (cll == null ? "Silly" : 
                            cll.ToString()), (fll == null ? "Silly" : 
                            fll.ToString()), (logFile == null ? usernameBox.Text + ".log" : logFile), 
                            displaynameBox.Text, (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile));
                        this.Hide();
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
            foreach(CheckBox box in triggerCheckBoxList.Items)
            {
                Bot.checkBoxes.Add(box);
            }
        }

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