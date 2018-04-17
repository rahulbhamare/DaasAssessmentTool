using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public class TestCaseResult
    {
        public string Target { get; set; }
        public string ProxyURL { get; set; }
        public string ProxyType { get; set; }
        public string InputJsonFileName { get; set; }
        public int ExpectedStatusCode { get; set; }
        public int ActualStatusCode { get; set; }
        public string Result { get; set; }//Pass or Fail
    }
}
