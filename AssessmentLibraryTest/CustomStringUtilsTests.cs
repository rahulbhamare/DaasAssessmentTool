using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssessmentLibrary;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary.Tests
{
    [TestClass()]
    public class CustomStringUtilsTests
    {
        [TestMethod()]
        public void GetHTTPCodeFromCurlOutputTest()
        {
            string curlSample = File.ReadAllText("SampleCurlOutput.txt");
            int actualCode = CustomStringUtils.GetHTTPCodeFromCurlOutput(curlSample);
            Assert.IsTrue(actualCode == 200);
        }


        [TestMethod()]
        public void FollowRedirectToGetFinalHTTPCode()
        {
            string curlSample = File.ReadAllText("CurlWithRedirect.txt");
            int actualCode = CustomStringUtils.GetHTTPCodeFromCurlOutput(curlSample, true);
            Assert.IsTrue(actualCode == 200);
        }

        [TestMethod()]
        public void GetLocationTest()
        {
            string curlSample = File.ReadAllText("CurlWithRedirect.txt");
            // TODO: insert correct test code
            int actualCode = CustomStringUtils.GetHTTPCodeFromCurlOutput(curlSample, false);
            Assert.IsTrue(actualCode == 301);

            string location = CustomStringUtils.GetLocation(curlSample);
            Assert.IsTrue(location == "http://www.starfieldtech.com/");
        }
    }
}