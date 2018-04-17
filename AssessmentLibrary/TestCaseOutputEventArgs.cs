using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssessmentLibrary.BaseTestCase;
using static AssessmentLibrary.TestCaseRunner;

namespace AssessmentLibrary
{
    public class TestCaseOutputEventArgs : EventArgs
    {
        public enum OutputType
        {
            TestResult,
            TestResultCommand,
            Verbose
        }

        public string ConsoleOuput { get; private set; }

        public OutputType TestCaseOutputType { get; private set; }
        //public int TestResult { get; private set; }
        public bool TestCasePass { get; private set; }

        //public string TargetUrl { get; private set; }
        public TestCaseResult TestCaseResult { get; private set; }

        //public TestCaseOutputEventArgs(string consoleOutput)
        //{
        //    ConsoleOuput = consoleOutput;
        //    TestCaseOutputType = OutputType.Verbose;
        //}

        public TestCaseOutputEventArgs(TestCaseResult _testCaseResult)
        {
            TestCaseOutputType = OutputType.TestResult;
            TestCaseResult = _testCaseResult;
            //TestCasePass = result;
            //TargetUrl = url;
            //TestCaseOutputType = OutputType.TestResult;
        }

        public override string ToString()
        {
            if(TestCaseOutputType == OutputType.TestResult)
            {
                //return string.Format("{0} : {1}",TargetUrl, TestCasePass);
                return null;
            }
            else
            {
                return base.ToString();
            }
            
        }
    }
}
