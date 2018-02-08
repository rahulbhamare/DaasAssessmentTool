using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

            if (testCase is CommandTestCase)
            {
                //TODO: redirect output?
                Console.WriteLine("Command test case");
                CommandTestCase commandTestCase = testCase as CommandTestCase;
                int status = RunProcessAsAdmin(commandTestCase.TestCommand, commandTestCase.Params);
                commandTestCase.ActualReturnCode = status;
                // TODO: do not assume that it actually finished
                commandTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;                
            }
            else if (testCase is HTTPTestCase)
            {
                Console.WriteLine("Network (HTTP) test case");
                HTTPTestCase httpTestCase = testCase as HTTPTestCase;      
                
                try
                {
                    webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();
                    
                    //Check if there is any redirected URI.
                    if (!string.IsNullOrEmpty(httpTestCase.RedirectLocation))
                    {
                        ReUri = httpTestCase.RedirectLocation;                        
                        ReUri = ReUri.Trim();
                        HTTPBaseClass BaseHttp = new HTTPBaseClass(httpTestCase.Username, httpTestCase.Password, httpTestCase.ProxyServer
                        , ReUri, "POST", collHeader);
                        webresponse = BaseHttp.GetFinalResponse(Cookie, false);
                        httpTestCase.Location = ReUri;                        
                    }
                    return true;
                }
                catch (WebException e)
                {
                    webresponse = (HttpWebResponse)e.Response;                    
                    Console.WriteLine(e.Message);

                    //check for auto detect proxy    
                    //if (httpTestCase.GetDefaultProxy() != null)
                    //{
                    //    WebProxy proxy = (WebProxy)HttpWebRequest.DefaultWebProxy;                        
                    //    httpTestCase.ProxyServer = Convert.ToString(proxy.Address);
                    //    try
                    //    {
                    //        webresponse = (HttpWebResponse)SendWebRequest(httpTestCase, "GET", collHeader).GetResponse();
                    //        return true;
                    //    }
                    //    catch (WebException ex)
                    //    {
                    //        webresponse = (HttpWebResponse)e.Response;
                    //        Console.WriteLine(ex.Message);                                                        
                    //    }
                    //}

                    // Obtain the 'Proxy' of the  Default browser.
                    if (httpTestCase.GetStaticProxy() != null)
                    {
                        HttpWebRequest webrequest = SendWebRequest(httpTestCase, "GET", collHeader);                        
                        IWebProxy proxy = WebRequest.DefaultWebProxy;
                        try
                        {
                            webrequest.Proxy = proxy;
                            webresponse = (HttpWebResponse)webrequest.GetResponse();
                            return true;
                        }
                        catch (WebException exc)
                        {
                            webresponse = (HttpWebResponse)e.Response;
                            Console.WriteLine(exc.Message);
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
                    
                    SetResponseCode(httpTestCase);                                        
                }
            }            
            return true;
        }

        private HttpWebRequest SendWebRequest(HTTPTestCase httpTestCase, string RequestMethod, NameValueCollection collHeader)
        {
            HttpWebRequest webrequest = null;
            HTTPBaseClass BaseHttp = null;
            BaseHttp = new HTTPBaseClass(httpTestCase.Username, httpTestCase.Password, httpTestCase.ProxyServer
            , httpTestCase.Target, RequestMethod, collHeader);
            return webrequest = BaseHttp.CreateWebRequest(false);//false indicates: Http non-secure request            
        }
        private void SetResponseCode(HTTPTestCase HttpTestCase)
        {
            // ... Check Status Code                                
            Console.WriteLine("Response StatusCode: " + HttpTestCase.ActualResponseCode);
            string combinedOutput = HttpTestCase.Target + "\r\n\r\n" + "HTTPWebRequest arguments:" + HttpTestCase.ActualResponseCode 
                + "\r\n\r\n";

            OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(combinedOutput));
            //////// for now, just output
            Console.WriteLine(combinedOutput);
            Console.WriteLine("-------");
            HttpTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;

            //////// TEST TEST
            //////// TODO: how to pull out http code
            OnTestCaseOutputEventHandler(new TestCaseOutputEventArgs(HttpTestCase.Target,
                HttpTestCase.DidTestCasePass()));
        }

        public int RunProcessAsAdmin(string fileName, string parameters)
        {
            try
            {
                // Get the current directory.
                string filePath = Utils.GetInputFilePath(fileName);
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
                startInfo.FileName = filePath;
                startInfo.Verb = "runas";                
                startInfo.Arguments = parameters;
                startInfo.ErrorDialog = true;

                Process process = System.Diagnostics.Process.Start(startInfo);
                process.WaitForExit();
                return process.ExitCode;
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
