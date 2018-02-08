using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public class ConfigFile
    {
        [JsonProperty("name")]
        public string TestName
        {
            get;
            set;
        }

        [JsonProperty("purpose")]
        public string Purpose
        {
            get;
            set;
        }

        [JsonProperty("path")]
        public string FilePath
        {
            get;
            set;
        }

        [JsonProperty("type")]
        public string TestCaseType
        {
            get;
            set;
        }
    }


}
