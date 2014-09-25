using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.DAL.Models;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;
using NSubstitute;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Implementation.Logic.MulticriterialAnalysis;
using System.Threading.Tasks;

namespace OptimalEducation.UnitTests.Logic.MulticriterialAnalysis
{
    [TestClass]
    public class MulticriterialAnalysisTest
    {
        [TestMethod]
        public void Calculate_correct_recomendation_with_current_data_set()
        {
            //Arrange
            var entrantCharacterizer = Substitute.For<ICharacterizer<Entrant>>();
            var educationLineCharacterizer = Substitute.For<ICharacterizer<EducationLine>>();
            var preferenceRelationCalculator = Substitute.For<IPreferenceRelationCalculator>();

            var entrant = new Entrant();
            var educationLines = new List<EducationLine>()
            {
                new EducationLine(){Id=1,
                    EducationLinesRequirements=new List<EducationLineRequirement>()
                {
                    new EducationLineRequirement(),
                    new EducationLineRequirement()
                }},
                new EducationLine(){Id=2,
                    EducationLinesRequirements=new List<EducationLineRequirement>()
                {
                    new EducationLineRequirement(),
                    new EducationLineRequirement()
                }},
            };
            var entrantCharacterisitc = new Dictionary<string, double>();
            entrantCharacterisitc.Add("Русский", 0.5);
            entrantCharacterisitc.Add("Математика", 0.6);
            entrantCharacterisitc.Add("Информатика", 0.7);

            var edLine1Characterisitc = new Dictionary<string, double>();
            edLine1Characterisitc.Add("Русский", 0.4);
            edLine1Characterisitc.Add("Математика", 0.6);
            edLine1Characterisitc.Add("Информатика", 0.8);
            var edLine2Characterisitc = new Dictionary<string, double>();
            edLine2Characterisitc.Add("Русский", 0.7);
            edLine2Characterisitc.Add("Математика", 0.5);
            edLine2Characterisitc.Add("Информатика", 0.5);

            entrantCharacterizer.Calculate(entrant).Returns(Task.FromResult(entrantCharacterisitc));
            educationLineCharacterizer.Calculate(educationLines[0]).Returns(Task.FromResult(edLine1Characterisitc));
            educationLineCharacterizer.Calculate(educationLines[1]).Returns(Task.FromResult(edLine2Characterisitc));

            var tetas = new Dictionary<string,double>();
            tetas.Add("Русский",0.67);
            tetas.Add("Математика",0.5);

            var prefRelation = new List<PreferenceRelation>()
            {
                new PreferenceRelation("Информатика")
                {
                    Tetas=tetas
                }
            };
            preferenceRelationCalculator.GetPreferenceRelations(entrantCharacterisitc).Returns(prefRelation);

            //Act
            var multicriterialAnalysisRecomendator = new MulticriterialAnalysisRecomendator(entrantCharacterizer, educationLineCharacterizer, preferenceRelationCalculator);
            var taskResult = multicriterialAnalysisRecomendator.Calculate(entrant, educationLines);
            taskResult.Wait();
            var result = taskResult.Result;

            //Assert
            Assert.AreEqual(educationLines[0], result.Single());
        }
    }
}
