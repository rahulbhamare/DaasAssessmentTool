using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssessmentLibrary;

namespace AssessmentLibraryTest
{
    [TestClass]
    public class TestCaseRunnerTest
    {
        [TestMethod]
        public void TestPassSuccess()
        {
            TestCaseRunner testCaseRunner = new TestCaseRunner("complete_tests_pass.json");
            //Assert.IsTrue(testCaseRunner.RunTestCases());
            Assert.IsTrue(testCaseRunner.DidAllTestCasesPass());
        }


        //[TestMethod]
        //public void NetworkTestPass()
        //{
        //    TestCaseRunner testCaseRunner = new TestCaseRunner("complete_tests_pass.json");
        //    Assert.IsTrue(testCaseRunner.RunTestCases());
        //    Assert.IsTrue(testCaseRunner.DidAllTestCasesPass());
        //}
    }
}
