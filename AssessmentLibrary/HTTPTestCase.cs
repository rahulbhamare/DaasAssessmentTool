using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AssessmentLibrary
{
    public class HTTPTestCase : BaseTestCase
    {
        [JsonProperty("target")]
        public string Target
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

        [JsonProperty("follow_redirect")]
        public bool FollowRedirect { get; private set; } = false;

        [JsonProperty("redirect_location")]
        public string RedirectLocation { get; private set; } = string.Empty;


        // to determine: is this necessary?
        public override string TestCaseType
        {
            get
            {
                return "http";
            }
        }

        public int ActualResponseCode
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        [JsonProperty("proxyserver")]
        public string ProxyServer
        {
            get;
            set;
        }
        
        public override bool DidTestCasePass()
        {
            bool httpCodeMatch = CaseStatus == TestCaseStatus.FINISHED &&                 
                ExpectedResponseCode == ActualResponseCode;

            if(RedirectLocation != string.Empty)
            {
                return httpCodeMatch && (RedirectLocation == Location);
            }
            else
            { 
                return httpCodeMatch;
            }
        }
                
        //TODO: insert shared code to instruct how to run curl commands
    }
}
