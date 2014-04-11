using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.Logic.MulticriterialAnalysis;

namespace UnitTests
{
    [TestClass]
    public class MulticriterialAnalysisTest
    {
        [TestMethod]
        public void ParretoSetCreate_GetSomeSet_ReturnCorrectAnswer()
        {
            var y1 = new Dictionary<string,double>();
            y1.Add("1",4);
            y1.Add("2",0);
            y1.Add("3",3);
            y1.Add("4",2);

            var y2 = new Dictionary<string,double>();
            y2.Add("1", 5);
            y2.Add("2", 0);
            y2.Add("3", 2);
            y2.Add("4", 2);

            var y3 = new Dictionary<string,double>();
            y3.Add("1", 2);
            y3.Add("2", 1);
            y3.Add("3", 1);
            y3.Add("4", 3);

            var y4 = new Dictionary<string,double>();
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
            table.Add(new EducationLineAndClustersRow(1) {Clusters = y1});
            table.Add(new EducationLineAndClustersRow(2) {Clusters = y2});
            table.Add(new EducationLineAndClustersRow(3) {Clusters = y3});
            table.Add(new EducationLineAndClustersRow(4) {Clusters = y4});
            table.Add(new EducationLineAndClustersRow(5) {Clusters = y5});


            var answerTable = new List<EducationLineAndClustersRow>();
            answerTable.Add(table[0]);
            answerTable.Add(table[1]);
            answerTable.Add(table[4]);

            MulticriterialAnalysis multicriterialAnalysis = new MulticriterialAnalysis(null,null);
            var parretoTable = multicriterialAnalysis.ParretoSetCreate(table);

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
        }
    }
}
