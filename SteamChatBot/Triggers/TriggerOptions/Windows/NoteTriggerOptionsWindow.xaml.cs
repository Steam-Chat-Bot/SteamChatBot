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
    /// Interaction logic for NoteTriggerOptionsWindow.xaml
    /// </summary>
    public partial class NoteTriggerOptionsWindow : Window
    {
        public NoteTriggerOptions NTO { get; set; }
        public NoCommand NC { get; set; }

        NoCommandWindow ncw = new NoCommandWindow();
        public NoteTriggerOptionsWindow()
        {
            InitializeComponent();
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            ncw.ShowDialog();
            if (ncw.DialogResult.HasValue && ncw.DialogResult == true)
            {
                NC = ncw.NC;
                NTO = new NoteTriggerOptions
                {
                    Name = NC.Name,
                    NoteCommand = "!note",
                    InfoCommand = "!note_info",
                    DeleteCommand = "!note_delete",
                    SaveTimer = 1000 * 60 * 5,
                    NoteFile = AppDomain.CurrentDomain.BaseDirectory + "/notes.json",
                    NotesCommand = "!notes"
                };

                if (noteCommandBox.Text != "") NTO.NoteCommand = noteCommandBox.Text;
                if (infoCommandBox.Text != "") NTO.InfoCommand = infoCommandBox.Text;
                if (deleteCommandBox.Text != "") NTO.DeleteCommand = deleteCommandBox.Text;
                if (saveTimerBox.Text != "" || Convert.ToInt32(saveTimerBox.Text) == 0) NTO.SaveTimer = Convert.ToInt32(saveTimerBox.Text);
                if (noteFileBox.Text != "") NTO.NoteFile = noteFileBox.Text;
                if (notesCommandBox.Text != "") NTO.NotesCommand = notesCommandBox.Text;

                DialogResult = true;
                Close();
            }
        }

        private void noteFileBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string file = "";
            OpenFileDialog ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == true)
            {
                file = ofd.FileName;
                noteFileBox.Text = ofd.FileName;
            }
        }
    }
}
