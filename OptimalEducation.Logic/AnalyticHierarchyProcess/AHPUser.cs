﻿using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Clusterizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptimalEducation.Logic.AnalyticHierarchyProcess
{
    /// <summary>
    /// Расчет приоритетности направлений с помощью метода анализа иерарохий относительно пользователя
    /// </summary>
    public class AHPUser
    {
        Entrant _entrant;
        OptimalEducationDbContext context = new OptimalEducationDbContext();
        int totalEducationLines = 0;

        #region Переменные для первого критерия - сложности на основе ЕГЭ
        List<FirstCriterionUnit> FirstCriterionContainer = new List<FirstCriterionUnit>();
        int firstCriterionMatrixSize = 0;
        class FirstCriterionUnit
        {
            public int databaseId;
            public bool firstCriterionAcceptable;
            public int matrixId;
            public int requiredSum;
            public int entrantSum;
            public int weakenedDifficulty;
            public double localPriority;
        }
        #endregion


        #region Переменные для второго критерия - схожести интересов (кластеров)
        List<SecondCriterionUnit> SecondCriterionContainer = new List<SecondCriterionUnit>();
        Dictionary<string, double> entruntClusters = new Dictionary<string, double>();
        double maxEntrantClusterSum = 0;
        int secondCriterionMatrixSize = 0;

        class SecondCriterionUnit
        {
            public int databaseId;
            public bool secondCriterionAcceptable;
            public int matrixId;

            public Dictionary<string, double> educationLineClusters = new Dictionary<string, double>();

            public double localPriority;
        }
        #endregion


        #region Переменные и классы для сложения критериев в конечную оценку
        public List<TotalResultUnit> AllCriterionContainer = new List<TotalResultUnit>();
        public class TotalResultUnit
        {
            public int databaseId;
            public double firstCriterionFinalPriority = 0;
            public double secondCriterionFinalPriority = 0;
            public double thirdCriterionFinalPriority = 0;

            public double absolutePriority = 0;
        }
        #endregion


        #region Общие настройки метода и приоритеты критериев

        double firstCriterionPriority = 1;
        double secondCriterionPriority = 0;
        double thirdCriterionPriority = 0;

        int firstCriterionLazyGap = 10;

        #endregion


        public AHPUser(int userID, Dictionary<string, double> settings)
        {
            _entrant = context.Entrants.Find(userID);

            if (settings.ContainsKey("firstCriterionLazyGap")) firstCriterionLazyGap = Convert.ToInt32(settings["firstCriterionLazyGap"]);
            if (settings.ContainsKey("firstCriterionPriority")) firstCriterionPriority = settings["firstCriterionPriority"];
            if (settings.ContainsKey("secondCriterionPriority")) secondCriterionPriority = settings["secondCriterionPriority"];
            if (settings.ContainsKey("thirdCriterionPriority")) thirdCriterionPriority = settings["thirdCriterionPriority"];

            //Console.WriteLine(_entrant.FirstName + " " + _entrant.LastName);
            CalculateAll();
        }


        private void CalculateAll()
        {
            InitialiseFirstCriterion();
            CalculateFirstCriterion();
            InitialiseSecondCriterion();
            CalculateSecondCriterion();

            FinalCalculate();
        }


        //Критерий трудности по ЕГЭ - заполнение направлений во временный список
        private void InitialiseFirstCriterion()
        {
            int totalAvailLines = 0;

            foreach (EducationLine EdLine in context.EducationLines)
            {
                int reqExamSum = 0;
                int entrExamSum = 0;
                bool edLineAcceptable = true;

                //Console.WriteLine(EdLine.Name + " REQUARES " + EdLine.RequiredSum.ToString());
                foreach (EducationLineRequirement EdLineReq in EdLine.EducationLinesRequirements)
                {
                    bool foundResult = false;
                    //Console.WriteLine("****** " + EdLineReq.ExamDiscipline.Name);
                    foreach (UnitedStateExam EntrExam in _entrant.UnitedStateExams)
                    {
                        if (EntrExam.ExamDisciplineId == EdLineReq.ExamDisciplineId)
                        {
                            //Console.WriteLine("****** Entrant has " + EntrExam.Result.ToString());
                            foundResult = true;
                            entrExamSum = entrExamSum + Convert.ToInt32(EntrExam.Result);
                            break;
                        }
                    }

                    if (foundResult == false)
                    {
                        edLineAcceptable = false;
                        //Console.WriteLine("****** Entrant has no such exam");
                        break;
                    }
                }
                if (edLineAcceptable == false)
                {
                    //Console.WriteLine("====== NOT ACCEPTABLE EXAMS");

                    FirstCriterionUnit EducationLine = new FirstCriterionUnit();
                    EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                    EducationLine.firstCriterionAcceptable = false;
                    EducationLine.requiredSum = Convert.ToInt32(EdLine.RequiredSum);
                    EducationLine.localPriority = 0;

                    FirstCriterionContainer.Add(EducationLine);
                }
                else
                {
                    reqExamSum = Convert.ToInt32(EdLine.RequiredSum);
                    //Console.WriteLine("====== ENTRANT HAS TOTAL OF " + entrExamSum.ToString());

                    FirstCriterionUnit EducationLine = new FirstCriterionUnit();
                    EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                    EducationLine.firstCriterionAcceptable = true;
                    EducationLine.matrixId = totalAvailLines;
                    EducationLine.requiredSum = Convert.ToInt32(EdLine.RequiredSum);
                    EducationLine.entrantSum = entrExamSum;
                    EducationLine.weakenedDifficulty = WeakenedDifficultyResult(entrExamSum, Convert.ToInt32(EdLine.RequiredSum));
                    EducationLine.localPriority = 0;

                    //Console.WriteLine(">>>>>>>> WEAK DIFF: " + EducationLine.weakenedDifficulty);

                    totalAvailLines++;

                    FirstCriterionContainer.Add(EducationLine);
                }
            }
            firstCriterionMatrixSize = totalAvailLines;
            //Console.WriteLine("TOTAL LINES TO GO: " + totalAvailLines.ToString());
            //Console.WriteLine("TOTAL LINES IN CONTAINER: " + FirstCriterionContainer.Count.ToString());
            if (FirstCriterionContainer.Count > totalEducationLines) totalEducationLines = FirstCriterionContainer.Count;
        }


        //Критерий трудности по ЕГЭ - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateFirstCriterion()
        {
            //Console.WriteLine();
            //Console.WriteLine("======================================================================");
            //Console.WriteLine("======================================================================");
            //Console.WriteLine();

            double[,] pairwiseComparisonMatrix = new double[firstCriterionMatrixSize, firstCriterionMatrixSize];

            for (int i = 0; i < firstCriterionMatrixSize; i++)
            {
                for (int j = 0; j < firstCriterionMatrixSize; j++)
                {
                    int a = FirstCriterionContainer.Find(x => x.matrixId == i).weakenedDifficulty;
                    int b = FirstCriterionContainer.Find(y => y.matrixId == j).weakenedDifficulty;
                    pairwiseComparisonMatrix[i, j] = FirstCriterionCompare(a, b);
                }
            }
            //Console.WriteLine();
            //Console.WriteLine("Matrix size: " + firstCriterionMatrixSize);

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[0, 0].ToString() + "          " + pairwiseComparisonMatrix[0, 1].ToString() + "          " + pairwiseComparisonMatrix[0, 2].ToString() + "          " + pairwiseComparisonMatrix[0, 3].ToString());

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[1, 0].ToString() + "          " + pairwiseComparisonMatrix[1, 1].ToString() + "          " + pairwiseComparisonMatrix[1, 2].ToString() + "          " + pairwiseComparisonMatrix[1, 3].ToString());

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[2, 0].ToString() + "          " + pairwiseComparisonMatrix[2, 1].ToString() + "          " + pairwiseComparisonMatrix[2, 2].ToString() + "          " + pairwiseComparisonMatrix[2, 3].ToString());

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[3, 0].ToString() + "          " + pairwiseComparisonMatrix[3, 1].ToString() + "          " + pairwiseComparisonMatrix[3, 2].ToString() + "          " + pairwiseComparisonMatrix[3, 3].ToString());

            //Console.WriteLine();

            double[] resultVector = CalcEigenvectors(pairwiseComparisonMatrix, firstCriterionMatrixSize);

            //Console.WriteLine();

            for (int i = 0; i < firstCriterionMatrixSize; i++)
            {
                FirstCriterionContainer.Find(x => x.matrixId == i).localPriority = resultVector[i];
            }
            //Первый критерий закончил рачет приоритетов (локальных)

        }


        //Расчет "ослабленного" результата направления
        private int WeakenedDifficultyResult(int entrantSum, int requiredSum)
        {
            int weakResEntr = (entrantSum - firstCriterionLazyGap);
            if (weakResEntr < 0) weakResEntr = 0;
            int weakResult = weakResEntr - Math.Abs(weakResEntr - requiredSum);
            if (weakResult < 0) weakResult = 0;
            return weakResult;
        }


        //Сравнение результатов 2 направлений (1 критерий)
        private double FirstCriterionCompare(int firstNum, int secondNum)
        {
            if ((firstNum <= 0) && (secondNum <= 0)) return 1;
            else if (firstNum <= 0) return (1 / Convert.ToDouble(secondNum));
            else if (secondNum <= 0) return firstNum;
            else return Convert.ToDouble(firstNum) / Convert.ToDouble(secondNum);
        }


        //Нахождение собственного вектора через приближенные оценки
        private double[] CalcEigenvectors(double[,] matrix, int martixSize)
        {
            double[] resultVector = new double[martixSize];

            for (int i = 0; i < martixSize; i++)
            {
                double lineProduction = 1;

                for (int j = 0; j < martixSize; j++)
                {
                    lineProduction = lineProduction * matrix[i, j];
                }
                resultVector[i] = Math.Pow(lineProduction, 1 / Convert.ToDouble(martixSize));
            }

            double vectorCompSum = 0;
            for (int i = 0; i < martixSize; i++)
            {
                vectorCompSum = vectorCompSum + resultVector[i];
            }
            //Console.WriteLine("> Eigenvector:");
            for (int i = 0; i < martixSize; i++)
            {
                resultVector[i] = resultVector[i] / vectorCompSum;
                //Console.WriteLine(resultVector[i].ToString());
            }
            return resultVector;
        }


        //Критерий совпадения кластеров - заполнение направлений во временный список
        private void InitialiseSecondCriterion()
        {
            int totalAvailLines = 0;
            EntrantClusterizer EntrClusterizer = new EntrantClusterizer(_entrant);

            maxEntrantClusterSum = EntrClusterizer.Cluster.Values.Max();
            entruntClusters = EntrClusterizer.Cluster;
            
            foreach (EducationLine EdLine in context.EducationLines)
            {
                bool edLineAcceptable = true;

                EducationLineClusterizer EdLineClusterizer = new EducationLineClusterizer(EdLine);
                if (EdLineClusterizer.Cluster.Count() <= 0) edLineAcceptable = false;


                //Console.WriteLine(">EDLINE: " + EdLine.Name.ToString());
                foreach (var item in EdLineClusterizer.Cluster) 
                {

                    //Console.WriteLine("cluster " + item.Key.ToString() + " has " + item.Value.ToString());
                    if (!entruntClusters.ContainsKey(item.Key))
                    {
                        edLineAcceptable = false;
                    }
                }

                if (edLineAcceptable == false)
                {
                    //Console.WriteLine("====== NOT ACCEPTABLE CLUSTERS");

                    SecondCriterionUnit EducationLine = new SecondCriterionUnit();
                    EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                    EducationLine.secondCriterionAcceptable = false;
                    EducationLine.localPriority = 0;

                    SecondCriterionContainer.Add(EducationLine);
                }
                else
                {
                    //Console.WriteLine("====== ENTRANT HAS THIS CLUSTERS");

                    SecondCriterionUnit EducationLine = new SecondCriterionUnit();
                    EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                    EducationLine.secondCriterionAcceptable = true;
                    EducationLine.matrixId = totalAvailLines;
                    EducationLine.educationLineClusters = EdLineClusterizer.Cluster;
                    EducationLine.localPriority = 0;
                    //Console.WriteLine("====== MAX EDLINE CLUSTER SUM: " + EdLineClusterizer.Cluster.Values.Max());

                    totalAvailLines++;

                    SecondCriterionContainer.Add(EducationLine);
                }
            }
            secondCriterionMatrixSize = totalAvailLines;

            //Console.WriteLine("MAX ENTRUNT CLUSTER SUM: " + maxEntrantClusterSum.ToString());

            //Console.WriteLine("TOTAL LINES TO GO: " + totalAvailLines.ToString());
            //Console.WriteLine("TOTAL LINES IN CONTAINER: " + FirstCriterionContainer.Count.ToString());

            if (SecondCriterionContainer.Count > totalEducationLines) totalEducationLines = SecondCriterionContainer.Count;
        }


        //Критерий совпадения кластеров - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateSecondCriterion()
        {
            //Console.WriteLine();
            //Console.WriteLine("======================================================================");
            //Console.WriteLine("======================================================================");
            //Console.WriteLine();

            double[,] pairwiseComparisonMatrix = new double[secondCriterionMatrixSize, secondCriterionMatrixSize];

            for (int i = 0; i < secondCriterionMatrixSize; i++)
            {
                for (int j = 0; j < secondCriterionMatrixSize; j++)
                {
                    double a = GetSecondCriterionEdLineValue(SecondCriterionContainer.Find(x => x.matrixId == i).educationLineClusters);
                    double b = GetSecondCriterionEdLineValue(SecondCriterionContainer.Find(y => y.matrixId == j).educationLineClusters);
                    pairwiseComparisonMatrix[i, j] = SecondCriterionCompare(a, b);

                    //Console.WriteLine("???? compare result: " + pairwiseComparisonMatrix[i, j].ToString());
                }
            }
            //Console.WriteLine();
            //Console.WriteLine("Matrix size: " + secondCriterionMatrixSize);

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[0, 0].ToString() + "          " + pairwiseComparisonMatrix[0, 1].ToString() + "          " + pairwiseComparisonMatrix[0, 2].ToString() + "          " + pairwiseComparisonMatrix[0, 3].ToString());

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[1, 0].ToString() + "          " + pairwiseComparisonMatrix[1, 1].ToString() + "          " + pairwiseComparisonMatrix[1, 2].ToString() + "          " + pairwiseComparisonMatrix[1, 3].ToString());

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[2, 0].ToString() + "          " + pairwiseComparisonMatrix[2, 1].ToString() + "          " + pairwiseComparisonMatrix[2, 2].ToString() + "          " + pairwiseComparisonMatrix[2, 3].ToString());

            //Console.WriteLine();
            //Console.Write(pairwiseComparisonMatrix[3, 0].ToString() + "          " + pairwiseComparisonMatrix[3, 1].ToString() + "          " + pairwiseComparisonMatrix[3, 2].ToString() + "          " + pairwiseComparisonMatrix[3, 3].ToString());

            //Console.WriteLine();

            double[] resultVector = CalcEigenvectors(pairwiseComparisonMatrix, secondCriterionMatrixSize);

            //Console.WriteLine();

            for (int i = 0; i < secondCriterionMatrixSize; i++)
            {
                SecondCriterionContainer.Find(x => x.matrixId == i).localPriority = resultVector[i];
            }
            //Второй критерий закончил рачет приоритетов (локальных)

        }


        //Подсчет значений для сравнения в таблице
        private double GetSecondCriterionEdLineValue(Dictionary<string, double> EdLineClusters)
        {
            double difference = 0;
            int clustersCount = 0;

            //Console.WriteLine("++++++++++++++++++++++");
            foreach (var item in EdLineClusters)
            {
                double normalizedEdLineValue;
                if (EdLineClusters.Values.Max() > 0) normalizedEdLineValue = item.Value / EdLineClusters.Values.Max();
                else normalizedEdLineValue = 0;
                double normalizedEntrantValuse = entruntClusters[item.Key] / maxEntrantClusterSum;

                difference =+ Math.Abs(normalizedEdLineValue - normalizedEntrantValuse);

                //Console.WriteLine("EDLINE: " + normalizedEdLineValue.ToString());
                //Console.WriteLine("ENTRANT " + normalizedEntrantValuse.ToString());
                clustersCount++;
            }
            //Console.WriteLine("++++++++++++++++++++++");

            return Math.Abs((Convert.ToDouble(clustersCount) - difference)) / Convert.ToDouble(clustersCount);
        }


        //Сравнение результатов 2 направлений 2 критерий
        private double SecondCriterionCompare(double firstNum, double secondNum)
        {
            if ((firstNum <= 0) && (secondNum <= 0)) return 1;
            else if (firstNum <= 0) return (1 / secondNum);
            else if (secondNum <= 0) return firstNum;
            else return firstNum / secondNum;
        }




        //Расчет конечных приоритетов и сортировка
        private void FinalCalculate()
        {
            //Пролистываем результатты первого критерия и добавляем в общий список (учитывая приогритет критерия)
            for (int i = 0; i < FirstCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.databaseId == FirstCriterionContainer[i].databaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    TotalResultUnit EducationLineFinal = new TotalResultUnit();
                    EducationLineFinal.databaseId = FirstCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EducationLineFinal);
                }

                AllCriterionContainer.Find(x => x.databaseId == FirstCriterionContainer[i].databaseId).firstCriterionFinalPriority =
                    FirstCriterionContainer[i].localPriority * firstCriterionPriority;
            }
            //Потом тоже самое для остальных критериев
            for (int i = 0; i < SecondCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.databaseId == SecondCriterionContainer[i].databaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    TotalResultUnit EducationLineFinal = new TotalResultUnit();
                    EducationLineFinal.databaseId = SecondCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EducationLineFinal);
                }

                AllCriterionContainer.Find(x => x.databaseId == SecondCriterionContainer[i].databaseId).secondCriterionFinalPriority =
                    SecondCriterionContainer[i].localPriority * secondCriterionPriority;
            }

            //Завершающее сложение и заполнение выходного словарая
            foreach (TotalResultUnit EdLineFinal in AllCriterionContainer)
            {
                EdLineFinal.absolutePriority = EdLineFinal.firstCriterionFinalPriority + EdLineFinal.secondCriterionFinalPriority + EdLineFinal.thirdCriterionFinalPriority;
            }

            //Сортировка
            AllCriterionContainer.Sort((x, y) => y.absolutePriority.CompareTo(x.absolutePriority));

        }


    }
}