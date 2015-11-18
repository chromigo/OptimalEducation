using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.Characterizers;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;


namespace OptimalEducation.UnitTests.Logic.MulticriterialAnalysis
{
    [TestFixture]
    public class MulticriterialAnalysisTest
    {
        [Test]
        public void Calculate_correct_recomendation_with_current_data_set()
        {
            //Arrange
            var entrantCharacterizer = Substitute.For<ICharacterizer<Entrant>>();
            var educationLineCharacterizer = Substitute.For<ICharacterizer<EducationLine>>();
            var preferenceRelationCalculator = Substitute.For<IPreferenceRelationCalculator>();

            var entrant = new Entrant();
            var educationLines = new List<EducationLine>
            {
                new EducationLine
                {
                    Id = 1,
                    EducationLinesRequirements = new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement(),
                        new EducationLineRequirement()
                    }
                },
                new EducationLine
                {
                    Id = 2,
                    EducationLinesRequirements = new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement(),
                        new EducationLineRequirement()
                    }
                }
            };
            var entrantCharacterisitc = new Dictionary<string, double>
            {
                {"Русский", 0.5},
                {"Математика", 0.6},
                {"Информатика", 0.7}
            };

            var edLine1Characterisitc = new Dictionary<string, double>
            {
                {"Русский", 0.4},
                {"Математика", 0.6},
                {"Информатика", 0.8}
            };
            var edLine2Characterisitc = new Dictionary<string, double>
            {
                {"Русский", 0.7},
                {"Математика", 0.5},
                {"Информатика", 0.5}
            };

            entrantCharacterizer.Calculate(entrant).Returns(Task.FromResult(entrantCharacterisitc));
            educationLineCharacterizer.Calculate(educationLines[0]).Returns(Task.FromResult(edLine1Characterisitc));
            educationLineCharacterizer.Calculate(educationLines[1]).Returns(Task.FromResult(edLine2Characterisitc));

            var tetas = new Dictionary<string, double>();
            tetas.Add("Русский", 0.67);
            tetas.Add("Математика", 0.5);

            var prefRelation = new List<PreferenceRelation>
            {
                new PreferenceRelation("Информатика")
                {
                    Tetas = tetas
                }
            };
            preferenceRelationCalculator.GetPreferenceRelations(entrantCharacterisitc).Returns(prefRelation);

            //Act
            var multicriterialAnalysisRecomendator = new MulticriterialAnalysisRecomendator(entrantCharacterizer,
                educationLineCharacterizer, preferenceRelationCalculator);
            var taskResult = multicriterialAnalysisRecomendator.Calculate(entrant, educationLines);
            taskResult.Wait();
            var result = taskResult.Result;

            //Assert
            Assert.AreEqual(educationLines[0], result.Single());
        }
    }
}