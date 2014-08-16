﻿using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace OptimalEducation.Logic.AnalyticHierarchyProcess
{
    /// <summary>
    /// Расчет приоритетности пользователей с помощью метода анализа иерарохий относительно направления
    /// </summary>
    public class AHPEducationLine
    {
        EducationLine _educationLine;
        List<Entrant> _entrants = new List<Entrant>();
        OptimalEducationDbContext context = new OptimalEducationDbContext();


        //Общие настройки метода и приоритеты критериев
        AHPEdLineSettings _settings;
        
        #region Переменные для первого критерия - сложности на основе ЕГЭ
        List<FirstCriterionUnit> FirstCriterionContainer = new List<FirstCriterionUnit>();
        int firstCriterionMatrixSize = 0;
        int edLineRequiredSum = 0;
        class FirstCriterionUnit
        {
            public int databaseId;
            public int matrixId;
            public int entrantSum;

            public double localPriority;
        }
        #endregion

        
        #region Переменные для второго критерия - схожести интересов (кластеров)
        List<SecondCriterionUnit> SecondCriterionContainer = new List<SecondCriterionUnit>();
        Dictionary<string, double> educationLineClusters = new Dictionary<string, double>();
        double maxEdLineClusterSum = 0;
        int secondCriterionMatrixSize = 0;

        class SecondCriterionUnit
        {
            public int databaseId;
            public bool secondCriterionAcceptable;
            public int matrixId;

            public Dictionary<string, double> entrantClusters = new Dictionary<string, double>();

            public double localPriority;
        }
        #endregion


        #region Переменные для 3 критерия - приоритет для не требующих общежития
        List<ThirdCriterionUnit> ThirdCriterionContainer = new List<ThirdCriterionUnit>();

        int thirdCriterionMatrixSize = 0;

        class ThirdCriterionUnit
        {
            public int databaseId;
            public int matrixId;

            public bool localUser;

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


        public AHPEducationLine(EducationLine educationLineGiven, List<Entrant> entrantsGiven, AHPEdLineSettings settings)
        {
            _educationLine = educationLineGiven;
            _entrants = entrantsGiven;
            _settings = settings;

            CalculateAll();
        }

        public AHPEducationLine(EducationLine educationLineGiven, List<Entrant> entrantsGiven)
        {
            _educationLine = educationLineGiven;
            _entrants = entrantsGiven;
            _settings = new AHPEdLineSettings();

            CalculateAll();
        }

        public AHPEducationLine(int edLineID)
        {
            _educationLine = context.EducationLines.Find(edLineID);
            foreach (var entrInDB in context.Entrants)
            {
                _entrants.Add(entrInDB);
            }
            _settings = new AHPEdLineSettings();

            CalculateAll();
        }


        void CalculateAll()
        {
            //foreach (EducationLineRequirement EdLineReq in _educationLine.EducationLinesRequirements)
            //{
            //    edLineRequiredSum += Convert.ToInt32(EdLineReq.Requirement);
            //}

            edLineRequiredSum = Convert.ToInt32(_educationLine.RequiredSum);
            foreach (EducationLineRequirement EdLineReq in _educationLine.EducationLinesRequirements)
            {
                if (EdLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                {
                    edLineRequiredSum = edLineRequiredSum - Convert.ToInt32(EdLineReq.Requirement);
                }
            }

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

            FinalCalculate();
        }


        //Критерий трудности по ЕГЭ - заполнение направлений во временный список
        private void InitialiseFirstCriterion()
        {
            int counter = 0;

            foreach (Entrant entrant in _entrants)
            {
                int entrExamSum = 0;
                bool edLineAcceptable = true;

                foreach (EducationLineRequirement EdLineReq in _educationLine.EducationLinesRequirements)
                {
                    if (EdLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                    {
                        break;
                    }

                    bool foundResult = false;

                    foreach (UnitedStateExam EntrExam in entrant.UnitedStateExams)
                    {
                        if (EntrExam.ExamDisciplineId == EdLineReq.ExamDisciplineId)
                        {
                            foundResult = true;
                            entrExamSum = entrExamSum + Convert.ToInt32(EntrExam.Result);
                            break;
                        }
                    }

                    if (foundResult == false)
                    {
                        edLineAcceptable = false;
                        break;
                    }
                }
                if (edLineAcceptable == false)
                {
                    FirstCriterionUnit User = new FirstCriterionUnit();
                    User.databaseId = Convert.ToInt32(entrant.Id);
                    User.localPriority = 0;

                    FirstCriterionContainer.Add(User);
                }
                else
                {
                    FirstCriterionUnit User = new FirstCriterionUnit();
                    User.databaseId = Convert.ToInt32(entrant.Id);
                    User.matrixId = counter;
                    User.entrantSum = entrExamSum;
                    User.localPriority = 0;

                    counter++;

                    FirstCriterionContainer.Add(User);
                }
            }

            firstCriterionMatrixSize = counter;
        }
        

        //Критерий трудности по ЕГЭ - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateFirstCriterion()
        {

            double[,] pairwiseComparisonMatrix = new double[firstCriterionMatrixSize, firstCriterionMatrixSize];

            for (int i = 0; i < firstCriterionMatrixSize; i++)
            {
                for (int j = 0; j < firstCriterionMatrixSize; j++)
                {
                    int a = CalculatedDifficultyResult(FirstCriterionContainer.Find(x => x.matrixId == i).entrantSum);
                    int b = CalculatedDifficultyResult(FirstCriterionContainer.Find(y => y.matrixId == j).entrantSum);
                    
                    pairwiseComparisonMatrix[i, j] = FirstCriterionCompare(a, b);
                }
            }

            double[] resultVector = CalcEigenvectors(pairwiseComparisonMatrix, firstCriterionMatrixSize);

            for (int i = 0; i < firstCriterionMatrixSize; i++)
            {
                FirstCriterionContainer.Find(x => x.matrixId == i).localPriority = resultVector[i];
            }

            //Первый критерий закончил рачет приоритетов (локальных)
        }


        //Первый критерий что-то вспомогательное
        private int CalculatedDifficultyResult(int entrantSum)
        {
            int tempResEntr = entrantSum;

            if (tempResEntr < 0) tempResEntr = 0;

            int weakResult = tempResEntr - Math.Abs(tempResEntr - edLineRequiredSum);
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
            for (int i = 0; i < martixSize; i++)
            {
                resultVector[i] = resultVector[i] / vectorCompSum;
            }
            return resultVector;
        }


        //Критерий совпадения кластеров - заполнение направлений во временный список
        private void InitialiseSecondCriterion()
        {
            int totalAvailLines = 0;
            var edLineClusterizer = new EducationLineCharacterizer(new EducationLineCalculationOptions());

            educationLineClusters = edLineClusterizer.Calculate(_educationLine);
            maxEdLineClusterSum = educationLineClusters.Values.Max();

            //foreach (var item in educationLineClusters)
            //{
            //    if (item.Value == null)
            //    {
            //        Console.WriteLine("Removing : " + item.Key);
            //        educationLineClusters.Remove(item.Key);
            //    }
            //}

            foreach (Entrant entrant in _entrants)
            {
                bool userAcceptable = true;

                EntrantCalculationOptions entrClassOpt = new EntrantCalculationOptions(false, true, true, true, true, true);
                var entrantCharacteristics = new EntrantCharacterizer(entrClassOpt).Calculate(entrant);
                if (entrantCharacteristics.Count() <= 0) userAcceptable = false;
                
                //foreach (var item in entrantCharacteristics)
                //{
                //    if (item.Value == null)
                //    {
                //        Console.WriteLine("Removing : " + item.Key);
                //        entrantCharacteristics.Remove(item.Key);
                //    }
                //}
                
                foreach (var item in educationLineClusters)
                {
                    if (!entrantCharacteristics.ContainsKey(item.Key))
                    {
                        userAcceptable = false;
                    }
                }

                if (userAcceptable == false)
                {
                    SecondCriterionUnit Entant = new SecondCriterionUnit();
                    Entant.databaseId = Convert.ToInt32(entrant.Id);
                    Entant.secondCriterionAcceptable = false;
                    Entant.localPriority = 0;

                    SecondCriterionContainer.Add(Entant);
                }
                else
                {
                    SecondCriterionUnit Entant = new SecondCriterionUnit();
                    Entant.databaseId = Convert.ToInt32(entrant.Id);
                    Entant.secondCriterionAcceptable = true;
                    Entant.matrixId = totalAvailLines;
                    Entant.entrantClusters = entrantCharacteristics;
                    Entant.localPriority = 0;

                    totalAvailLines++;

                    SecondCriterionContainer.Add(Entant);
                }
            }
            secondCriterionMatrixSize = totalAvailLines;
        }


        //Критерий совпадения кластеров - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateSecondCriterion()
        {

            double[,] pairwiseComparisonMatrix = new double[secondCriterionMatrixSize, secondCriterionMatrixSize];

            for (int i = 0; i < secondCriterionMatrixSize; i++)
            {
                for (int j = 0; j < secondCriterionMatrixSize; j++)
                {
                    double a = GetSecondCriterionEntruntValue(SecondCriterionContainer.Find(x => x.matrixId == i).entrantClusters);
                    double b = GetSecondCriterionEntruntValue(SecondCriterionContainer.Find(y => y.matrixId == j).entrantClusters);
                    pairwiseComparisonMatrix[i, j] = SecondCriterionCompare(a, b);
                }
            }
            
            double[] resultVector = CalcEigenvectors(pairwiseComparisonMatrix, secondCriterionMatrixSize);
            
            for (int i = 0; i < secondCriterionMatrixSize; i++)
            {
                SecondCriterionContainer.Find(x => x.matrixId == i).localPriority = resultVector[i];
            }
            //Второй критерий закончил рачет приоритетов (локальных)

        }


        //Подсчет значений для сравнения в таблице 2 критерий
        private double GetSecondCriterionEntruntValue(Dictionary<string, double> EntClusters)
        {
            double difference = 0;
            int clustersCount = 0;

            foreach (var item in EntClusters)
            {
                if (!educationLineClusters.ContainsKey(item.Key)) continue;

                double normalizedEntrValue;
                if (EntClusters.Values.Max() > 0) normalizedEntrValue = item.Value / EntClusters.Values.Max();
                else normalizedEntrValue = 0;
                double normalizedEdLineValue;
                if (maxEdLineClusterSum > 0) normalizedEdLineValue = educationLineClusters[item.Key] / maxEdLineClusterSum;
                else normalizedEdLineValue = 0;

                difference = +Math.Abs(normalizedEntrValue - normalizedEdLineValue);

                clustersCount++;
            }

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


        //Критерий местных абитуриентов - заполнение направлений во временный список
        private void InitialiseThirdCriterion()
        {
            int counter = 0;

            foreach (Entrant entrant in _entrants)
            {

                ThirdCriterionUnit user = new ThirdCriterionUnit();

                user.databaseId = Convert.ToInt32(entrant.Id);
                user.matrixId = counter;
                user.localPriority = 0;
                if ((entrant.City != null) && (entrant.City.Id == _educationLine.Faculty.HigherEducationInstitution.City.Id))
                {
                    user.localUser = true;
                }
                else
                {
                    user.localUser = false;
                }
                
                counter++;

                ThirdCriterionContainer.Add(user);
            }
            thirdCriterionMatrixSize = counter;
        }


        //Критерий местных абитуриентов - расчет притетов для всех направлений
        private void CalculateThirdCriterion()
        {

            double[,] pairwiseComparisonMatrix = new double[thirdCriterionMatrixSize, thirdCriterionMatrixSize];

            for (int i = 0; i < thirdCriterionMatrixSize; i++)
            {
                for (int j = 0; j < thirdCriterionMatrixSize; j++)
                {
                    bool a = ThirdCriterionContainer.Find(x => x.matrixId == i).localUser;
                    bool b = ThirdCriterionContainer.Find(y => y.matrixId == j).localUser;

                    if (a == b) pairwiseComparisonMatrix[i, j] = 1.0;
                    else if (a == true) pairwiseComparisonMatrix[i, j] = 9.0;
                    else pairwiseComparisonMatrix[i, j]  = 1.0/9.0;                    
                }
            }

            double[] resultVector = CalcEigenvectors(pairwiseComparisonMatrix, thirdCriterionMatrixSize);

            for (int i = 0; i < thirdCriterionMatrixSize; i++)
            {
                ThirdCriterionContainer.Find(x => x.matrixId == i).localPriority = resultVector[i];
            }
            //Третий критерий закончил рачет приоритетов (локальных)
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
                    TotalResultUnit EntrantFinal = new TotalResultUnit();
                    EntrantFinal.databaseId = FirstCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EntrantFinal);
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
                    TotalResultUnit EntrantFinal = new TotalResultUnit();
                    EntrantFinal.databaseId = SecondCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EntrantFinal);
                }

                AllCriterionContainer.Find(x => x.databaseId == SecondCriterionContainer[i].databaseId).secondCriterionFinalPriority =
                    SecondCriterionContainer[i].localPriority * _settings.SecondCriterionPriority;
            }
            //Потом тоже самое для 3 критерия
            for (int i = 0; i < ThirdCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.databaseId == ThirdCriterionContainer[i].databaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    TotalResultUnit EntrantFinal = new TotalResultUnit();
                    EntrantFinal.databaseId = ThirdCriterionContainer[i].databaseId;
                    AllCriterionContainer.Add(EntrantFinal);
                }

                AllCriterionContainer.Find(x => x.databaseId == ThirdCriterionContainer[i].databaseId).thirdCriterionFinalPriority =
                    ThirdCriterionContainer[i].localPriority * _settings.ThirdCriterionPriority;
            }



            //Завершающее сложение и заполнение выходного словарая
            foreach (TotalResultUnit UserFinal in AllCriterionContainer)
            {
                UserFinal.absolutePriority = UserFinal.firstCriterionFinalPriority + UserFinal.secondCriterionFinalPriority + UserFinal.thirdCriterionFinalPriority;
            }

            //Сортировка
            AllCriterionContainer.Sort((x, y) => y.absolutePriority.CompareTo(x.absolutePriority));

        }



    }
    public class AHPEdLineSettings
    {
        #region Fields
        OptimalEducationDbContext context = new OptimalEducationDbContext();

        public double firstCriterionPriority = 0.4;
        public double secondCriterionPriority = 0.35;
        public double thirdCriterionPriority = 0.25;

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
        #endregion


        public AHPEdLineSettings()
        {
        }

        public AHPEdLineSettings(double firstPriority, double secondPriority, double thirdPriority)
        {
            firstCriterionPriority = firstPriority;
            secondCriterionPriority = secondPriority;
            thirdCriterionPriority = thirdPriority;
        }
    }
}
