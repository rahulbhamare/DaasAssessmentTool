using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary.Logs
{
    public class TestingProperties
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(TestingProperties));

        #region Singleton
        private static readonly TestingProperties instance = new TestingProperties();

        [JsonConstructor()]
        private TestingProperties()
        {
            LoadFile();
        }

        private void LoadFile()
        {
            //string propsFile = Path.Combine(new FileInfo(typeof(TestingProperties).Assembly.Location).DirectoryName, "testingproperties.json");
            //if (File.Exists(propsFile))
            //{
            //    Properties = JsonConvert.DeserializeObject<Properties>(File.ReadAllText(propsFile));
            //    LOG.Info("Testing properties file loaded");
            //}
            //else
            //{
            //    Properties = new Properties();
            //    LOG.Debug("No testing properties file");
            //}
        }

        public static Properties Instance
        {
            get
            {
                return instance.Properties;
            }
        }
        #endregion

        internal static void Reload()
        {
            instance.LoadFile();
        }

        private Properties Properties { set; get; }

    }
}
