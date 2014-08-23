using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.DAL.Models;
using OptimalEducation.Interfaces.Logic.MulticriterialAnalysis.Models;

namespace OptimalEducation.UnitTests.Logic.MulticriterialAnalysis
{
    [TestClass]
    public class MulticriterialAnalysisTest
    {
        //Важно помнить, что у нас может быть несколько методов по вычислению предпочтений
        [TestMethod]
        public void GetPreferenceRelations_ReturnCorrectAnswer()
        {
            #region Arrange
            var userCharacterisitcs = new Dictionary<string, double>();
            //2 группы
            //Важные(max-0.1 разница)
            userCharacterisitcs.Add("Математика", 0.8);
            userCharacterisitcs.Add("Физика", 0.7);
            //неважные
            userCharacterisitcs.Add("Литература", 0.4);
            userCharacterisitcs.Add("Русский", 0.3);
            userCharacterisitcs.Add("Химия", 0.5);
            userCharacterisitcs.Add("Биология", 0.6);
            userCharacterisitcs.Add("География", 0.6);
            userCharacterisitcs.Add("История", 0.5); 
            #endregion

            //#region Act
            //var preferenceCalculator = new PreferenceRelationCalculator();
            //var calculatedPreferences = preferenceCalculator.GetPreferenceRelations(userCharacterisitcs); 
            //#endregion

            //#region Asserts
            //Assert.IsTrue(calculatedPreferences.Count == 2);
            //Assert.IsTrue(calculatedPreferences.Exists(p => p.ImportantCharacterisicName == "Математика"));
            //Assert.IsTrue(calculatedPreferences.Exists(p => p.ImportantCharacterisicName == "Физика"));

            ////Не изменились
            //foreach (var preference in calculatedPreferences)
            //{
            //    Assert.IsTrue(preference.Tetas.Count == 6);
            //    //Корректно изменились (1-значение)
            //    Assert.AreEqual(preference.Tetas["Литература"], (1 - userCharacterisitcs["Литература"]));
            //    Assert.AreEqual(preference.Tetas["Русский"], (1 - userCharacterisitcs["Русский"]));
            //    Assert.AreEqual(preference.Tetas["Химия"], (1 - userCharacterisitcs["Химия"]));
            //    Assert.AreEqual(preference.Tetas["Биология"], (1 - userCharacterisitcs["Биология"]));
            //    Assert.AreEqual(preference.Tetas["География"], (1 - userCharacterisitcs["География"]));
            //    Assert.AreEqual(preference.Tetas["История"], (1 - userCharacterisitcs["История"]));
            //} 
            //#endregion
        }

        [TestMethod]
        public void RecalculateEducationLineCharacterisics_ReturnCorrectAnswer()
        {
            #region Arrange
            var characterisitcs1=new Dictionary<string,double>();
            characterisitcs1.Add("Математика", 0.9);    
            characterisitcs1.Add("Физика", 0.8);
            characterisitcs1.Add("Литература", 0.4);
            characterisitcs1.Add("Русский", 0.3);
            characterisitcs1.Add("Химия", 0.5);
            characterisitcs1.Add("Биология", 0.3);
            characterisitcs1.Add("География", 0.3);
            characterisitcs1.Add("История", 0.3);

            var characterisitcs2 = new Dictionary<string, double>();
            characterisitcs2.Add("Математика", 0.7);
            characterisitcs2.Add("Физика", 0.6);
            characterisitcs2.Add("Литература", 0.3);
            characterisitcs2.Add("Русский", 0.5);
            characterisitcs2.Add("Химия", 0.5);
            characterisitcs2.Add("Биология", 0.6);
            characterisitcs2.Add("География", 0.4);
            characterisitcs2.Add("История", 0.3);

            var characterisitcs3 = new Dictionary<string, double>();
            characterisitcs3.Add("Математика", 0.4);
            characterisitcs3.Add("Физика", 0.3);
            characterisitcs3.Add("Литература", 0.3);
            characterisitcs3.Add("Русский", 0.8);
            characterisitcs3.Add("Химия", 0.3);
            characterisitcs3.Add("Биология", 0.3);
            characterisitcs3.Add("География", 0.3);
            characterisitcs3.Add("История", 0.7);

            var educationLine1 = new EducationLine() { Id = 1 };
            var educationLine2 = new EducationLine() { Id = 2 };
            var educationLine3 = new EducationLine() { Id = 3 };

            var educationLineCharacterisics = new List<EducationLineWithCharacterisics>();
            educationLineCharacterisics.Add(new EducationLineWithCharacterisics(educationLine1, characterisitcs1));
            educationLineCharacterisics.Add(new EducationLineWithCharacterisics(educationLine2, characterisitcs2));
            educationLineCharacterisics.Add(new EducationLineWithCharacterisics(educationLine3, characterisitcs3));

            var tetasMath = new Dictionary<string, double>();
            tetasMath.Add("Литература", 0.6);
            tetasMath.Add("Русский", 0.7);
            tetasMath.Add("Химия", 0.5);
            tetasMath.Add("Биология", 0.4);
            tetasMath.Add("География", 0.4);
            tetasMath.Add("История", 0.5);
            var tetasPhys = new Dictionary<string, double>();
            tetasPhys.Add("Литература", 0.6);
            tetasPhys.Add("Русский", 0.7);
            tetasPhys.Add("Химия", 0.5);
            tetasPhys.Add("Биология", 0.4);
            tetasPhys.Add("География", 0.4);
            tetasPhys.Add("История", 0.5);

            var userPref = new List<PreferenceRelation>()
            {
                new PreferenceRelation("Математика"){Tetas=tetasMath},
                new PreferenceRelation("Физика"){Tetas=tetasPhys}
            };
            #endregion

            //#region Act
            //var vectorCriteriaRecalculator = new VectorCriteriaRecalculator();
            //var recalculatedEducationLineCharacterisics = vectorCriteriaRecalculator.RecalculateEducationLineCharacterisics(educationLineCharacterisics, userPref); 
            //#endregion

            //#region Asserts
            ////Проверка что значения важных кластеров(Математика и Физика) не изменились
            //var characterisics = recalculatedEducationLineCharacterisics[0];

            //Assert.IsTrue(characterisics.Characterisics["Математика"] == characterisitcs1["Математика"]);
            //Assert.IsTrue(characterisics.Characterisics["Физика"] == characterisitcs1["Физика"]);

            //Assert.IsTrue(recalculatedEducationLineCharacterisics[1].Characterisics["Математика"] == characterisitcs2["Математика"]);
            //Assert.IsTrue(recalculatedEducationLineCharacterisics[1].Characterisics["Физика"] == characterisitcs2["Физика"]);

            //Assert.IsTrue(recalculatedEducationLineCharacterisics[2].Characterisics["Математика"] == characterisitcs3["Математика"]);
            //Assert.IsTrue(recalculatedEducationLineCharacterisics[2].Characterisics["Физика"] == characterisitcs3["Физика"]);

            ////Проверка корректности вычислений
            ////Значения в виде диапазона(+ - 0.01). Т.к. он по ебанутому округляет(т.к. double тип)
            //Assert.IsTrue(characterisics.Characterisics["Литература(1)"] >= 0.69 && characterisics.Characterisics["Литература(1)"] < 0.71);
            //Assert.IsTrue(characterisics.Characterisics["Русский(1)"] >= 0.71 && characterisics.Characterisics["Русский(1)"]<0.73);
            //Assert.IsTrue(characterisics.Characterisics["Химия(1)"] >= 0.69 && characterisics.Characterisics["Химия(1)"] < 0.71);
            //Assert.IsTrue(characterisics.Characterisics["Биология(1)"] >= 0.53 && characterisics.Characterisics["Биология(1)"] < 0.55);
            //Assert.IsTrue(characterisics.Characterisics["География(1)"] >= 0.53 && characterisics.Characterisics["География(1)"] < 0.55);
            //Assert.IsTrue(characterisics.Characterisics["История(1)"] >= 0.59 && characterisics.Characterisics["История(1)"] < 0.61);

            //Assert.IsTrue(characterisics.Characterisics["Литература(2)"] >= 0.63 && characterisics.Characterisics["Литература(2)"] < 0.65);
            //Assert.IsTrue(characterisics.Characterisics["Русский(2)"] >= 0.64 && characterisics.Characterisics["Русский(2)"] < 0.66);
            //Assert.IsTrue(characterisics.Characterisics["Химия(2)"] >= 0.64 && characterisics.Characterisics["Химия(2)"] < 0.66);
            //Assert.IsTrue(characterisics.Characterisics["Биология(2)"] >= 0.49 && characterisics.Characterisics["Биология(2)"] < 0.51);
            //Assert.IsTrue(characterisics.Characterisics["География(2)"] >= 0.49 && characterisics.Characterisics["География(2)"] < 0.51);
            //Assert.IsTrue(characterisics.Characterisics["История(2)"] >= 0.54 && characterisics.Characterisics["История(2)"] < 0.56);

            //#endregion
        }

        [TestMethod]
        public void ParretoSetCreate_GetSomeSet_ReturnCorrectAnswer()
        {
            #region Arrange
            var y1 = new Dictionary<string, double>();
            y1.Add("1", 4);
            y1.Add("2", 0);
            y1.Add("3", 3);
            y1.Add("4", 2);

            var y2 = new Dictionary<string, double>();
            y2.Add("1", 5);
            y2.Add("2", 0);
            y2.Add("3", 2);
            y2.Add("4", 2);

            var y3 = new Dictionary<string, double>();
            y3.Add("1", 2);
            y3.Add("2", 1);
            y3.Add("3", 1);
            y3.Add("4", 3);

            var y4 = new Dictionary<string, double>();
            y4.Add("1", 5);
            y4.Add("2", 0);
            y4.Add("3", 1);
            y4.Add("4", 2);

            var y5 = new Dictionary<string, double>();
            y5.Add("1", 3);
            y5.Add("2", 1);
            y5.Add("3", 2);
            y5.Add("4", 3);


            var educationLine1 = new EducationLine() { Id = 1 };
            var educationLine2 = new EducationLine() { Id = 2 };
            var educationLine3 = new EducationLine() { Id = 3 };
            var educationLine4 = new EducationLine() { Id = 4 };
            var educationLine5 = new EducationLine() { Id = 5 };
            List<EducationLineWithCharacterisics> table = new List<EducationLineWithCharacterisics>();
            table.Add(new EducationLineWithCharacterisics(educationLine1,y1));
            table.Add(new EducationLineWithCharacterisics(educationLine2,y2));
            table.Add(new EducationLineWithCharacterisics(educationLine3,y3));
            table.Add(new EducationLineWithCharacterisics(educationLine4,y4));
            table.Add(new EducationLineWithCharacterisics(educationLine5,y5));


            var answerTable = new List<EducationLineWithCharacterisics>();
            answerTable.Add(table[0]);
            answerTable.Add(table[1]);
            answerTable.Add(table[4]); 
            #endregion

            //#region Act
            //ParretoCalculator parretoCalculator = new ParretoCalculator();
            //var parretoTable = parretoCalculator.ParretoSetCreate(table); 
            //#endregion

            //#region Asserts
            //if (parretoTable.Count == answerTable.Count)
            //    for (int i = 0; i < answerTable.Count; i++)
            //    {
            //        if (parretoTable[i] != answerTable[i])
            //        {
            //            Assert.Fail("Элемент таблицы не совпадает с ответом.");
            //            break;
            //        }
            //    }
            //else Assert.Fail("Не совпадает количество элеметнов в матрице ответа и полученной матрице."); 
            //#endregion
        }
    }
}
