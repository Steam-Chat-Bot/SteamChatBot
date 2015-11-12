using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace steam_chat_bot_net
{
    public partial class SteamChatBot : Form
    {
        public SteamChatBot()
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("login.json") && usernameBox.Text == "" && passwordBox.Text == "" && consoleLLBox.Text == "" && fileLLBox.Text == "" && sentryBox.Text == "" && logBox.Text == "" && autoJoinBox.Text == "" && displaynameBox.Text == "")
            {
                var _data = Bot.ReadData();
                logFile = _data.logFile;
                sentryFile = _data.sentryFile;
                autoJoinFile = _data.autoJoinFile;
                username = _data.username;
                password = _data.password;
                displayName = _data.displayName;
                
                Log = Log.CreateInstance(logFile, username, (consoleLLBox.Text == "" ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) : (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), consoleLLBox.Text, true)), (fileLLBox.Text == "" ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) : (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fileLLBox.Text, true)));
                Log.Instance.Silly("Successfully read login data from file");
                Bot.Start(username, password, (consoleLLBox.Text == null ? "Silly" : consoleLLBox.Text), (fileLLBox.Text == null ? "Silly" : fileLLBox.Text), logFile, displayName, autoJoinFile, sentryFile);
            }
            else
            {
                if (usernameBox.Text != "" && passwordBox.Text != "")
                {
                    Log = Log.CreateInstance((logBox.Text == "" ? usernameBox.Text + ".log" : logBox.Text), usernameBox.Text, (consoleLLBox.Text == "" ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) : (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), consoleLLBox.Text, true)), (fileLLBox.Text == "" ? (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), "Silly", true) : (Log.LogLevel)Enum.Parse(typeof(Log.LogLevel), fileLLBox.Text, true)));
                    Log.Instance.Silly("Console started successfully!");
                    if (passwordBox.Text != "" && displaynameBox.Text != null)
                    {
                        Bot.Start(usernameBox.Text, passwordBox.Text, (consoleLLBox.Text == null ? "Silly" : consoleLLBox.Text), (fileLLBox.Text == null ? "Silly" : fileLLBox.Text), (logFile == null ? usernameBox.Text + ".log" : logFile), displaynameBox.Text, (autoJoinFile == null ? usernameBox.Text + ".autojoin" : autoJoinFile), (sentryFile == null ? usernameBox.Text + ".sentry" : sentryFile));
                    }
                }
                else
                {
                    MessageBox.Show("Missing either username or password.");
                }
            }
        }

        #region gui stuff

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void passwordBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sentryBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void logBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void autoReconnectBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void autoJoinBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void fileLLBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void consoleLLBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void logFileBrowse_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void sentryFileBrowse_FileOk(object sender, CancelEventArgs e)
        {

        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if(dr == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            sentryFile = filename;
            sentryBox.Text = filename;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            logFile = filename;
            logBox.Text = filename;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filename = "";
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            autoJoinFile = filename;
            autoJoinBox.Text = filename;
        }
    }
}
