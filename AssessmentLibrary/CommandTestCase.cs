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
        public string Target
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

        [JsonProperty("expected_response_code")]
        public int ExpectedResponseCode
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

        public override string TestCaseType
        {
            get
            {
                return "command";
            }
        }

        public int ActualReturnCode
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
        public override bool DidTestCasePass()
        {
            return CaseStatus == TestCaseStatus.FINISHED &&
                    ExpectedResponseCode == ActualReturnCode;
        }

        public override void ClearTestResult()
        {
            base.ClearTestResult();
            ActualReturnCode = 0;
        }
    }
}
