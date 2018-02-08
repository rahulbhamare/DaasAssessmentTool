using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AssessmentLibrary;

namespace DaasAssessmentTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestCaseRunner testCaseRunner;

        private string verboseOutput = string.Empty;

        private TestVerboseWindow verboseWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            verboseWindow = new TestVerboseWindow();

            #region Zip TM client Log option with Verbose logging on Readiness Tool
            //RunProcessAsAdmin("HPLogReportTool\\HPLogReportTool.exe", "");
            #endregion
        }

        private async void RunTestsButton_Click(object sender, RoutedEventArgs e)
        {
            verboseOutput = string.Empty;
            ConsoleOutputTextBox.Text = "Running test cases \r\n\r\n";
            await Task.Run(() => RunTests());

            /*
             * idea - two tabs, one for high level overview, one for raw/verbose
             * OR just store in string, pop up window with export(or just export)             
             */
        }

        private void EnableButtons(bool enableButtons)
        {
            //todo: any way to do all? or more elegantly?
            RunTestsButton.Dispatcher.Invoke(new Action(() => RunTestsButton.IsEnabled = enableButtons));
            SeeVerboseButton.Dispatcher.Invoke(new Action(() => SeeVerboseButton.IsEnabled = enableButtons));
        }

        private void RunTests()
        {
            if(verboseWindow.IsVisible)
            {
                verboseWindow.Hide();                
            }
            EnableButtons(false);
            List<ConfigFile> lstConfigFiles = BaseTestCase.LoadConfigFile("config_file.json");
            string _testcasetype = string.Empty;
            foreach (var item in lstConfigFiles)
            {
                _testcasetype = item.TestCaseType;
                string testResultString = string.Format(item.Purpose + "\r\n\r\n");
                ConsoleOutputTextBox.Dispatcher.Invoke(new Action(() => ConsoleOutputTextBox.FontWeight = FontWeights.Bold));
                ConsoleOutputTextBox.Dispatcher.Invoke(new Action(() => ConsoleOutputTextBox.AppendText(testResultString)));
                testCaseRunner = new TestCaseRunner(item.FilePath);
                // make configurable?
                //Replace curl with C# default httpWebRequest API
                //testCaseRunner.PathToCurl = "curl.exe";
                testCaseRunner.TestCaseOutputEventHandler += TestCaseRunner_TestCaseOutputEventHandler;
                ConsoleOutputTextBox.Dispatcher.Invoke(new Action(() => ConsoleOutputTextBox.FontWeight = FontWeights.Normal));
                //TODO: hook up string streaming
                testCaseRunner.RunTestCases();

                testResultString = string.Format(item.TestName + " test case status : {0} \r\n\r\n", testCaseRunner.DidAllTestCasesPass());
                ConsoleOutputTextBox.Dispatcher.Invoke(new Action(() => ConsoleOutputTextBox.AppendText(testResultString)));
            }
            EnableButtons(true);
        }

        private void TestCaseRunner_TestCaseOutputEventHandler(object sender, TestCaseOutputEventArgs e)
        {
            switch(e.TestCaseOutputType)
            {
                case TestCaseOutputEventArgs.OutputType.Verbose:
                    verboseOutput += e.ConsoleOuput;
                    break;
                case TestCaseOutputEventArgs.OutputType.TestResult:
                    ConsoleOutputTextBox.Dispatcher.Invoke(new Action(() => ConsoleOutputTextBox.AppendText(e.ToString() + "\r\n\r\n")));
                    break;
            }
            //TODO: scroll to bottom
        }

        private void SeeVerboseButton_Click(object sender, RoutedEventArgs e)
        {
            verboseWindow.VerboseOutput = verboseOutput;
            verboseWindow.TestResultOutput = ConsoleOutputTextBox.Text;
            verboseWindow.Show();
        }        
    }    
}
