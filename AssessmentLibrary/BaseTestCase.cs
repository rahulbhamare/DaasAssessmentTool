using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public abstract class BaseTestCase
    {
        public static string FIELD_TESTCASE_LIST = "testcases";

        public static string FIELD_CONFIGFILE_LIST = "configfiles";

        public enum TestCaseStatus
        {
            NOT_YET_RUN,
            RUNNING,
            FINISHED,
            FAILED_TO_RUN
        }

        [JsonProperty("name")]
        public string TestName
        {
            get;
            set;
        }
        public TestCaseStatus CaseStatus
        {
            get;
            set;
        }
        public abstract string TestCaseType
        {
            get;
        }

        public abstract bool DidTestCasePass();

        public virtual void ClearTestResult()
        {
            CaseStatus = TestCaseStatus.NOT_YET_RUN;
        }

        public static List<BaseTestCase> LoadTests(string fileName)
        {
            // todo: deal with throws
            // is there something like path for .net?
            string jsonString = string.Empty;
            string filePath = Utils.GetInputFilePath(fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                jsonString = streamReader.ReadToEnd();
            }
            return LoadTestsFromString(jsonString);
        }

        public static List<BaseTestCase> LoadTestsFromString(string testCases)
        {
            List<BaseTestCase> myTestCases = new List<BaseTestCase>();
            JObject testCaseJson = JObject.Parse(testCases);
            JArray testCaseArray = (JArray)testCaseJson[FIELD_TESTCASE_LIST];
            foreach (JObject singleCase in testCaseArray)
            {
                // for now, go lazy
                string testCaseType = (string)singleCase["type"];
                if (testCaseType == "command")
                {
                    myTestCases.Add(singleCase.ToObject<CommandTestCase>());
                }
                else if (testCaseType == "http")
                {
                    myTestCases.Add(singleCase.ToObject<HTTPTestCase>());
                }
            }
            return myTestCases;
        }

        public static List<ConfigFile> LoadConfigFile(string fileName)
        {
            // todo: deal with throws
            // is there something like path for .net?
            string jsonString = string.Empty;
            string filePath = Utils.GetInputFilePath(fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                jsonString = streamReader.ReadToEnd();
            }
            return LoadConfigFileFromString(jsonString);
        }

        public static List<ConfigFile> LoadConfigFileFromString(string testCases)
        {
            List<ConfigFile> configFileList = new List<ConfigFile>();
            JObject testCaseJson = JObject.Parse(testCases);
            JArray testCaseArray = (JArray)testCaseJson[FIELD_CONFIGFILE_LIST];
            foreach (JObject singleCase in testCaseArray)
            {
                configFileList.Add(singleCase.ToObject<ConfigFile>());
            }
            return configFileList;
        }
        // todo: keep it simple, if list becomes insane then refactor
        // or maybe it will magically work...
    }
}
