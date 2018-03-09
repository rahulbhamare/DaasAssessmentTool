using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
//using System.Threading.Tasks;

namespace AssessmentLibrary
{
    /**
     * <summary>Runs the specified test cases, gathering the results and output for consumption.</summary>
     */
    public class TestCaseRunner
    {
        private List<BaseTestCase> testCases = null;

        //Replace curl with C# default httpWebRequest API
        //public string PathToCurl { get; set; }

        /// <summary>
        /// Global setting to have test pass to run in system context.
        /// </summary>
        public bool RunInSystemContext
        {
            get;
            set;
        }

        public event EventHandler<TestCaseOutputEventArgs> TestCaseOutputEventHandler;

        // runner array to track process?
        //private int TestC

        // todo: fancy tasks and stuff

        protected virtual void OnTestCaseOutputEventHandler(TestCaseOutputEventArgs e)
        {
            TestCaseOutputEventHandler?.Invoke(this, e);
        }

        public TestCaseRunner() { }

        public TestCaseRunner(string testCasesPath)
        {
            testCases = BaseTestCase.LoadTests(testCasesPath);
            RunInSystemContext = false;
        }

        #region Existing code : Curl.exe
        //Replace curl with C# default httpWebRequest API
        // todo: async task
        //private bool RunTestCase(BaseTestCase testCase)
        //{

        //    // for now, keep it simple
        //    // we'll use more advanced stuff later

        //    // probably will need to make a runner class still
        //    // for now jam it inline
        //    Console.WriteLine("Running test case {0}", testCase.TestName);
        //    // is a ref... right?
        //    if(testCase is CommandTestCase)
        //    {
        //        //TODO: redirect output?
        //        Console.WriteLine("Command test case");
        //        CommandTestCase commandTestCase = testCase as CommandTestCase;
        //        Process testCaseProcess = Process.Start(commandTestCase.TestCommand, commandTestCase.Params);
        //        if(testCaseProcess.WaitForExit(60000))
        //        { 
        //            commandTestCase.ActualReturnCode = testCaseProcess.ExitCode;
        //            // TODO: do not assume that it actually finished
        //            commandTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;
        //        }
        //    }
        //    else if(testCase is HTTPTestCase)
        //    {
        //        Console.WriteLine("Network (HTTP) test case");
        //        HTTPTestCase httpTestCase = testCase as HTTPTestCase;
        //        // craft the curl request
        //        // TODO: modular so we may use restsdk?
        //        // first pass: just get requests, no proxy, etc.
        //        ProcessStartInfo startInfo = new ProcessStartInfo();
        //        //TODO: fix code path
        //        startInfo.FileName = PathToCurl;
        //        startInfo.Arguments = string.Format("-i {0} {1}", httpTestCase.Target, httpTestCase.FollowRedirect ? "-L": "");
        //        startInfo.RedirectStandardError = true;
        //        startInfo.RedirectStandardOutput = true;
        //        startInfo.UseShellExecute = false;
        //        startInfo.CreateNoWindow = true;

        //        Process process = Process.Start(startInfo);
        //        string output = process.StandardOutput.ReadToEnd();
        //        // to determine: where to look
        //        string errorOutput = process.StandardOutput.ReadToEnd();

        //        // TODO: output command line to logs

        //        string combinedOutput = httpTestCase.Target + "\r\n\r\n" +
        //            "Curl arguments:" + startInfo.Arguments + "\r\n\r\n" +
        //            output + "\r\n\r\n" + 
        //            errorOutput + "\r\n\r\n";

        //        OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(combinedOutput));

        //        if(process.WaitForExit(60000))
        //        {
        //            // for now, just output
        //            Console.WriteLine(output);
        //            Console.WriteLine("-------");
        //            Console.WriteLine(errorOutput);
        //            httpTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;
        //            // TEST TEST
        //            // TODO: how to pull out http code
        //            // TODO: if redirect that triggers more https that's not in scope. need to think about this
        //            httpTestCase.ActualResponseCode = CustomStringUtils.GetHTTPCodeFromCurlOutput(output, httpTestCase.FollowRedirect);
        //            httpTestCase.Location = CustomStringUtils.GetLocation(output, httpTestCase.FollowRedirect);

        //            OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(httpTestCase.Target,
        //                httpTestCase.DidTestCasePass()));
        //        }
        //        // TODO: if psexec how to capture output?
        //        // Another option - just run the whole thing as system...
        //        // worry later
        //    }
        //    else
        //    {
        //        Console.WriteLine("Test case type not yet supported.");
        //    }


        //    return true;
        //}

        // todo: async
        #endregion


        #region Newly added : HTTPClient patch
        private bool RunTestCase(BaseTestCase testCase)
        {
            // for now, keep it simple
            // we'll use more advanced stuff later
            // probably will need to make a runner class still
            // for now jam it inline
            Console.WriteLine("Running test case {0}", testCase.TestName);
            // is a ref... right?

            string Cookie = string.Empty;
            string ReUri = string.Empty;
            string combinedOutput = string.Empty;
            NameValueCollection collHeader = new NameValueCollection();
            HttpWebResponse webresponse = null;
            HttpWebRequest webrequest = null;

            if (testCase is CommandTestCase)
            {
                //TODO: redirect output?
                //Console.WriteLine("Command test case");
                CommandTestCase commandTestCase = testCase as CommandTestCase;
                int status = 0;

                switch (commandTestCase.Purpose)
                {
                    case "InstallExe":
                        status = RunProcessAsAdmin(commandTestCase.TestCommand, commandTestCase.Params);
                        combinedOutput = string.Format("\t {0} has been executed ", commandTestCase.TestName);
                        break;
                    case "ServiceStatus":
                        status = GetServiceStatus(commandTestCase.TestCommand);
                        combinedOutput = string.Format("\t {0} running ", commandTestCase.TestName);
                        break;
                    case "RegistryStatus":
                        status = CheckRegistryExists(commandTestCase.TestCommand);
                        combinedOutput = string.Format("\t {0} exists ", commandTestCase.TestName);
                        break;
                    case "ProcessStatus":
                        status = GetProcessStatus(commandTestCase.TestCommand);
                        combinedOutput = string.Format("\t {0} enrolled ", commandTestCase.TestName);
                        break;
                    default:break;
                }
                                 
                commandTestCase.ActualReturnCode = status;
                // TODO: do not assume that it actually finished
                
                SetResponseCode(commandTestCase, commandTestCase.ActualReturnCode, combinedOutput);

                //commandTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;
                //OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(combinedOutput,
                //       commandTestCase.DidTestCasePass()));
            }
            else if (testCase is HTTPTestCase)
            {
                Console.WriteLine("Network (HTTP) test case");
                HTTPTestCase httpTestCase = testCase as HTTPTestCase;                
                try
                {  
                    webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();
                    int temp = (int)webresponse.StatusCode;
                    //Check if there is any redirected URI
                    if (!string.IsNullOrEmpty(httpTestCase.RedirectLocation))
                    {
                        ReUri = httpTestCase.RedirectLocation;                        
                        ReUri = ReUri.Trim();
                        HTTPBaseClass BaseHttp = new HTTPBaseClass(httpTestCase.Username, httpTestCase.Password, httpTestCase.ProxyServer
                        , ReUri, "POST", collHeader);
                        webresponse = BaseHttp.GetFinalResponse(Cookie, false, true);//set auto redirect flag=true
                        httpTestCase.Location = ReUri;      
                    }
                    return true;
                }
                catch (WebException e)
                {
                    webresponse = (HttpWebResponse)e.Response;
                    if(webresponse != null)
                        httpTestCase.ActualResponseCode = (int)webresponse.StatusCode;
                    Console.WriteLine(e.Message);

                    // Obtain the 'Proxy' mentioned in the LAN settings
                    string defaultProxy = WebRequest.DefaultWebProxy.
                                GetProxy(new Uri(httpTestCase.Target)).Authority;
                    if (!string.IsNullOrEmpty(defaultProxy))
                    {
                        httpTestCase.ProxyServer = defaultProxy;                        
                        try
                        {                            
                            webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();
                            return true;
                        }
                        catch (WebException ex)
                        {
                            webresponse = (HttpWebResponse)ex.Response;
                            Console.WriteLine("Default proxy block :"+ ex.Message);
                        }
                    }

                    // Obtain the 'Proxy' mentioned in the Default browser
                    string staticProxy = WebRequest.GetSystemWebProxy().
                                GetProxy(new Uri(httpTestCase.Target)).Authority;
                    if (!string.IsNullOrEmpty(staticProxy))                    
                    {
                        httpTestCase.ProxyServer = staticProxy;                        
                        try
                        {
                            webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();                            
                            return true;
                        }
                        catch (WebException exc)
                        {
                            webresponse = (HttpWebResponse)exc.Response;
                            Console.WriteLine("Static proxy block :"+ exc.Message);
                        }
                    }
                }
                finally
                {
                    //Record Actual Status code 
                    int resposeCode = 0;
                    if(webresponse != null)
                    {
                        resposeCode = (int)webresponse.StatusCode;
                        httpTestCase.ActualResponseCode = resposeCode;
                        webresponse.Close();                        
                    }
                    combinedOutput = string.Format("\t Connection result to {0} ", httpTestCase.Target);
                    SetResponseCode(httpTestCase, httpTestCase.ActualResponseCode, combinedOutput);
                }
            }             
            return true;
        }

        //Check whether provided registry entry exists or not
        private int CheckRegistryExists(string testCommand)
        {   
            string[] topNode = testCommand.Split(new[] { '\\' }, 2);
            RegistryHive registryHive = RegistryHive.LocalMachine;

            //Assign registry top-level node
            switch(topNode[0].ToUpper())
            {
                case "HKEY_CLASSES_ROOT":
                    registryHive = RegistryHive.ClassesRoot;
                    break;
                case "HKEY_CURRENT_USER":
                    registryHive = RegistryHive.CurrentUser;
                    break;
                case "HKEY_LOCAL_MACHINE":
                    registryHive = RegistryHive.LocalMachine;
                    break;
                case "HKEY_USERS":
                    registryHive = RegistryHive.Users;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    registryHive = RegistryHive.CurrentConfig;
                    break;
                case "HKEY_DYN_DATA":
                    registryHive = RegistryHive.DynData;
                    break;
                case "HKEY_PERFORMANCE_DATA":
                    registryHive = RegistryHive.PerformanceData;
                    break;
                default:
                    break;
            }

            try
            {
                RegistryKey localKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
                using (RegistryKey Key = localKey.OpenSubKey(topNode[1]))
                {
                    if (Key != null)
                        return 1; // registry exists
                }
            }
            catch (Exception e)
            {            }
            return -1;// registry not exists
        }

        //Get service status
        //Expected status result is Running i.e. 4
        private int GetServiceStatus(string serviceName)
        {
            int status = -1;
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    status = (int)sc.Status;
                    if(status == 4)//Running
                        return 1;
                }
            }            
            catch (Exception) {  }
            return status;
        }

        //Get service status
        //Expected status result is Running i.e. 4
        private int GetProcessStatus(string processName)
        {
            try
            {
                Process[] pc = Process.GetProcessesByName(processName);
                foreach(var item in pc)
                {
                    if (item.Responding)
                        return 1;                    
                }                
            }            
            catch (Exception) { }
            return -1;
        }

        private HttpWebRequest SendWebRequest(HTTPTestCase httpTestCase, string RequestMethod, NameValueCollection collHeader)
        {
            HttpWebRequest webrequest = null;
            HTTPBaseClass BaseHttp = null;
            BaseHttp = new HTTPBaseClass(httpTestCase.Username, httpTestCase.Password, httpTestCase.ProxyServer
            , httpTestCase.Target, RequestMethod, collHeader);
            bool allowRedirect = (!string.IsNullOrEmpty(httpTestCase.RedirectLocation)) ? true : false;
            bool isNetworkCred = (!string.IsNullOrEmpty(httpTestCase.Username)
                && !string.IsNullOrEmpty(httpTestCase.Password)) ? true : false;
            return webrequest = BaseHttp.CreateWebRequest(isNetworkCred, allowRedirect);//false indicates: Http non-secure request            
        }
        private void SetResponseCode(BaseTestCase baseTestCase, int ActualResponseCode, string output)
        {
            string combinedOutput = "\t\n Response StatusCode: " + ActualResponseCode;
            OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(combinedOutput));
            baseTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;            
            OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(output,
                baseTestCase.DidTestCasePass()));
            Console.WriteLine("----------------------------------------------------------------------");
        }

        public int RunProcessAsAdmin(string fileName, string parameters)
        {
            try
            {
                // Get the current directory.
                //#if DEBUG
                //                //string filePath = Utils.GetInputFilePath(fileName);
                //                string filePath = fileName;
                //#else
                //                string filePath = fileName;
                //#endif
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = fileName;
                startInfo.Verb = "runas";
                startInfo.Arguments = parameters;
                startInfo.ErrorDialog = true;
                Process process = System.Diagnostics.Process.Start(startInfo);
                process.WaitForExit();
                return 1;
            }
            catch (Win32Exception ex)
            {
                // WriteLog(ex);
            }
            catch (Exception ex)
            {

            }
            return -1;//Indicates failure 
        }
        #endregion
        public bool RunTestCases()
        {
            foreach (var singleTestCase in testCases)
            {
                if(!RunTestCase(singleTestCase))
                {
                    return false;
                }
            }
            return true;
        }

        
        public bool DidAllTestCasesPass()
        {
            if(testCases.Count == 0)
            {
                return false;
            }
            foreach (var singleTestCase in testCases)
            {
                if (!singleTestCase.DidTestCasePass())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
