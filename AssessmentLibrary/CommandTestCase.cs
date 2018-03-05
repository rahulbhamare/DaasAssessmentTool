using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace AssessmentLibrary
{
    public class CommandTestCase : BaseTestCase
    {        
        [JsonProperty("command")]
        public string TestCommand
        {
            get;
            private set;
        }

        [JsonProperty("params")]
        public string Params
        {
            get;
            private set;
        }

        [JsonProperty("expected_return_code")]
        public int ExpectedReturnCode
        {
            get;
            private set;
        }

        [JsonProperty("purpose")]
        public string Purpose
        {
            get;
            private set;
        }

        public int ActualReturnCode
        {
            get;
            set;
        }

        public override string TestCaseType => throw new NotImplementedException();

        public override bool DidTestCasePass()
        {
            return CaseStatus == TestCaseStatus.FINISHED &&
                    ExpectedReturnCode == ActualReturnCode;
        }

        public override void ClearTestResult()
        {
            base.ClearTestResult();
            ActualReturnCode = 0;
        }



    }
}
