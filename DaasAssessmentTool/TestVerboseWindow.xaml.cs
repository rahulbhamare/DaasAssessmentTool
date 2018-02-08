using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaasAssessmentTool
{
    /// <summary>
    /// Interaction logic for TestVerboseWindow.xaml
    /// </summary>
    public partial class TestVerboseWindow : Window
    {
        public string VerboseOutput { get; set; }

        public string TestResultOutput { get; set; }

        public TestVerboseWindow()
        {
            InitializeComponent();
        }

        private void SaveLogsButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "verbosetestlogs";
            dlg.DefaultExt = ".log";
            dlg.Filter = "Log files (.log) | *.log";

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true)
            {
                // do IO here
                string filename = dlg.FileName;
                try
                {
                    File.WriteAllText(filename, TestResultOutput + "\n\n" + VerboseOutput);
                }
                catch(IOException)
                {
                    MessageBox.Show("Failed to write log file due to IOException", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool visibility = (bool)e.NewValue;

            if(!visibility)
            {
                ConsoleOutputTextBox.Text = string.Empty;
            } else
            {
                ConsoleOutputTextBox.Text = VerboseOutput;
            }
        }
    }
}
