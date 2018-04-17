using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AssessmentLibrary;
using AssessmentLibrary.Logs;
using log4net;
using static AssessmentLibrary.TestCaseRunner;

namespace DaasAssessmentTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestCaseRunner testCaseRunner;
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            
            //Initialize logs
            DaasToolLogManagement.InitializeLog();

            #region Zip TM client Log option with Verbose logging on Readiness Tool
            //RunProcessAsAdmin("HPLogReportTool\\HPLogReportTool.exe", "");
            #endregion
        }

        // Check internet connectivity status changed
        private static bool IsNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // however, this will include all adapters
                NetworkInterface[] interfaces =
                    NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface face in interfaces)
                {
                    // filter so we see only Internet adapters
                    if (face.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
                            (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            IPv4InterfaceStatistics statistics =
                                face.GetIPv4Statistics();

                            // all testing seems to prove that once an interface
                            // comes online it has already accrued statistics for
                            // both received and sent...

                            if ((statistics.BytesReceived > 0) &&
                                (statistics.BytesSent > 0))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private async void RunTestsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsNetworkAvailable())
                MessageBox.Show("You are not connected to the Internet, please connect to internet and try again.", " Internet status");
            else
                await Task.Run(() => RunTests());
        }

        private void EnableButtons(bool enableButtons)
        {
            //todo: any way to do all? or more elegantly?
            RunTestsButton.Dispatcher.InvokeAsync(new Action(() => RunTestsButton.IsEnabled = enableButtons));
            ExportDataButton.Dispatcher.InvokeAsync(new Action(() => ExportDataButton.IsEnabled = enableButtons));
        }

        private async void RunTests()
        {   
            EnableButtons(false);
            List<ConfigFile> lstConfigFiles = BaseTestCase.LoadConfigFile("config_file.json");
            string _testcasetype = string.Empty;
            foreach (var item in lstConfigFiles)
            {
                try
                {
                    _testcasetype = item.TestCaseType;
                    string testResultString = string.Format(item.Purpose + "\r\n\r\n");
                    testCaseRunner = new TestCaseRunner(item.FilePath);
                    testCaseRunner.TestCaseOutputEventHandler += TestCaseRunner_TestCaseOutputEventHandler;
                    testCaseRunner.RunTestCases();
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(new Action(() =>
                    TestResultVM.TestResultObservableCollection.Add(new TestCaseResult
                    {
                        Target = item.TestName,
                        InputJsonFileName = item.FilePath,
                        Result = "Fail",
                        Description = ex.Message
                    })));
                }
            }            
            EnableButtons(true);
        }
        private async void TestCaseRunner_TestCaseOutputEventHandler(object sender, TestCaseOutputEventArgs e)
        {
            switch (e.TestCaseOutputType)
            {
                //case TestCaseOutputEventArgs.OutputType.Verbose:
                //    verboseOutput += e.ConsoleOuput;
                //    break;
                case TestCaseOutputEventArgs.OutputType.TestResult:
                    await Dispatcher.InvokeAsync(new Action(() =>
                    TestResultVM.TestResultObservableCollection.Add(new TestCaseResult
                    {
                        Target = e.TestCaseResult.Target,
                        ProxyURL = e.TestCaseResult.ProxyURL,
                        ProxyType = e.TestCaseResult.ProxyType,
                        InputJsonFileName = e.TestCaseResult.InputJsonFileName,
                        ExpectedStatusCode = e.TestCaseResult.ExpectedStatusCode,
                        ActualStatusCode = e.TestCaseResult.ActualStatusCode,
                        Result = e.TestCaseResult.Result,
                        Description = e.TestCaseResult.Description
                    })));
                    break;
            }
        }
        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            //ExportToExcel<TestCaseResult, ObservableCollection<TestCaseResult>> s = new ExportToExcel<TestCaseResult, ObservableCollection<TestCaseResult>>();
            //s.dataToPrint = TestResultVM.TestResultObservableCollection;
            //s.GenerateReport();
            string filename = "TestCaseResult" + DateTime.Now.ToString("yyyy''MM''dd'T'HH''mm''ss")+".csv";
            TestCaseDataGrid.SelectAllCells();
            TestCaseDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, TestCaseDataGrid);
            TestCaseDataGrid.UnselectAllCells();
            string LivePreviewText = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            File.AppendAllText(filename, LivePreviewText, UnicodeEncoding.UTF8);
            MessageBox.Show(string.Format("Test case data has been exported to {0} file in current directory",filename));
        }
    }    
}
