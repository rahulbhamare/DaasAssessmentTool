using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AssessmentLibrary;
using AssessmentLibrary.Logs;
using log4net;
using static AssessmentLibrary.TestCaseRunner;

namespace DaasAssessmentToolConsole
{
    class Program
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(Program));
        private static TestCaseRunner testCaseRunner;
        static void PrintUsageInfo()
        {
            // will fill this out later
            Console.WriteLine("-t <filename>        Test config to run");
        }

        static List<TestCaseResult> itemList = new List<TestCaseResult>();
        static void Main(string[] args)
        {
            DaasToolLogManagement.InitializeLog();
            LOG.Info("----------------------------------------------------------------------------------");
            LOG.Info(string.Format("{0} session has been started ", System.Security.Principal.WindowsIdentity.GetCurrent().Name));

            string configFilePath = string.Empty;
            
            if (args.Length > 1 && args[0] == "-t")
            {
                configFilePath = args[1];
            }

            if (string.IsNullOrEmpty(configFilePath))
            {
                PrintUsageInfo();
                //return 400; // what to return?
            }
            
            try
            {
                //Console.WriteLine("Running tests using test suite config:{0}", configFilePath);
                RunTests(args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //today's goal - get test run working
            // todo: figure out return paths, etc.
            //return returnValue;            
            Console.ReadKey();
        }

        private static void RunTests(string configFileName)
        {   
            List<ConfigFile> lstConfigFiles = BaseTestCase.LoadConfigFile(configFileName);
            string _testcasetype = string.Empty;

            foreach (var item in lstConfigFiles)
            {
                try
                {
                    _testcasetype = item.TestCaseType;
                    //string testResultString = string.Format(item.Purpose + "\r\n\r\n");
                    //Console.WriteLine(testResultString);

                    // make configurable?
                    //Replace curl with C# default httpWebRequest API
                    testCaseRunner = new TestCaseRunner(item.FilePath);
                    //testCaseRunner.PathToCurl = "curl.exe";
                    testCaseRunner.TestCaseOutputEventHandler += TestCaseRunner_TestCaseOutputEventHandler;
                    //TODO: hook up string streaming
                    testCaseRunner.RunTestCases();
                    //testResultString = string.Format(item.TestName + " test case status : {0} \r\n\r\n", testCaseRunner.DidAllTestCasesPass());
                    //Console.WriteLine(testResultString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("\t" + ex.Message + "\r\n\r\n"));
                }
            }

            Console.WriteLine("Do you want to export test case result? [yes/no]");
            string option = Console.ReadLine();
            if (option == "yes")
            {
                string filename = "TestCaseResult" + DateTime.Now.ToString("yyyy''MM''dd'T'HH''mm''ss") + ".csv";
                if (itemList == null || itemList.Count == 0) return;
                var sb = new StringBuilder();
                string csvHeaderRow = String.Join(",", typeof(TestCaseResult).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToArray<string>()) + Environment.NewLine;
                sb.AppendLine(csvHeaderRow);
                foreach (var data in itemList)
                {
                    sb.AppendLine(data.Target + "," + data.ProxyURL + ","+data.ProxyType+","+data.InputJsonFileName+","
                        +data.ExpectedStatusCode+","+ data.ActualStatusCode+","+data.Result+","+data.Description);
                }
                File.WriteAllText(filename, sb.ToString());
                Console.WriteLine(string.Format("Test case data has been exported to {0} file in current directory", filename));
            }
        }
        private static void TestCaseRunner_TestCaseOutputEventHandler(object sender, TestCaseOutputEventArgs e)
        {
            switch (e.TestCaseOutputType)
            {
                case TestCaseOutputEventArgs.OutputType.Verbose:
                    Console.WriteLine(e.ConsoleOuput + "\r\n\r\n");
                    break;
                case TestCaseOutputEventArgs.OutputType.TestResult:
                    itemList.Add(new TestCaseResult
                    {
                        Target = e.TestCaseResult.Target,
                        ProxyURL = e.TestCaseResult.ProxyURL,
                        ProxyType = e.TestCaseResult.ProxyType,
                        InputJsonFileName = e.TestCaseResult.InputJsonFileName,
                        ExpectedStatusCode = e.TestCaseResult.ExpectedStatusCode,
                        ActualStatusCode = e.TestCaseResult.ActualStatusCode,
                        Result = e.TestCaseResult.Result,
                        Description = e.TestCaseResult.Description
                    });
                    break;
            }
            //TODO: scroll to bottom
        }
    }
}
