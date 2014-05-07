using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
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
        Dictionary<string, double> entrantCharacteristics = new Dictionary<string, double>();
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


        #region Переменные для тертьего критерия - престиж (универов и направлений)
        List<ThirdCriterionUnit> ThirdCriterionContainer = new List<ThirdCriterionUnit>();

        int thirdCriterionMatrixSize = 0;
        double HEIprestigePart = 0.7;

        class ThirdCriterionUnit
        {
            public int databaseId;
            public int matrixId;

            public int facultyPrestige;
            public int HEIPrestige;

            public double localPriorityFaculty;
            public double localPriorityHEI;
        }
        #endregion


        #region Переменные для 4 критерия - расстояния до заданного города
        List<FourthCriterionUnit> FourthCriterionContainer = new List<FourthCriterionUnit>();

        int fourthCriterionMatrixSize = 0;

        class FourthCriterionUnit
        {
            public int databaseId;
            public int matrixId;

            public bool hasCoordinates;
            public double distance;

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
            public double fourthCriterionFinalPriority = 0;

            public double absolutePriority = 0;
        }
        #endregion


        //Общие настройки метода и приоритеты критериев
        AHPUserSettings _settings;

        public AHPUser(int userID, AHPUserSettings settings)
        {
            _entrant = context.Entrants.Find(userID);
            _settings = settings;

            CalculateAll();
        }

        public AHPUser(int userID)
        {
            _entrant = context.Entrants.Find(userID);
            _settings = new AHPUserSettings();

            //Console.WriteLine(_entrant.FirstName + " " + _entrant.LastName);
            CalculateAll();
        }



        private void CalculateAll()
        {
            if (_settings.FirstCriterionPriority > 0)
            {
                InitialiseFirstCriterion();
                CalculateFirstCriterion();
            }

            if (_settings.SecondCriterionPriority > 0)
            {
                InitialiseSecondCriterion();
                CalculateSecondCriterion();
            }

            if (_settings.ThirdCriterionPriority > 0)
            {
                InitialiseThirdCriterion();
                CalculateThirdCriterion();
            }

            if (_settings.FourthCriterionPriority > 0)
            {
                InitialiseFourthCriterion();
                CalculateFourthCriterion();
            }

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
            int weakResEntr = (entrantSum - _settings.FirstCriterionLazyGap);
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

            entrantCharacteristics = new EntrantCharacterizer(_entrant,new EntrantCalculationOptions()).CalculateNormSum();
            maxEntrantClusterSum = entrantCharacteristics.Values.Max();
            
            foreach (EducationLine EdLine in context.EducationLines)
            {
                bool edLineAcceptable = true;

                var edLineClusterizer = new EducationLineCharacterizer(EdLine, new EducationLineCalculationOptions());
                var edLineResult = edLineClusterizer.CalculateNormSum();

                if (edLineResult.Count() <= 0) edLineAcceptable = false;


                //Console.WriteLine(">EDLINE: " + EdLine.Name.ToString());
                foreach (var item in edLineResult) 
                {

                    //Console.WriteLine("cluster " + item.Key.ToString() + " has " + item.Value.ToString());
                    if (!entrantCharacteristics.ContainsKey(item.Key))
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
                    EducationLine.educationLineClusters = edLineResult;
                    EducationLine.localPriority = 0;
                    //Console.WriteLine("====== MAX EDLINE CLUSTER SUM: " + EdLineClusterizer.Characterisic.Values.Max());

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


        //Подсчет значений для сравнения в таблице 2 критерий
        private double GetSecondCriterionEdLineValue(Dictionary<string, double> EdLineClusters)
        {
            double difference = 0;
            int clustersCount = 0;

            //Console.WriteLine("++++++++++++++++++++++");
            foreach (var item in EdLineClusters)
            {
                if (!entrantCharacteristics.ContainsKey(item.Key)) continue;

                double normalizedEdLineValue;
                if (EdLineClusters.Values.Max() > 0) normalizedEdLineValue = item.Value / EdLineClusters.Values.Max();
                else normalizedEdLineValue = 0;
                double normalizedEntrantValuse = entrantCharacteristics[item.Key] / maxEntrantClusterSum;

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


        //Критерий престижа - заполнение направлений во временный список
        private void InitialiseThirdCriterion()
        {
            int counter = 0;

            foreach (EducationLine EdLine in context.EducationLines)
            {

                ThirdCriterionUnit EducationLine = new ThirdCriterionUnit();
                EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                EducationLine.matrixId = counter;

                if (EdLine.Faculty.Prestige > 0) EducationLine.facultyPrestige = EdLine.Faculty.Prestige;
                else EducationLine.facultyPrestige = 0;

                if (EdLine.Faculty.HigherEducationInstitution.Prestige > 0) EducationLine.HEIPrestige = EdLine.Faculty.HigherEducationInstitution.Prestige;
                else EducationLine.HEIPrestige = 0;

                EducationLine.localPriorityFaculty = 0;
                EducationLine.localPriorityHEI = 0;

                counter++;

                ThirdCriterionContainer.Add(EducationLine);                
            }
            thirdCriterionMatrixSize = counter;
        }


        //Критерий престижа - расчет притетов для всех направлений
        private void CalculateThirdCriterion()
        {

            double[,] pairwiseComparisonMatrix = new double[thirdCriterionMatrixSize, thirdCriterionMatrixSize];

            for (int i = 0; i < thirdCriterionMatrixSize; i++)
            {
                for (int j = 0; j < thirdCriterionMatrixSize; j++)
                {
                    double a = GetThirdCriterionEdLineValue(ThirdCriterionContainer.Find(x => x.matrixId == i).facultyPrestige);
                    double b = GetThirdCriterionEdLineValue(ThirdCriterionContainer.Find(y => y.matrixId == j).facultyPrestige);
                    
                    pairwiseComparisonMatrix[i, j] = a/b;

                    //Console.WriteLine("???? compare result: " + pairwiseComparisonMatrix[i, j].ToString());
                }
            }

            double[] resultVector1 = CalcEigenvectors(pairwiseComparisonMatrix, thirdCriterionMatrixSize);

            for (int i = 0; i < thirdCriterionMatrixSize; i++)
            {
                ThirdCriterionContainer.Find(x => x.matrixId == i).localPriorityFaculty = resultVector1[i];
            }

            for (int i = 0; i < thirdCriterionMatrixSize; i++)
            {
                for (int j = 0; j < thirdCriterionMatrixSize; j++)
                {
                    double a = GetThirdCriterionEdLineValue(ThirdCriterionContainer.Find(x => x.matrixId == i).HEIPrestige);
                    double b = GetThirdCriterionEdLineValue(ThirdCriterionContainer.Find(y => y.matrixId == j).HEIPrestige);

                    pairwiseComparisonMatrix[i, j] = a / b;

                    //Console.WriteLine("???? compare result: " + pairwiseComparisonMatrix[i, j].ToString());
                }
            }

            double[] resultVector2 = CalcEigenvectors(pairwiseComparisonMatrix, thirdCriterionMatrixSize);

            for (int i = 0; i < thirdCriterionMatrixSize; i++)
            {
                ThirdCriterionContainer.Find(x => x.matrixId == i).localPriorityHEI = resultVector2[i];
            }
            
            //Тертий критерий закончил рачет приоритетов (локальных)

        }


        //Подсчет значений для сравнения в таблице 3 критерий (сильно упростился, но вообще убирать лень)
        private double GetThirdCriterionEdLineValue(int prestige)
        {
            if (prestige <= 0) return 1;
            else return Convert.ToDouble(prestige);
        }


        //Критерий расстояния - заполнение направлений во временный список
        private void InitialiseFourthCriterion()
        {
            int counter = 0;

            Console.WriteLine(_settings.FourthCriterionExactLocation.ToString());

            foreach (EducationLine EdLine in context.EducationLines)
            {

                if (_settings.FourthCriterionExactLocation == true)
                {
                    FourthCriterionUnit EducationLine = new FourthCriterionUnit();
                    EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                    EducationLine.matrixId = counter;
                    EducationLine.hasCoordinates = false;
                    EducationLine.localPriority = 0;

                    if (EdLine.Faculty.HigherEducationInstitution.City.Id == _settings.FourthCriterionCityID) EducationLine.distance = 0;
                    else EducationLine.distance = 9.0;
                    
                    counter++;

                    FourthCriterionContainer.Add(EducationLine);
                }
                else
                {

                    var subjectLocation = context.Cities.Find(_settings.FourthCriterionCityID).Location;

                    if (EdLine.Faculty.HigherEducationInstitution.City.Location == null)
                    {
                        FourthCriterionUnit EducationLine = new FourthCriterionUnit();
                        EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                        EducationLine.hasCoordinates = false;                        
                        EducationLine.localPriority = 0;
                        FourthCriterionContainer.Add(EducationLine);
                    }
                    else
                    {
                        FourthCriterionUnit EducationLine = new FourthCriterionUnit();
                        EducationLine.databaseId = Convert.ToInt32(EdLine.Id);
                        EducationLine.matrixId = counter;
                        EducationLine.hasCoordinates = true;
                        EducationLine.localPriority = 0;

                        var universityLocation = EdLine.Faculty.HigherEducationInstitution.City.Location;

                        EducationLine.distance = Convert.ToDouble(subjectLocation.Distance(universityLocation));

                        Console.WriteLine("dist: " + EducationLine.distance.ToString());

                        counter++;

                        FourthCriterionContainer.Add(EducationLine);
                    }
                }

            }
            fourthCriterionMatrixSize = counter;
        }


        //Критерий расстояний - расчеты приоритетов для всех подходящих направлений
        private void CalculateFourthCriterion()
        {

            double[,] pairwiseComparisonMatrix = new double[fourthCriterionMatrixSize, fourthCriterionMatrixSize];

            for (int i = 0; i < fourthCriterionMatrixSize; i++)
            {
                for (int j = 0; j < fourthCriterionMatrixSize; j++)
                {

                    double a = FourthCriterionContainer.Find(x => x.matrixId == i).distance;
                    double b = FourthCriterionContainer.Find(y => y.matrixId == j).distance;

                    pairwiseComparisonMatrix[i, j] = FourthCriterionCompare(a, b);
                    
                    //Console.WriteLine("???? compare result: " + pairwiseComparisonMatrix[i, j].ToString());
                }
            }

            double[] resultVector = CalcEigenvectors(pairwiseComparisonMatrix, fourthCriterionMatrixSize);


            for (int i = 0; i < fourthCriterionMatrixSize; i++)
            {
                FourthCriterionContainer.Find(x => x.matrixId == i).localPriority = resultVector[i];
            }
            //4 критерий закончил рачет приоритетов (локальных)

        }


        //Сравнение дистанции 
        private double FourthCriterionCompare(double firstDist, double secondDist)
        {
            if ((firstDist <= 0) && (secondDist <= 0)) return 1;
            else if (firstDist <= 0) return 9.0;
            else if (secondDist <= 0) return 1.0 / 9.0;
            else return secondDist / firstDist;
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
                    FirstCriterionContainer[i].localPriority * _settings.FirstCriterionPriority;
            }
            //Потом тоже самое для 2 критерия
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
                    SecondCriterionContainer[i].localPriority * _settings.SecondCriterionPriority;
            }
            //Потом тоже самое для 3-го
            for (int i = 0; i < ThirdCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.databaseId == ThirdCriterionContainer[i].databaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    TotalResultUnit EducationLineFinal = new TotalResultUnit();
                    EducationLineFinal.databaseId = ThirdCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EducationLineFinal);
                }

                AllCriterionContainer.Find(x => x.databaseId == ThirdCriterionContainer[i].databaseId).thirdCriterionFinalPriority =
                    ThirdCriterionContainer[i].localPriorityFaculty * _settings.ThirdCriterionPriority * (1 - HEIprestigePart) +
                        ThirdCriterionContainer[i].localPriorityHEI * _settings.ThirdCriterionPriority * HEIprestigePart;
            }
            //Потом тоже самое для 4 критерия
            for (int i = 0; i < FourthCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.databaseId == FourthCriterionContainer[i].databaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    TotalResultUnit EducationLineFinal = new TotalResultUnit();
                    EducationLineFinal.databaseId = FourthCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EducationLineFinal);
                }

                AllCriterionContainer.Find(x => x.databaseId == FourthCriterionContainer[i].databaseId).fourthCriterionFinalPriority =
                    FourthCriterionContainer[i].localPriority * _settings.FourthCriterionPriority;
            }

            //Завершающее сложение и заполнение выходного словарая
            foreach (TotalResultUnit EdLineFinal in AllCriterionContainer)
            {
                EdLineFinal.absolutePriority = EdLineFinal.firstCriterionFinalPriority + EdLineFinal.secondCriterionFinalPriority + EdLineFinal.thirdCriterionFinalPriority + EdLineFinal.fourthCriterionFinalPriority;
            }

            //Сортировка
            AllCriterionContainer.Sort((x, y) => y.absolutePriority.CompareTo(x.absolutePriority));

        }


    }
    public class AHPUserSettings
    {
        #region Fields
        OptimalEducationDbContext context = new OptimalEducationDbContext();

        double firstCriterionPriority = 0.40;
        double secondCriterionPriority = 0.35;
        double thirdCriterionPriority = 0.25;
        double fourthCriterionPriority = 0.0;

        int firstCriterionLazyGap = 10;
        bool fourthCriterionExactLocation = false;
        int fourthCriterionCityID = 0; 
        #endregion

        #region Properties
        public double FirstCriterionPriority
        {
            get { return firstCriterionPriority; }
        }
        public double SecondCriterionPriority
        {
            get { return secondCriterionPriority; }
        }
        public double ThirdCriterionPriority
        {
            get { return thirdCriterionPriority; }
        }
        public double FourthCriterionPriority
        {
            get { return fourthCriterionPriority; }
        }

        public int FirstCriterionLazyGap
        {
            get { return firstCriterionLazyGap; }
        }
        public bool FourthCriterionExactLocation
        {
            get { return fourthCriterionExactLocation; }
        }
        public int FourthCriterionCityID
        {
            get { return fourthCriterionCityID; }
        } 
        #endregion

        public AHPUserSettings()
        {
            if (fourthCriterionPriority > 0)
            {
                if (fourthCriterionCityID != 0)
                {
                    //Лучше без контекста(убрать вообще проверку, принять что у всех городов есть координаты)
                    if (context.Cities.Find(fourthCriterionCityID).Location == null)
                    {
                        fourthCriterionExactLocation = true;
                    }
                }
                else fourthCriterionPriority = 0;
            }
        }

        public AHPUserSettings(double firstPriority, double secondPriority, double thirdPriority, double fourthPriority,
                            int lazyGap, bool exactLocation, int cityId)
        {
            firstCriterionPriority = firstPriority;
            secondCriterionPriority = secondPriority;
            thirdCriterionPriority = thirdPriority;
            fourthCriterionPriority = fourthPriority;

            firstCriterionLazyGap = lazyGap;
            fourthCriterionExactLocation = exactLocation;
            fourthCriterionCityID = cityId;


            if (fourthCriterionPriority > 0)
            {
                if (fourthCriterionCityID != 0)
                {
                    //Лучше без контекста(убрать вообще проверку, принять что у всех городов есть координаты)
                    if (context.Cities.Find(fourthCriterionCityID).Location == null)
                    {
                        fourthCriterionExactLocation = true;
                    }
                }
                else fourthCriterionPriority = 0;
            }
        }


    }
}