using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Implementation.CQRS;
using OptimalEducation.DAL;
using OptimalEducation.DAL.Models;
using OptimalEducation.Implementation.Logic.Characterizers;

namespace OptimalEducation.Implementation.Logic.AHP
{
    /// <summary>
    ///     Расчет приоритетности направлений с помощью метода анализа иерарохий относительно пользователя
    /// </summary>
    [Obsolete(
        "АХТУНГ! Страшный говнокод! Был удален и возвращен по причине - нужно чтобы временно работало. В дальнейшем перепишется или выпилится насовсем."
        )]
    public class AhpUser
    {
        private int _totalEducationLines;
        private readonly OptimalEducationDbContext _context = new OptimalEducationDbContext();
        private readonly List<EducationLine> _educationLines = new List<EducationLine>();
        private readonly Entrant _entrant;
        //Общие настройки метода и приоритеты критериев
        private readonly AhpUserSettings _settings;

        public AhpUser(Entrant entrantGiven, List<EducationLine> educationLinesGiven, AhpUserSettings settings)
        {
            _entrant = entrantGiven;
            _educationLines = educationLinesGiven;
            _settings = settings;
        }

        public AhpUser(Entrant entrantGiven, List<EducationLine> educationLinesGiven)
        {
            _entrant = entrantGiven;
            _educationLines = educationLinesGiven;
            _settings = new AhpUserSettings();
        }

        public AhpUser(int userId)
        {
            _entrant = _context.Entrants.Find(userId);
            foreach (var edLineInDb in _context.EducationLines)
            {
                _educationLines.Add(edLineInDb);
            }
            _settings = new AhpUserSettings();
        }

        public async Task<List<TotalResultUnit>> CalculateAll()
        {
            if (_settings.FirstCriterionPriority > 0)
            {
                InitialiseFirstCriterion();
                CalculateFirstCriterion();
            }

            if (_settings.SecondCriterionPriority > 0)
            {
                await InitialiseSecondCriterion();
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

            return FinalCalculate();
        }

        //Критерий расстояний - расчеты приоритетов для всех подходящих направлений
        //Расчет конечных приоритетов и сортировка
        private List<TotalResultUnit> FinalCalculate()
        {
            //Пролистываем результатты первого критерия и добавляем в общий список (учитывая приогритет критерия)
            for (var i = 0; i < _firstCriterionContainer.Count; i++)
            {
                if ((_allCriterionContainer.FindIndex(x => x.DatabaseId == _firstCriterionContainer[i].DatabaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    var educationLineFinal = new TotalResultUnit();
                    educationLineFinal.DatabaseId = _firstCriterionContainer[i].DatabaseId;
                    _allCriterionContainer.Add(educationLineFinal);
                }

                _allCriterionContainer.Find(x => x.DatabaseId == _firstCriterionContainer[i].DatabaseId)
                    .FirstCriterionFinalPriority =
                    _firstCriterionContainer[i].LocalPriority*_settings.FirstCriterionPriority;
            }
            //Потом тоже самое для 2 критерия
            for (var i = 0; i < _secondCriterionContainer.Count; i++)
            {
                if ((_allCriterionContainer.FindIndex(x => x.DatabaseId == _secondCriterionContainer[i].DatabaseId)) >=
                    0)
                {
                    //DUNNO LOL
                }
                else
                {
                    var educationLineFinal = new TotalResultUnit();
                    educationLineFinal.DatabaseId = _secondCriterionContainer[i].DatabaseId;
                    _allCriterionContainer.Add(educationLineFinal);
                }

                _allCriterionContainer.Find(x => x.DatabaseId == _secondCriterionContainer[i].DatabaseId)
                    .SecondCriterionFinalPriority =
                    _secondCriterionContainer[i].LocalPriority*_settings.SecondCriterionPriority;
            }
            //Потом тоже самое для 3-го
            for (var i = 0; i < _thirdCriterionContainer.Count; i++)
            {
                if ((_allCriterionContainer.FindIndex(x => x.DatabaseId == _thirdCriterionContainer[i].DatabaseId)) >= 0)
                {
                    //DUNNO LOL
                }
                else
                {
                    var educationLineFinal = new TotalResultUnit();
                    educationLineFinal.DatabaseId = _thirdCriterionContainer[i].DatabaseId;
                    _allCriterionContainer.Add(educationLineFinal);
                }

                _allCriterionContainer.Find(x => x.DatabaseId == _thirdCriterionContainer[i].DatabaseId)
                    .ThirdCriterionFinalPriority =
                    _thirdCriterionContainer[i].LocalPriorityFaculty*_settings.ThirdCriterionPriority*
                    (1 - HeIprestigePart) +
                    _thirdCriterionContainer[i].LocalPriorityHei*_settings.ThirdCriterionPriority*HeIprestigePart;
            }
            //Потом тоже самое для 4 критерия
            for (var i = 0; i < _fourthCriterionContainer.Count; i++)
            {
                if ((_allCriterionContainer.FindIndex(x => x.DatabaseId == _fourthCriterionContainer[i].DatabaseId)) >=
                    0)
                {
                    //DUNNO LOL
                    //ЕВГЕНИЙ: БЛЯЯЯ. ЧИтающий этот говнокод, да узри же тот же ужас и батхерт, который испытал я, пытаясь причесать его...
                }
                else
                {
                    var educationLineFinal = new TotalResultUnit();
                    educationLineFinal.DatabaseId = _fourthCriterionContainer[i].DatabaseId;
                    _allCriterionContainer.Add(educationLineFinal);
                }

                _allCriterionContainer.Find(x => x.DatabaseId == _fourthCriterionContainer[i].DatabaseId)
                    .FourthCriterionFinalPriority =
                    _fourthCriterionContainer[i].LocalPriority*_settings.FourthCriterionPriority;
            }

            //Завершающее сложение и заполнение выходного словарая
            foreach (var edLineFinal in _allCriterionContainer)
            {
                edLineFinal.AbsolutePriority = edLineFinal.FirstCriterionFinalPriority +
                                               edLineFinal.SecondCriterionFinalPriority +
                                               edLineFinal.ThirdCriterionFinalPriority +
                                               edLineFinal.FourthCriterionFinalPriority;
            }

            //Сортировка
            _allCriterionContainer.Sort((x, y) => y.AbsolutePriority.CompareTo(x.AbsolutePriority));

            return _allCriterionContainer;
        }

        //Критерий трудности по ЕГЭ - заполнение направлений во временный список
        private void InitialiseFirstCriterion()
        {
            var totalAvailLines = 0;

            foreach (var edLine in _educationLines)
            {
                var entrExamSum = 0;
                var edLineAcceptable = true;

                foreach (var edLineReq in edLine.EducationLinesRequirements)
                {
                    if (edLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                    {
                        break;
                    }

                    var foundResult = false;
                    foreach (var entrExam in _entrant.UnitedStateExams)
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
                    var educationLine = new FirstCriterionUnit();
                    educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                    educationLine.FirstCriterionAcceptable = false;
                    educationLine.RequiredSum = Convert.ToInt32(edLine.RequiredSum);
                    foreach (var edLineReq in edLine.EducationLinesRequirements)
                    {
                        if (edLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                        {
                            educationLine.RequiredSum = educationLine.RequiredSum -
                                                        Convert.ToInt32(edLineReq.Requirement);
                        }
                    }
                    educationLine.LocalPriority = 0;

                    _firstCriterionContainer.Add(educationLine);
                }
                else
                {
                    var educationLine = new FirstCriterionUnit();
                    educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                    educationLine.FirstCriterionAcceptable = true;
                    educationLine.MatrixId = totalAvailLines;
                    educationLine.RequiredSum = Convert.ToInt32(edLine.RequiredSum);
                    foreach (var edLineReq in edLine.EducationLinesRequirements)
                    {
                        if (edLineReq.ExamDiscipline.ExamType != ExamType.UnitedStateExam)
                        {
                            educationLine.RequiredSum = educationLine.RequiredSum -
                                                        Convert.ToInt32(edLineReq.Requirement);
                        }
                    }
                    educationLine.WeakenedDifficulty = WeakenedDifficultyResult(entrExamSum,
                        Convert.ToInt32(edLine.RequiredSum));
                    educationLine.LocalPriority = 0;

                    totalAvailLines++;

                    _firstCriterionContainer.Add(educationLine);
                }
            }
            _firstCriterionMatrixSize = totalAvailLines;
            if (_firstCriterionContainer.Count > _totalEducationLines)
                _totalEducationLines = _firstCriterionContainer.Count;
        }

        //Критерий трудности по ЕГЭ - расчеты приоритетов для всех подхолдящий направлений
        private void CalculateFirstCriterion()
        {
            var pairwiseComparisonMatrix = new double[_firstCriterionMatrixSize, _firstCriterionMatrixSize];

            for (var i = 0; i < _firstCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _firstCriterionMatrixSize; j++)
                {
                    var a = _firstCriterionContainer.Find(x => x.MatrixId == i).WeakenedDifficulty;
                    var b = _firstCriterionContainer.Find(y => y.MatrixId == j).WeakenedDifficulty;
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

        //Расчет "ослабленного" результата направления
        private int WeakenedDifficultyResult(int entrantSum, int requiredSum)
        {
            var weakResEntr = (entrantSum - _settings.FirstCriterionLazyGap);
            if (weakResEntr < 0) weakResEntr = 0;
            var weakResult = weakResEntr - Math.Abs(weakResEntr - requiredSum);
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
        private async Task InitialiseSecondCriterion()
        {
            var totalAvailLines = 0;
            var queryBuilder = new QueryBuilder();
            var educationCharacteristicNamesHelper = new EducationCharacteristicNamesHelper(queryBuilder);
            var entrantSummator = new EntrantSummator(educationCharacteristicNamesHelper);
            var entrantCharacterizer = new EntrantCharacterizer(educationCharacteristicNamesHelper, entrantSummator,
                new IdealEntrantResult(entrantSummator, queryBuilder));
            _entrantCharacteristics = await entrantCharacterizer.Calculate(_entrant);

            _maxEntrantClusterSum = _entrantCharacteristics.Values.Max();
            var educationLineSummator = new EducationLineSummator(educationCharacteristicNamesHelper);
            var edLineClusterizer = new EducationLineCharacterizer(educationCharacteristicNamesHelper,
                educationLineSummator, new IdealEducationLineResult(educationLineSummator, queryBuilder));

            foreach (var edLine in _educationLines)
            {
                var edLineAcceptable = true;

                var edLineResult = await edLineClusterizer.Calculate(edLine);

                if (!edLineResult.Any()) edLineAcceptable = false;

                foreach (var item in edLineResult)
                {
                    if (!_entrantCharacteristics.ContainsKey(item.Key))
                    {
                        edLineAcceptable = false;
                    }
                }

                if (edLineAcceptable == false)
                {
                    var educationLine = new SecondCriterionUnit();
                    educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                    educationLine.SecondCriterionAcceptable = false;
                    educationLine.LocalPriority = 0;

                    _secondCriterionContainer.Add(educationLine);
                }
                else
                {
                    var educationLine = new SecondCriterionUnit();
                    educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                    educationLine.SecondCriterionAcceptable = true;
                    educationLine.MatrixId = totalAvailLines;
                    educationLine.EducationLineClusters = edLineResult;
                    educationLine.LocalPriority = 0;

                    totalAvailLines++;

                    _secondCriterionContainer.Add(educationLine);
                }
            }
            _secondCriterionMatrixSize = totalAvailLines;

            if (_secondCriterionContainer.Count > _totalEducationLines)
                _totalEducationLines = _secondCriterionContainer.Count;
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
                        GetSecondCriterionEdLineValue(
                            _secondCriterionContainer.Find(x => x.MatrixId == i).EducationLineClusters);
                    var b =
                        GetSecondCriterionEdLineValue(
                            _secondCriterionContainer.Find(y => y.MatrixId == j).EducationLineClusters);
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
        private double GetSecondCriterionEdLineValue(Dictionary<string, double> edLineClusters)
        {
            double difference = 0;
            var clustersCount = 0;

            foreach (var item in edLineClusters)
            {
                if (!_entrantCharacteristics.ContainsKey(item.Key)) continue;

                double normalizedEdLineValue;
                if (edLineClusters.Values.Max() > 0) normalizedEdLineValue = item.Value/edLineClusters.Values.Max();
                else normalizedEdLineValue = 0;
                var normalizedEntrantValuse = _entrantCharacteristics[item.Key]/_maxEntrantClusterSum;

                difference = +Math.Abs(normalizedEdLineValue - normalizedEntrantValuse);

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

        //Критерий престижа - заполнение направлений во временный список
        private void InitialiseThirdCriterion()
        {
            var counter = 0;

            foreach (var edLine in _educationLines)
            {
                var educationLine = new ThirdCriterionUnit();
                educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                educationLine.MatrixId = counter;

                if (edLine.Faculty.Prestige > 0) educationLine.FacultyPrestige = edLine.Faculty.Prestige;
                else educationLine.FacultyPrestige = 0;

                if (edLine.Faculty.HigherEducationInstitution.Prestige > 0)
                    educationLine.HeiPrestige = edLine.Faculty.HigherEducationInstitution.Prestige;
                else educationLine.HeiPrestige = 0;

                educationLine.LocalPriorityFaculty = 0;
                educationLine.LocalPriorityHei = 0;

                counter++;

                _thirdCriterionContainer.Add(educationLine);
            }
            _thirdCriterionMatrixSize = counter;
        }

        //Критерий престижа - расчет притетов для всех направлений
        private void CalculateThirdCriterion()
        {
            var pairwiseComparisonMatrix = new double[_thirdCriterionMatrixSize, _thirdCriterionMatrixSize];

            for (var i = 0; i < _thirdCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _thirdCriterionMatrixSize; j++)
                {
                    var a =
                        GetThirdCriterionEdLineValue(_thirdCriterionContainer.Find(x => x.MatrixId == i).FacultyPrestige);
                    var b =
                        GetThirdCriterionEdLineValue(_thirdCriterionContainer.Find(y => y.MatrixId == j).FacultyPrestige);

                    pairwiseComparisonMatrix[i, j] = a/b;
                }
            }

            var resultVector1 = CalcEigenvectors(pairwiseComparisonMatrix, _thirdCriterionMatrixSize);

            for (var i = 0; i < _thirdCriterionMatrixSize; i++)
            {
                _thirdCriterionContainer.Find(x => x.MatrixId == i).LocalPriorityFaculty = resultVector1[i];
            }

            for (var i = 0; i < _thirdCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _thirdCriterionMatrixSize; j++)
                {
                    var a = GetThirdCriterionEdLineValue(_thirdCriterionContainer.Find(x => x.MatrixId == i).HeiPrestige);
                    var b = GetThirdCriterionEdLineValue(_thirdCriterionContainer.Find(y => y.MatrixId == j).HeiPrestige);

                    pairwiseComparisonMatrix[i, j] = a/b;
                }
            }

            var resultVector2 = CalcEigenvectors(pairwiseComparisonMatrix, _thirdCriterionMatrixSize);

            for (var i = 0; i < _thirdCriterionMatrixSize; i++)
            {
                _thirdCriterionContainer.Find(x => x.MatrixId == i).LocalPriorityHei = resultVector2[i];
            }

            //Тертий критерий закончил рачет приоритетов (локальных)
        }

        //Подсчет значений для сравнения в таблице 3 критерий (сильно упростился, но вообще убирать лень)
        private double GetThirdCriterionEdLineValue(int prestige)
        {
            if (prestige <= 0) return 1;
            return Convert.ToDouble(prestige);
        }

        //Критерий расстояния - заполнение направлений во временный список
        private void InitialiseFourthCriterion()
        {
            var counter = 0;

            Console.WriteLine(_settings.FourthCriterionExactLocation.ToString());

            foreach (var edLine in _educationLines)
            {
                if (_settings.FourthCriterionExactLocation)
                {
                    var educationLine = new FourthCriterionUnit();
                    educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                    educationLine.MatrixId = counter;
                    educationLine.HasCoordinates = false;
                    educationLine.LocalPriority = 0;

                    educationLine.Distance = edLine.Faculty.HigherEducationInstitution.City.Id ==
                                             _settings.FourthCriterionCityId
                        ? 0
                        : 9.0;

                    counter++;

                    _fourthCriterionContainer.Add(educationLine);
                }
                else
                {
                    var subjectLocation = _context.Cities.Find(_settings.FourthCriterionCityId).Location;

                    if (edLine.Faculty.HigherEducationInstitution.City.Location == null)
                    {
                        var educationLine = new FourthCriterionUnit();
                        educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                        educationLine.HasCoordinates = false;
                        educationLine.LocalPriority = 0;
                        _fourthCriterionContainer.Add(educationLine);
                    }
                    else
                    {
                        var educationLine = new FourthCriterionUnit();
                        educationLine.DatabaseId = Convert.ToInt32(edLine.Id);
                        educationLine.MatrixId = counter;
                        educationLine.HasCoordinates = true;
                        educationLine.LocalPriority = 0;

                        var universityLocation = edLine.Faculty.HigherEducationInstitution.City.Location;

                        educationLine.Distance = Convert.ToDouble(subjectLocation.Distance(universityLocation));

                        Console.WriteLine("dist: " + educationLine.Distance);

                        counter++;

                        _fourthCriterionContainer.Add(educationLine);
                    }
                }
            }
            _fourthCriterionMatrixSize = counter;
        }

        private void CalculateFourthCriterion()
        {
            var pairwiseComparisonMatrix = new double[_fourthCriterionMatrixSize, _fourthCriterionMatrixSize];

            for (var i = 0; i < _fourthCriterionMatrixSize; i++)
            {
                for (var j = 0; j < _fourthCriterionMatrixSize; j++)
                {
                    var a = _fourthCriterionContainer.Find(x => x.MatrixId == i).Distance;
                    var b = _fourthCriterionContainer.Find(y => y.MatrixId == j).Distance;

                    pairwiseComparisonMatrix[i, j] = FourthCriterionCompare(a, b);
                }
            }

            var resultVector = CalcEigenvectors(pairwiseComparisonMatrix, _fourthCriterionMatrixSize);


            for (var i = 0; i < _fourthCriterionMatrixSize; i++)
            {
                _fourthCriterionContainer.Find(x => x.MatrixId == i).LocalPriority = resultVector[i];
            }
            //4 критерий закончил рачет приоритетов (локальных)
        }

        //Сравнение дистанции 

        private double FourthCriterionCompare(double firstDist, double secondDist)
        {
            if ((firstDist <= 0) && (secondDist <= 0)) return 1;
            if (firstDist <= 0) return 9.0;
            if (secondDist <= 0) return 1.0/9.0;
            return secondDist/firstDist;
        }

        #region Переменные для первого критерия - сложности на основе ЕГЭ

        private readonly List<FirstCriterionUnit> _firstCriterionContainer = new List<FirstCriterionUnit>();
        private int _firstCriterionMatrixSize;

        private class FirstCriterionUnit
        {
            public int DatabaseId;
            public bool FirstCriterionAcceptable;
            public double LocalPriority;
            public int MatrixId;
            public int RequiredSum;
            public int WeakenedDifficulty;
        }

        #endregion

        #region Переменные для второго критерия - схожести интересов (кластеров)

        private readonly List<SecondCriterionUnit> _secondCriterionContainer = new List<SecondCriterionUnit>();
        private Dictionary<string, double> _entrantCharacteristics = new Dictionary<string, double>();
        private double _maxEntrantClusterSum;
        private int _secondCriterionMatrixSize;

        private class SecondCriterionUnit
        {
            public int DatabaseId;
            public Dictionary<string, double> EducationLineClusters = new Dictionary<string, double>();
            public double LocalPriority;
            public int MatrixId;
            public bool SecondCriterionAcceptable;
        }

        #endregion

        #region Переменные для тертьего критерия - престиж (универов и направлений)

        private readonly List<ThirdCriterionUnit> _thirdCriterionContainer = new List<ThirdCriterionUnit>();

        private int _thirdCriterionMatrixSize;
        private const double HeIprestigePart = 0.7;

        private class ThirdCriterionUnit
        {
            public int DatabaseId;
            public int FacultyPrestige;
            public int HeiPrestige;
            public double LocalPriorityFaculty;
            public double LocalPriorityHei;
            public int MatrixId;
        }

        #endregion

        #region Переменные для 4 критерия - расстояния до заданного города

        private readonly List<FourthCriterionUnit> _fourthCriterionContainer = new List<FourthCriterionUnit>();

        private int _fourthCriterionMatrixSize;

        private class FourthCriterionUnit
        {
            public int DatabaseId;
            public double Distance;
            public bool HasCoordinates;
            public double LocalPriority;
            public int MatrixId;
        }

        #endregion

        #region Переменные и классы для сложения критериев в конечную оценку

        /// <summary>
        ///     Отсортированный список с резульатами работы МАИ
        /// </summary>
        private readonly List<TotalResultUnit> _allCriterionContainer = new List<TotalResultUnit>();

        /// <summary>
        ///     Класс с результатами измерений
        /// </summary>
        public class TotalResultUnit
        {
            //Приоритет направления
            public double AbsolutePriority;
            //Id направления/ученика в базе данных
            public int DatabaseId;
            //Необязательные для отображения параметры
            public double FirstCriterionFinalPriority;
            public double FourthCriterionFinalPriority;
            public double SecondCriterionFinalPriority;
            public double ThirdCriterionFinalPriority;
        }

        #endregion
    }
}