using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssessmentLibrary;

namespace DaasAssessmentToolConsole
{
    class Program
    {
        private static TestCaseRunner testCaseRunner;
        static void PrintUsageInfo()
        {
            // will fill this out later
            Console.WriteLine("-t <filename>        Test config to run");
        }

        static void Main(string[] args)
        {
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
                Console.WriteLine("Running tests using test suite config:{0}", configFilePath);
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
                    string testResultString = string.Format(item.Purpose + "\r\n\r\n");
                    Console.WriteLine(testResultString);

                    // make configurable?
                    //Replace curl with C# default httpWebRequest API
                    testCaseRunner = new TestCaseRunner(item.FilePath);
                    //testCaseRunner.PathToCurl = "curl.exe";
                    testCaseRunner.TestCaseOutputEventHandler += TestCaseRunner_TestCaseOutputEventHandler;
                    //TODO: hook up string streaming
                    testCaseRunner.RunTestCases();
                    testResultString = string.Format(item.TestName + " test case status : {0} \r\n\r\n", testCaseRunner.DidAllTestCasesPass());

                    Console.WriteLine(testResultString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("\t" + ex.Message + "\r\n\r\n"));
                }
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
                    Console.WriteLine(e.ToString() + "\r\n\r\n");                    
                    break;
            }
            //TODO: scroll to bottom
        }
    }
}
