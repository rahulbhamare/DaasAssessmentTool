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
using System.Threading.Tasks;
using static AssessmentLibrary.BaseTestCase;
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

        public static string InputFileName = string.Empty;

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
            InputFileName = testCasesPath;
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
            string Cookie = string.Empty;
            string ReUri = string.Empty;
            string combinedOutput = string.Empty;

            NameValueCollection collHeader = new NameValueCollection();
            HttpWebResponse webresponse = null;
            
            if (testCase is CommandTestCase)
            {
                CommandTestCase commandTestCase = testCase as CommandTestCase;
                int status = 0;
                Dictionary<string, object> result = new Dictionary<string, object>();
                switch (commandTestCase.Purpose)
                {
                    case "InstallExe":
                        status = RunProcessAsAdmin(commandTestCase.Target, commandTestCase.Params);
                        if (status == 1)
                            combinedOutput = "Logs has been created at C:\\ path";
                        break;
                    case "ServiceStatus":
                        ServiceControllerStatus serviceControllerStatus =  GetServiceStatus(commandTestCase.Target);
                        if (serviceControllerStatus == ServiceControllerStatus.Running)                        
                            status = 1;                        
                        else
                            status = -1;
                        combinedOutput = string.Format("Service status : {0} ", serviceControllerStatus);
                        break;
                    case "RegistryStatus":
                        result = CheckRegistryExists(commandTestCase.Target);
                        if (result.Count > 0)
                        {
                            status = 1;
                            combinedOutput = "Registry contains following entries :";
                            foreach (var item in result)
                                combinedOutput += string.Format("(Key- {0}: Value- {1}) ",item.Key,item.Value);
                        }
                        else
                            status = -1;
                        break;
                    case "ProcessStatus":
                        status = GetProcessStatus(commandTestCase.Target);
                        break;
                    default:break;
                }
                                 
                commandTestCase.ActualReturnCode = status;
                commandTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;
                commandTestCase.Description = combinedOutput;
                SetResponseCode(commandTestCase);

                //Console ouput                 
                combinedOutput = string.Format("Running test case {0}\nResponse StatusCode : {1}\n\tConnection result to {2} : {3}",
                    commandTestCase.TestName, commandTestCase.ActualReturnCode, commandTestCase.Target,
                    (commandTestCase.ExpectedResponseCode == commandTestCase.ActualReturnCode) ? "Pass" : "Fail");
                Console.WriteLine(combinedOutput);
            }
            else if (testCase is HTTPTestCase)
            {                
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
                    httpTestCase.Description = "Exception message :\n" + e.Message;
                    // Obtain the 'Proxy' mentioned in the LAN settings
                    string defaultProxy = WebRequest.DefaultWebProxy.
                                GetProxy(new Uri(httpTestCase.Target)).Authority;
                    if (!string.IsNullOrEmpty(defaultProxy))
                    {
                        try
                        {                            
                            webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();
                            return true;
                        }
                        catch (WebException ex)
                        {
                            webresponse = (HttpWebResponse)ex.Response;
                            Console.WriteLine("Default proxy block :"+ ex.Message);
                            httpTestCase.Description = ex.Message;
                        }
                    }
                    // Obtain the 'Proxy' mentioned in the Default browser
                    string staticProxy = WebRequest.GetSystemWebProxy().
                                GetProxy(new Uri(httpTestCase.Target)).Authority;
                    if (!string.IsNullOrEmpty(staticProxy))                    
                    {
                        try
                        {
                            webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();                            
                            return true;
                        }
                        catch (WebException exc)
                        {
                            webresponse = (HttpWebResponse)exc.Response;
                            Console.WriteLine("Static proxy block :"+ exc.Message);
                            httpTestCase.Description = exc.Message;
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
                    //Console ouput 
                    combinedOutput = string.Format("Running test case {0}\nResponse StatusCode : {1}\n\tConnection result to {2} : {3}",
                    httpTestCase.TestName, httpTestCase.ActualResponseCode, httpTestCase.Target,
                    (httpTestCase.ExpectedResponseCode == httpTestCase.ActualResponseCode) ? "Pass" : "Fail");
                    Console.WriteLine(combinedOutput);
                    
                    SetResponseCode(httpTestCase);
                }
            }             
            return true;
        }

        //Check whether provided registry entry exists or not
        private Dictionary<string, object> CheckRegistryExists(string testCommand)
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
                Dictionary<string, object> keyValuePairs;
                RegistryKey localKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
                using (RegistryKey settingsRegKey = localKey.OpenSubKey(topNode[1]))
                {
                    var valueNames = settingsRegKey.GetValueNames();
                    keyValuePairs = valueNames.ToDictionary(name => name, settingsRegKey.GetValue);
                }
                return keyValuePairs;
            }
            catch (Exception e)
            {            }
            return new Dictionary<string, object>();// registry not exists
        }

        //Get service status
        //Expected status result is Running i.e. 4
        private ServiceControllerStatus GetServiceStatus(string serviceName)
        {
            ServiceControllerStatus serviceControllerStatus = ServiceControllerStatus.Stopped;
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    serviceControllerStatus = sc.Status;
                }
            }            
            catch (Exception) {  }
            return serviceControllerStatus;
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
        private void SetResponseCode(BaseTestCase baseTestCase)
        {
            HTTPTestCase httpTestCase = null;
            CommandTestCase commandTestCase = null;
            TestCaseOutputEventArgs testCaseOutputEventArgs;
            baseTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;
            InputFileName = InputFileName.Substring(InputFileName.IndexOf('\\') + 1);
            if (baseTestCase.TestCaseType == "http")
            {
                httpTestCase = (HTTPTestCase)baseTestCase;
                testCaseOutputEventArgs = new TestCaseOutputEventArgs(new TestCaseResult
                {
                    Target = httpTestCase.Target,
                    ProxyURL = httpTestCase.ProxyServer,
                    ProxyType = (!string.IsNullOrEmpty(httpTestCase.ProxyServer) ? "Manual" : "Static"),
                    InputJsonFileName = InputFileName,
                    ExpectedStatusCode = httpTestCase.ExpectedResponseCode,
                    ActualStatusCode = httpTestCase.ActualResponseCode,
                    Result = (httpTestCase.ExpectedResponseCode == httpTestCase.ActualResponseCode) ? "Pass" : "Fail",
                    Description = httpTestCase.Description
                });
            }
            else
            {
                commandTestCase = (CommandTestCase)baseTestCase;
                testCaseOutputEventArgs = new TestCaseOutputEventArgs(new TestCaseResult
                {
                    Target = commandTestCase.Target,
                    InputJsonFileName = InputFileName,
                    ExpectedStatusCode = commandTestCase.ExpectedResponseCode,
                    ActualStatusCode = commandTestCase.ActualReturnCode,
                    Result = (commandTestCase.ExpectedResponseCode == commandTestCase.ActualReturnCode) ? "Pass" : "Fail",
                    Description = commandTestCase.Description
                });
            }

            OnTestCaseOutputEventHandler(testCaseOutputEventArgs);
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
                //if (fileName.Contains("HPLogReportTool"))//HPLogReportTool.exe is physically present in package folder
                //    startInfo.UseShellExecute = false;
                //else

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = fileName;
                startInfo.Verb = "runas";
                startInfo.Arguments = parameters;
                startInfo.ErrorDialog = true;
                Process process = System.Diagnostics.Process.Start(startInfo);
                process.WaitForExit(6000);
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

        public class TestCaseResult
        {
            public string Target { get; set; }
            public string ProxyURL { get; set; }
            public string ProxyType { get; set; }
            public string InputJsonFileName { get; set; }
            public int ExpectedStatusCode { get; set; }
            public int ActualStatusCode { get; set; }
            public string Result { get; set; }//Pass or Fail
            public string Description { get; set; }
        }
    }
}
