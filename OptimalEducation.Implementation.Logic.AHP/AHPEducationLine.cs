using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.CQRS;
using OptimalEducation.DAL;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.Characterizers;

namespace OptimalEducation.Implementation.Logic.AHP
{
    /// <summary>
    ///     Расчет приоритетности пользователей с помощью метода анализа иерарохий относительно направления
    /// </summary>
    [Obsolete(
        "АХТУНГ! Страшный говнокод! Был удален и возвращен по причине - нужно чтобы временно работало. В дальнейшем перепишется или выпилится насовсем."
        )]
    public class AhpEducationLine
    {
        private readonly OptimalEducationDbContext _context = new OptimalEducationDbContext();
        private readonly EducationLine _educationLine;
        private readonly List<Entrant> _entrants = new List<Entrant>();
        //Общие настройки метода и приоритеты критериев
        private readonly AhpEdLineSettings _settings;

        public AhpEducationLine(EducationLine educationLineGiven, List<Entrant> entrantsGiven,
            AhpEdLineSettings settings)
        {
            _educationLine = educationLineGiven;
            _entrants = entrantsGiven;
            _settings = settings;

            CalculateAll();
        }

        public AhpEducationLine(EducationLine educationLineGiven, List<Entrant> entrantsGiven)
        {
            _educationLine = educationLineGiven;
            _entrants = entrantsGiven;
            _settings = new AhpEdLineSettings();

            CalculateAll();
        }

        public AhpEducationLine(int edLineId)
        {
            _educationLine = _context.EducationLines.Find(edLineId);
            foreach (var entrInDb in _context.Entrants)
            {
                _entrants.Add(entrInDb);
            }
            _settings = new AhpEdLineSettings();

            CalculateAll();
        }

        private void CalculateAll()
        {
            _edLineRequiredSum = Convert.ToInt32(_educationLine.RequiredSum);
            foreach (var edLineReq in _educationLine.EducationLinesRequirements)
            {
                if (edLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                {
                    _edLineRequiredSum = _edLineRequiredSum - Convert.ToInt32(edLineReq.Requirement);
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
            var counter = 0;

            foreach (var entrant in _entrants)
            {
                var entrExamSum = 0;
                var edLineAcceptable = true;

                foreach (var edLineReq in _educationLine.EducationLinesRequirements)
                {
                    if (edLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                    {
                        break;
                    }

                    var foundResult = false;

                    foreach (var entrExam in entrant.UnitedStateExams)
                    {
                        if (entrExam.ExamDisciplineId == edLineReq.ExamDisciplineId)
                        {
                            foundResult = true;
                            entrExamSum = entrExamSum + Convert.ToInt32(entrExam.Result);
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
                    var user = new FirstCriterionUnit();
                    user.DatabaseId = Convert.ToInt32(entrant.Id);
                    user.LocalPriority = 0;

                    _firstCriterionContainer.Add(user);
                }
                else
                {
                    var user = new FirstCriterionUnit();
                    user.DatabaseId = Convert.ToInt32(entrant.Id);
                    user.MatrixId = counter;
                    user.EntrantSum = entrExamSum;
                    user.LocalPriority = 0;

                    counter++;

                    _firstCriterionContainer.Add(user);
                }
            }

            _firstCriterionMatrixSize = counter;
        }

        //Критерий трудности по ЕГЭ - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateFirstCriterion()
        {
            var pairwiseComparisonMatrix = new double[_firstCriterionMatrixSize, _firstCriterionMatrixSize];

            for (var i = 0; i < _firstCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _firstCriterionMatrixSize; j++)
                {
                    var a = CalculatedDifficultyResult(_firstCriterionContainer.Find(x => x.MatrixId == i).EntrantSum);
                    var b = CalculatedDifficultyResult(_firstCriterionContainer.Find(y => y.MatrixId == j).EntrantSum);

                    pairwiseComparisonMatrix[i, j] = FirstCriterionCompare(a, b);
                }
            }

            var resultVector = CalcEigenvectors(pairwiseComparisonMatrix, _firstCriterionMatrixSize);

            for (var i = 0; i < _firstCriterionMatrixSize; i++)
            {
                _firstCriterionContainer.Find(x => x.MatrixId == i).LocalPriority = resultVector[i];
            }

            //Первый критерий закончил рачет приоритетов (локальных)
        }

        //Первый критерий что-то вспомогательное
        private int CalculatedDifficultyResult(int entrantSum)
        {
            var tempResEntr = entrantSum;

            if (tempResEntr < 0) tempResEntr = 0;

            var weakResult = tempResEntr - Math.Abs(tempResEntr - _edLineRequiredSum);
            if (weakResult < 0) weakResult = 0;
            return weakResult;
        }

        //Сравнение результатов 2 направлений (1 критерий)
        private double FirstCriterionCompare(int firstNum, int secondNum)
        {
            if ((firstNum <= 0) && (secondNum <= 0)) return 1;
            if (firstNum <= 0) return (1/Convert.ToDouble(secondNum));
            if (secondNum <= 0) return firstNum;
            return Convert.ToDouble(firstNum)/Convert.ToDouble(secondNum);
        }

        //Нахождение собственного вектора через приближенные оценки
        private double[] CalcEigenvectors(double[,] matrix, int martixSize)
        {
            var resultVector = new double[martixSize];

            for (var i = 0; i < martixSize; i++)
            {
                double lineProduction = 1;

                for (var j = 0; j < martixSize; j++)
                {
                    lineProduction = lineProduction*matrix[i, j];
                }
                resultVector[i] = Math.Pow(lineProduction, 1/Convert.ToDouble(martixSize));
            }

            double vectorCompSum = 0;
            for (var i = 0; i < martixSize; i++)
            {
                vectorCompSum = vectorCompSum + resultVector[i];
            }
            for (var i = 0; i < martixSize; i++)
            {
                resultVector[i] = resultVector[i]/vectorCompSum;
            }
            return resultVector;
        }

        //Критерий совпадения кластеров - заполнение направлений во временный список
        private async void InitialiseSecondCriterion()
        {
            var totalAvailLines = 0;
            var queryBuilder = new QueryBuilder();
            var educationCharacteristicNamesHelper = new EducationCharacteristicNamesHelper(queryBuilder);
            var educationLineSummator = new EducationLineSummator(educationCharacteristicNamesHelper);
            var edLineClusterizer = new EducationLineCharacterizer(educationCharacteristicNamesHelper,
                educationLineSummator, new IdealEducationLineResult(educationLineSummator, queryBuilder));
            _educationLineClusters = await edLineClusterizer.Calculate(_educationLine);
            _maxEdLineClusterSum = _educationLineClusters.Values.Max();

            var entrantSummator = new EntrantSummator(educationCharacteristicNamesHelper);
            var entrantCharacterizer = new EntrantCharacterizer(educationCharacteristicNamesHelper, entrantSummator,
                new IdealEntrantResult(entrantSummator, queryBuilder));

            foreach (var entrant in _entrants)
            {
                var userAcceptable = true;

                var entrantCharacteristics = await entrantCharacterizer.Calculate(entrant);
                if (!entrantCharacteristics.Any()) userAcceptable = false;

                foreach (var item in _educationLineClusters)
                {
                    if (!entrantCharacteristics.ContainsKey(item.Key))
                    {
                        userAcceptable = false;
                    }
                }

                if (userAcceptable == false)
                {
                    var entant = new SecondCriterionUnit
                    {
                        DatabaseId = Convert.ToInt32(entrant.Id),
                        SecondCriterionAcceptable = false,
                        LocalPriority = 0
                    };

                    _secondCriterionContainer.Add(entant);
                }
                else
                {
                    var entant = new SecondCriterionUnit();
                    entant.DatabaseId = Convert.ToInt32(entrant.Id);
                    entant.SecondCriterionAcceptable = true;
                    entant.MatrixId = totalAvailLines;
                    entant.EntrantClusters = entrantCharacteristics;
                    entant.LocalPriority = 0;

                    totalAvailLines++;

                    _secondCriterionContainer.Add(entant);
                }
            }
            _secondCriterionMatrixSize = totalAvailLines;
        }

        //Критерий совпадения кластеров - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateSecondCriterion()
        {
            var pairwiseComparisonMatrix = new double[_secondCriterionMatrixSize, _secondCriterionMatrixSize];

            for (var i = 0; i < _secondCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _secondCriterionMatrixSize; j++)
                {
                    var a =
                        GetSecondCriterionEntruntValue(
                            _secondCriterionContainer.Find(x => x.MatrixId == i).EntrantClusters);
                    var b =
                        GetSecondCriterionEntruntValue(
                            _secondCriterionContainer.Find(y => y.MatrixId == j).EntrantClusters);
                    pairwiseComparisonMatrix[i, j] = SecondCriterionCompare(a, b);
                }
            }

            var resultVector = CalcEigenvectors(pairwiseComparisonMatrix, _secondCriterionMatrixSize);

            for (var i = 0; i < _secondCriterionMatrixSize; i++)
            {
                _secondCriterionContainer.Find(x => x.MatrixId == i).LocalPriority = resultVector[i];
            }
            //Второй критерий закончил рачет приоритетов (локальных)
        }

        //Подсчет значений для сравнения в таблице 2 критерий
        private double GetSecondCriterionEntruntValue(Dictionary<string, double> entClusters)
        {
            double difference = 0;
            var clustersCount = 0;

            foreach (var item in entClusters)
            {
                if (!_educationLineClusters.ContainsKey(item.Key)) continue;

                double normalizedEntrValue;
                if (entClusters.Values.Max() > 0) normalizedEntrValue = item.Value/entClusters.Values.Max();
                else normalizedEntrValue = 0;
                double normalizedEdLineValue;
                if (_maxEdLineClusterSum > 0)
                    normalizedEdLineValue = _educationLineClusters[item.Key]/_maxEdLineClusterSum;
                else normalizedEdLineValue = 0;

                difference = +Math.Abs(normalizedEntrValue - normalizedEdLineValue);

                clustersCount++;
            }

            return Math.Abs((Convert.ToDouble(clustersCount) - difference))/Convert.ToDouble(clustersCount);
        }

        //Сравнение результатов 2 направлений 2 критерий
        private double SecondCriterionCompare(double firstNum, double secondNum)
        {
            if ((firstNum <= 0) && (secondNum <= 0)) return 1;
            if (firstNum <= 0) return (1/secondNum);
            if (secondNum <= 0) return firstNum;
            return firstNum/secondNum;
        }

        //Критерий местных абитуриентов - заполнение направлений во временный список
        private void InitialiseThirdCriterion()
        {
            var counter = 0;

            foreach (var entrant in _entrants)
            {
                var user = new ThirdCriterionUnit();

                user.DatabaseId = Convert.ToInt32(entrant.Id);
                user.MatrixId = counter;
                user.LocalPriority = 0;
                if ((entrant.City != null) &&
                    (entrant.City.Id == _educationLine.Faculty.HigherEducationInstitution.City.Id))
                {
                    user.LocalUser = true;
                }
                else
                {
                    user.LocalUser = false;
                }

                counter++;

                _thirdCriterionContainer.Add(user);
            }
            _thirdCriterionMatrixSize = counter;
        }

        //Критерий местных абитуриентов - расчет притетов для всех направлений
        private void CalculateThirdCriterion()
        {
            var pairwiseComparisonMatrix = new double[_thirdCriterionMatrixSize, _thirdCriterionMatrixSize];

            for (var i = 0; i < _thirdCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _thirdCriterionMatrixSize; j++)
                {
                    var a = _thirdCriterionContainer.Find(x => x.MatrixId == i).LocalUser;
                    var b = _thirdCriterionContainer.Find(y => y.MatrixId == j).LocalUser;

                    if (a == b) pairwiseComparisonMatrix[i, j] = 1.0;
                    else if (a) pairwiseComparisonMatrix[i, j] = 9.0;
                    else pairwiseComparisonMatrix[i, j] = 1.0/9.0;
                }
            }

            var resultVector = CalcEigenvectors(pairwiseComparisonMatrix, _thirdCriterionMatrixSize);

            for (var i = 0; i < _thirdCriterionMatrixSize; i++)
            {
                _thirdCriterionContainer.Find(x => x.MatrixId == i).LocalPriority = resultVector[i];
            }
            //Третий критерий закончил рачет приоритетов (локальных)
        }

        //Расчет конечных приоритетов и сортировка
        private void FinalCalculate()
        {
            //Пролистываем результатты первого критерия и добавляем в общий список (учитывая приогритет критерия)
            for (var i = 0; i < _firstCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.DatabaseId == _firstCriterionContainer[i].DatabaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    var entrantFinal = new TotalResultUnit();
                    entrantFinal.DatabaseId = _firstCriterionContainer[i].DatabaseId;
                    AllCriterionContainer.Add(entrantFinal);
                }

                AllCriterionContainer.Find(x => x.DatabaseId == _firstCriterionContainer[i].DatabaseId)
                    .FirstCriterionFinalPriority =
                    _firstCriterionContainer[i].LocalPriority*_settings.FirstCriterionPriority;
            }
            //Потом тоже самое для 2 критерия
            for (var i = 0; i < _secondCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.DatabaseId == _secondCriterionContainer[i].DatabaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    var entrantFinal = new TotalResultUnit();
                    entrantFinal.DatabaseId = _secondCriterionContainer[i].DatabaseId;
                    AllCriterionContainer.Add(entrantFinal);
                }

                AllCriterionContainer.Find(x => x.DatabaseId == _secondCriterionContainer[i].DatabaseId)
                    .SecondCriterionFinalPriority =
                    _secondCriterionContainer[i].LocalPriority*_settings.SecondCriterionPriority;
            }
            //Потом тоже самое для 3 критерия
            for (var i = 0; i < _thirdCriterionContainer.Count; i++)
            {
                if ((AllCriterionContainer.FindIndex(x => x.DatabaseId == _thirdCriterionContainer[i].DatabaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    var entrantFinal = new TotalResultUnit();
                    entrantFinal.DatabaseId = _thirdCriterionContainer[i].DatabaseId;
                    AllCriterionContainer.Add(entrantFinal);
                }

                AllCriterionContainer.Find(x => x.DatabaseId == _thirdCriterionContainer[i].DatabaseId)
                    .ThirdCriterionFinalPriority =
                    _thirdCriterionContainer[i].LocalPriority*_settings.ThirdCriterionPriority;
            }


            //Завершающее сложение и заполнение выходного словарая
            foreach (var userFinal in AllCriterionContainer)
            {
                userFinal.AbsolutePriority = userFinal.FirstCriterionFinalPriority +
                                             userFinal.SecondCriterionFinalPriority +
                                             userFinal.ThirdCriterionFinalPriority;
            }

            //Сортировка
            AllCriterionContainer.Sort((x, y) => y.AbsolutePriority.CompareTo(x.AbsolutePriority));
        }

        #region Переменные для первого критерия - сложности на основе ЕГЭ

        private readonly List<FirstCriterionUnit> _firstCriterionContainer = new List<FirstCriterionUnit>();
        private int _firstCriterionMatrixSize;
        private int _edLineRequiredSum;

        private class FirstCriterionUnit
        {
            public int DatabaseId;
            public int EntrantSum;
            public double LocalPriority;
            public int MatrixId;
        }

        #endregion

        #region Переменные для второго критерия - схожести интересов (кластеров)

        private readonly List<SecondCriterionUnit> _secondCriterionContainer = new List<SecondCriterionUnit>();
        private Dictionary<string, double> _educationLineClusters = new Dictionary<string, double>();
        private double _maxEdLineClusterSum;
        private int _secondCriterionMatrixSize;

        private class SecondCriterionUnit
        {
            public int DatabaseId;
            public Dictionary<string, double> EntrantClusters = new Dictionary<string, double>();
            public double LocalPriority;
            public int MatrixId;
            public bool SecondCriterionAcceptable;
        }

        #endregion

        #region Переменные для 3 критерия - приоритет для не требующих общежития

        private readonly List<ThirdCriterionUnit> _thirdCriterionContainer = new List<ThirdCriterionUnit>();

        private int _thirdCriterionMatrixSize;

        private class ThirdCriterionUnit
        {
            public int DatabaseId;
            public double LocalPriority;
            public bool LocalUser;
            public int MatrixId;
        }

        #endregion

        #region Переменные и классы для сложения критериев в конечную оценку

        public List<TotalResultUnit> AllCriterionContainer = new List<TotalResultUnit>();

        public class TotalResultUnit
        {
            public double AbsolutePriority;
            public int DatabaseId;
            public double FirstCriterionFinalPriority;
            public double SecondCriterionFinalPriority;
            public double ThirdCriterionFinalPriority;
        }

        #endregion
    }
}