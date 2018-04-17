using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssessmentLibrary;
using System.Collections.Generic;

namespace AssessmentLibraryTest
{
    [TestClass]
    public class TestCaseTest
    {
        [TestMethod]
        public void LoadTestCaseFromJson()
        {
            /**
             * Hello world goal: 
             * Execute a command based on the loaded json
             * 
             * later on: group of tests that define success
             * for now, keep it simple. all or nothing
             */
            List<BaseTestCase> myTests = BaseTestCase.LoadTests("SampleConfigFile.json");

            Assert.IsTrue(myTests.Count == 2);
            Assert.IsTrue(myTests[0].TestName == "My test case");
            Assert.IsTrue(myTests[0] is CommandTestCase);
            CommandTestCase commandTestCase = myTests[0] as CommandTestCase;
            //Assert.IsTrue(commandTestCase.TestCommand == "c:\\Some command.exe");
            Assert.IsTrue(commandTestCase.ExpectedResponseCode == 0);
            Assert.IsTrue(commandTestCase.Params == "-param1 -param2 3");
        }

        [TestMethod]
        public void LoadNetworkTestsFromJson()
        {
            /**
             * Hello world goal: 
             * Execute a command based on the loaded json
             * 
             * later on: group of tests that define success
             * for now, keep it simple. all or nothing
             */
            List<BaseTestCase> myTests = BaseTestCase.LoadTests("network_tests.json");

            Assert.IsTrue(myTests.Count == 3);
            Assert.IsTrue(myTests[0].TestName == "HP Daas");
            Assert.IsTrue(myTests[0] is HTTPTestCase);
            HTTPTestCase httpTestCase = myTests[0] as HTTPTestCase;
            Assert.IsTrue(httpTestCase.FollowRedirect == false);
            Assert.IsTrue(httpTestCase.Target == "https://www.hpdaas.com");
            Assert.IsTrue(httpTestCase.ExpectedResponseCode == 200);

            Assert.IsTrue(myTests[1].TestName == "Look at redirect location");
            HTTPTestCase secondTestCase = myTests[1] as HTTPTestCase;
            //Assert.IsTrue(secondTestCase.FollowRedirect == true);
            Assert.IsTrue(secondTestCase.Target == "http://o.ss2.us/");
            Assert.IsTrue(secondTestCase.RedirectLocation == "http://www.starfieldtech.com/");
            Assert.IsTrue(secondTestCase.ExpectedResponseCode == 301);

            Assert.IsTrue(myTests[2].TestName == "Network redirect");
            HTTPTestCase thirdTestCase = myTests[2] as HTTPTestCase;
            //Assert.IsTrue(secondTestCase.FollowRedirect == true);
            Assert.IsTrue(thirdTestCase.Target == "http://o.ss2.us/");
            Assert.IsTrue(thirdTestCase.ExpectedResponseCode == 200);
        }
    }
}
