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

namespace SteamChatBot
{
    /// <summary>
    /// Interaction logic for LoggerWindow.xaml
    /// </summary>
    public partial class LoggerWindow : Window
    {
        private System.Windows.Forms.NotifyIcon ni;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem quitMI;
        private System.Windows.Forms.MenuItem openMI;
        private System.ComponentModel.IContainer components;

        public LoggerWindow()
        {
            InitializeComponent();

            ni = new System.Windows.Forms.NotifyIcon();
            var ih = Properties.Resources.scb.GetHicon();
            ni.Icon = System.Drawing.Icon.FromHandle(ih);
            ni.Visible = true;
            ni.Click +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
            components = new System.ComponentModel.Container();
            contextMenu = new System.Windows.Forms.ContextMenu();
            quitMI = new System.Windows.Forms.MenuItem();
            openMI = new System.Windows.Forms.MenuItem();

            contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { openMI, quitMI });

            openMI.Index = 0;
            openMI.Text = "Open";
            openMI.Click += new EventHandler(openMI_Click);

            quitMI.Index = 1;
            quitMI.Text = "Quit";
            quitMI.Click += new EventHandler(quitMI_Click);

            ni.ContextMenu = contextMenu;

            ni.Text = "Steam Chat Bot";
        }

        private void quitMI_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openMI_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }
    }
}
