using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.Logic.MulticriterialAnalysis;
using OptimalEducation.Logic.MulticriterialAnalysis.Models;

namespace UnitTests
{
    [TestClass]
    public class MulticriterialAnalysisTest
    {
        //Важно помнить, что у нас может быть несколько методов по вычислению предпочтений
        [TestMethod]
        public void GetPreferenceRelations_ReturnCorrectAnswer()
        {
            #region Arrange
            var userCluster = new Dictionary<string, double>();
            //2 группы
            //Важные(max-0.1 разница)
            userCluster.Add("Математика", 0.8);
            userCluster.Add("Физика", 0.7);
            //неважные
            userCluster.Add("Литература", 0.4);
            userCluster.Add("Русский", 0.3);
            userCluster.Add("Химия", 0.5);
            userCluster.Add("Биология", 0.6);
            userCluster.Add("География", 0.6);
            userCluster.Add("История", 0.5); 
            #endregion

            #region Act
            var preferenceCalculator = new PreferenceRelationCalculator(userCluster);
            var calculatedPreferences = preferenceCalculator.GetPreferenceRelations(); 
            #endregion

            #region Asserts
            Assert.IsTrue(calculatedPreferences.Count == 2);
            Assert.IsTrue(calculatedPreferences.Exists(p => p.ImportantClusterName == "Математика"));
            Assert.IsTrue(calculatedPreferences.Exists(p => p.ImportantClusterName == "Физика"));

            //Не изменились
            foreach (var preference in calculatedPreferences)
            {
                Assert.IsTrue(preference.Tetas.Count == 6);
                //Корректно изменились (1-значение)
                Assert.AreEqual(preference.Tetas["Литература"], (1 - userCluster["Литература"]));
                Assert.AreEqual(preference.Tetas["Русский"], (1 - userCluster["Русский"]));
                Assert.AreEqual(preference.Tetas["Химия"], (1 - userCluster["Химия"]));
                Assert.AreEqual(preference.Tetas["Биология"], (1 - userCluster["Биология"]));
                Assert.AreEqual(preference.Tetas["География"], (1 - userCluster["География"]));
                Assert.AreEqual(preference.Tetas["История"], (1 - userCluster["История"]));
            } 
            #endregion
        }

        [TestMethod]
        public void RecalculateEducationLineClusters_ReturnCorrectAnswer()
        {
            #region Arrange
            var clusters1=new Dictionary<string,double>();
            clusters1.Add("Математика", 0.9);    
            clusters1.Add("Физика", 0.8);
            clusters1.Add("Литература", 0.4);
            clusters1.Add("Русский", 0.3);
            clusters1.Add("Химия", 0.5);
            clusters1.Add("Биология", 0.3);
            clusters1.Add("География", 0.3);
            clusters1.Add("История", 0.3);

            var clusters2 = new Dictionary<string, double>();
            clusters2.Add("Математика", 0.7);
            clusters2.Add("Физика", 0.6);
            clusters2.Add("Литература", 0.3);
            clusters2.Add("Русский", 0.5);
            clusters2.Add("Химия", 0.5);
            clusters2.Add("Биология", 0.6);
            clusters2.Add("География", 0.4);
            clusters2.Add("История", 0.3);

            var clusters3 = new Dictionary<string, double>();
            clusters3.Add("Математика", 0.4);
            clusters3.Add("Физика", 0.3);
            clusters3.Add("Литература", 0.3);
            clusters3.Add("Русский", 0.8);
            clusters3.Add("Химия", 0.3);
            clusters3.Add("Биология", 0.3);
            clusters3.Add("География", 0.3);
            clusters3.Add("История", 0.7);

            var educationLineClusters = new List<EducationLineAndClustersRow>();
            educationLineClusters.Add(new EducationLineAndClustersRow(1) { Clusters = clusters1 });
            educationLineClusters.Add(new EducationLineAndClustersRow(2) { Clusters = clusters2 });
            educationLineClusters.Add(new EducationLineAndClustersRow(3) { Clusters = clusters3 });

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

            #region Act
            var vectorCriteriaRecalculator = new VectorCriteriaRecalculator();
            var recalculatedEducationLineClusters = vectorCriteriaRecalculator.RecalculateEducationLineClusters(educationLineClusters, userPref); 
            #endregion

            #region Asserts
            //Проверка что значения важных кластеров(Математика и Физика) не изменились
            var clusters = recalculatedEducationLineClusters[0];

            Assert.IsTrue(clusters.Clusters["Математика"] == clusters1["Математика"]);
            Assert.IsTrue(clusters.Clusters["Физика"] == clusters1["Физика"]);

            Assert.IsTrue(recalculatedEducationLineClusters[1].Clusters["Математика"] == clusters2["Математика"]);
            Assert.IsTrue(recalculatedEducationLineClusters[1].Clusters["Физика"] == clusters2["Физика"]);

            Assert.IsTrue(recalculatedEducationLineClusters[2].Clusters["Математика"] == clusters3["Математика"]);
            Assert.IsTrue(recalculatedEducationLineClusters[2].Clusters["Физика"] == clusters3["Физика"]);

            //Проверка корректности вычислений
            //Значения в виде диапазона(+ - 0.01). Т.к. он по ебанутому округляет
            Assert.IsTrue(clusters.Clusters["Литература(1)"] >= 0.69 && clusters.Clusters["Литература(1)"] < 0.71);
            Assert.IsTrue(clusters.Clusters["Русский(1)"] >= 0.71 && clusters.Clusters["Русский(1)"]<0.73);
            Assert.IsTrue(clusters.Clusters["Химия(1)"] >= 0.69 && clusters.Clusters["Химия(1)"] < 0.71);
            Assert.IsTrue(clusters.Clusters["Биология(1)"] >= 0.53 && clusters.Clusters["Биология(1)"] < 0.55);
            Assert.IsTrue(clusters.Clusters["География(1)"] >= 0.53 && clusters.Clusters["География(1)"] < 0.55);
            Assert.IsTrue(clusters.Clusters["История(1)"] >= 0.59 && clusters.Clusters["История(1)"] < 0.61);

            Assert.IsTrue(clusters.Clusters["Литература(2)"] >= 0.63 && clusters.Clusters["Литература(2)"] < 0.65);
            Assert.IsTrue(clusters.Clusters["Русский(2)"] >= 0.64 && clusters.Clusters["Русский(2)"] < 0.66);
            Assert.IsTrue(clusters.Clusters["Химия(2)"] >= 0.64 && clusters.Clusters["Химия(2)"] < 0.66);
            Assert.IsTrue(clusters.Clusters["Биология(2)"] >= 0.49 && clusters.Clusters["Биология(2)"] < 0.51);
            Assert.IsTrue(clusters.Clusters["География(2)"] >= 0.49 && clusters.Clusters["География(2)"] < 0.51);
            Assert.IsTrue(clusters.Clusters["История(2)"] >= 0.54 && clusters.Clusters["История(2)"] < 0.56);

            #endregion
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

            List<EducationLineAndClustersRow> table = new List<EducationLineAndClustersRow>();
            table.Add(new EducationLineAndClustersRow(1) { Clusters = y1 });
            table.Add(new EducationLineAndClustersRow(2) { Clusters = y2 });
            table.Add(new EducationLineAndClustersRow(3) { Clusters = y3 });
            table.Add(new EducationLineAndClustersRow(4) { Clusters = y4 });
            table.Add(new EducationLineAndClustersRow(5) { Clusters = y5 });


            var answerTable = new List<EducationLineAndClustersRow>();
            answerTable.Add(table[0]);
            answerTable.Add(table[1]);
            answerTable.Add(table[4]); 
            #endregion

            #region Act
            ParretoCalculator parretoCalculator = new ParretoCalculator();
            var parretoTable = parretoCalculator.ParretoSetCreate(table); 
            #endregion

            #region Asserts
            if (parretoTable.Count == answerTable.Count)
                for (int i = 0; i < answerTable.Count; i++)
                {
                    if (parretoTable[i] != answerTable[i])
                    {
                        Assert.Fail("Элемент таблицы не совпадает с ответом.");
                        break;
                    }
                }
            else Assert.Fail("Не совпадает количество элеметнов в матрице ответа и полученной матрице."); 
            #endregion
        }
    }
}
