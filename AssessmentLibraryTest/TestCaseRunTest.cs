using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssessmentLibrary;
using System.Collections.Generic;

namespace AssessmentLibraryTest
{
    [TestClass]
    public class TestCaseRunTest
    {
        [TestMethod]
        public void ExerciseTestCaseResultField()
        {
            // TODO: repurpose test cases to exercise resetting

            List<BaseTestCase> myTests = BaseTestCase.LoadTests("SampleConfigFile.json");
            CommandTestCase commandTestCase = myTests[0] as CommandTestCase;
            Assert.IsTrue(commandTestCase.ActualReturnCode == 0);
            commandTestCase.ActualReturnCode = 1;
            commandTestCase.CaseStatus = BaseTestCase.TestCaseStatus.FINISHED;
            Assert.IsTrue(commandTestCase.ActualReturnCode == 1);
            Assert.IsTrue(commandTestCase.CaseStatus == BaseTestCase.TestCaseStatus.FINISHED);
            commandTestCase.ClearTestResult();
            Assert.IsTrue(commandTestCase.ActualReturnCode == 0);
            Assert.IsTrue(commandTestCase.CaseStatus == BaseTestCase.TestCaseStatus.NOT_YET_RUN);
        }

    }
}
