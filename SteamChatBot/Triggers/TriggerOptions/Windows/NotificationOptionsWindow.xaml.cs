using Microsoft.Win32;
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
    /// Interaction logic for NotificationOptionsWindow.xaml
    /// </summary>
    public partial class NotificationOptionsWindow : Window
    {
        public NotificationOptions NO { get; set; }
        public NoCommand NC { get; set; }

        NoCommandWindow ncw = new NoCommandWindow();
    
        public NotificationOptionsWindow()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            ncw.ShowDialog();
            if(ncw.DialogResult.HasValue && ncw.DialogResult == true)
            {
                NC = ncw.NC;
                NO = new NotificationOptions
                {
                    Name = ncw.NC.Name,
                    SaveTimer = 1000 * 60 * 5,
                    APICommand = "!pbapi",
                    SeenCommand = "!seen",
                    FilterCommand = "!filter",
                    ClearCommand = "!clear",
                    DBFile = AppDomain.CurrentDomain.BaseDirectory + "/notification.json",
                };

                if (saveTimerBox.Text != "") NO.SaveTimer = Convert.ToInt32(saveTimerBox.Text);
                if (dbFileBox.Text != "") NO.DBFile = dbFileBox.Text;
                if (clearCommandBox.Text != "") NO.ClearCommand = clearCommandBox.Text;
                if (apiCommandBox.Text != "") NO.APICommand = apiCommandBox.Text;
                if (seenCommandBox.Text != "") NO.SeenCommand = seenCommandBox.Text;
                if (filterCommandBox.Text != "") NO.FilterCommand = filterCommandBox.Text;
            }

            DialogResult = true;
            Close();
        }

        private void dbFileBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string file = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == true)
            {
                file = ofd.FileName;
                dbFileBox.Text = ofd.FileName;
            }
        }
    }
}
