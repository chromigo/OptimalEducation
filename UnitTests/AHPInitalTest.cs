using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.Models;
using OptimalEducation.Logic.AnalyticHierarchyProcess;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class AHPInitalTest
    {
        //MethodName_Scenario_ExpectedBehavior
        [TestMethod]
        public void AHPInitial_DUNNO_DUNNO()
        {
            Console.WriteLine("TESTSTART");
            Console.WriteLine();

            var AHPUserInst = new AHPUser(21);

            Console.WriteLine("User ID is 21");
            Console.WriteLine();

            foreach (OptimalEducation.Logic.AnalyticHierarchyProcess.AHPUser.TotalResultUnit resultUnit in AHPUserInst.AllCriterionContainer)
            {
                Console.WriteLine(resultUnit.absolutePriority.ToString() + " - priority of Education Line with ID " + resultUnit.databaseId.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("END OF TEST");
        }
    }
}
