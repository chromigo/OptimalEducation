using OptimalEducation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.Clusterizer
{
    /// <summary>
    /// Результаты кластеризации для пользователя. 
    /// Класс инкапсулирует логику построения частичных кластеров(по каждой характеристике пользователя)
    /// и ихнему суммированию в целостный кластер.
    /// </summary>
    public class EntrantClusterizer
    {
        Entrant _entrant;
        Dictionary<string, double> unatedStatedExamCluster = new Dictionary<string, double>();
        Dictionary<string, double> schoolMarkCluster = new Dictionary<string, double>();
        Dictionary<string, double> olympiadCluster = new Dictionary<string, double>();
        Dictionary<string, double> sectionCluster = new Dictionary<string, double>();
        Dictionary<string, double> hobbieCluster = new Dictionary<string, double>();
        Dictionary<string, double> schoolTypeCluster = new Dictionary<string, double>();

        Dictionary<string, double> _totalCluster = new Dictionary<string, double>();
        public Dictionary<string, double> Cluster { get { return _totalCluster; } }
        public EntrantClusterizer(Entrant entrant)
        {
            _entrant = entrant;
            CalculateSum();
        }

        #region По заданным характеристикам строит частичные кластеры с результатами
        private void UnatedStateExamClustering()
        {
            foreach (var exam in _entrant.UnitedStateExams)
            {
                var result = exam.Result;
                var discipline = exam.Discipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    var clusterResult = result * coeff;
                    FillPartialCluster(unatedStatedExamCluster, clusterName, clusterResult);
                }
            }
        }

        private void SchoolMarkClustering()
        {
            foreach (var shoolMark in _entrant.SchoolMarks)
            {
                var result = shoolMark.Result;
                var discipline = shoolMark.SchoolDiscipline;
                foreach (var weight in discipline.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    var clusterResult = result * coeff;
                    FillPartialCluster(schoolMarkCluster, clusterName, clusterResult);
                }
            }
        }

        private void OlympiadClustering()
        {
            foreach (var olympResult in _entrant.ParticipationInOlympiads)
            {
                var result = olympResult.Result;
                var olympiad = olympResult.Olympiad;
                foreach (var weight in olympiad.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    double clusterResult = 0;
                    //TODO: Реализовать особую логику учета данных?
                    switch (result)
                    {
                        case OlypmpiadResult.FirstPlace: clusterResult = (int)result * coeff;
                            break;
                        case OlypmpiadResult.SecondPlace: clusterResult = (int)result * coeff;
                            break;
                        case OlypmpiadResult.ThirdPlace: clusterResult = (int)result * coeff;
                            break;
                        default:
                            break;
                    }

                    FillPartialCluster(olympiadCluster, clusterName, clusterResult);
                }
            }
        }

        private void SectionClustering()
        {
            foreach (var sectionResult in _entrant.ParticipationInSections)
            {
                var result = sectionResult.YearPeriod;
                var section = sectionResult.Section;
                foreach (var weight in section.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    //TODO: Реализовать особую логику учета данных?
                    double clusterResult = result * coeff;

                    FillPartialCluster(sectionCluster, clusterName, clusterResult);
                }
            }
        }

        private void HobbieClustering()
        {
            foreach (var hobbieResult in _entrant.Hobbies)
            {
                foreach (var weight in hobbieResult.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    //TODO: Реализовать особую логику учета данных?
                    double someValue = 1;
                    double clusterResult = someValue * coeff;

                    FillPartialCluster(hobbieCluster, clusterName, clusterResult);
                }
            }
        }

        private void SchoolTypeClustering()
        {
            //для простоты будем брать последнюю школу, где абитуриент учился
            //(возможно стоит рассмотреть более сложный вариант в будущем)
            var lastSchool = _entrant.Schools.LastOrDefault();
            if(lastSchool!=null)
            {
                //Или еще учитывать кол-во лет обучения?(нет в модели)
                var quality = lastSchool.EducationQuality;
                var schoolType = lastSchool.SchoolType;
                foreach (var weight in schoolType.Weights)
                {
                    var coeff = weight.Coefficient;
                    var clusterName = weight.Cluster.Name;

                    //TODO: Реализовать особую логику учета данных?
                    double clusterResult = (int)quality * coeff;

                    FillPartialCluster(schoolTypeCluster, clusterName, clusterResult);
                }
            }
        }
        /// <summary>
        /// Заполняет выбранный кластер заданными значениями(добавляет или суммирует)
        /// </summary>
        /// <param name="clusterToFill">Кластер, который необходимо заполнить/обновить</param>
        /// <param name="clusterName">Элемент кластера, который добавляют/обновляют</param>
        /// <param name="clusterResult"></param>
        private void FillPartialCluster(Dictionary<string,double> clusterToFill ,string clusterName, double clusterResult)
        {
            if (!clusterToFill.ContainsKey(clusterName))
                clusterToFill.Add(clusterName, clusterResult);
            else
                clusterToFill[clusterName] += clusterResult;
        }
        #endregion

        /// <summary>
        /// Складывает результаты каждого частного кластера по определенному правилу
        /// </summary>
        private void CalculateSum()
        {
            UnatedStateExamClustering();
            SchoolMarkClustering();
            OlympiadClustering();
            SectionClustering();
            HobbieClustering();
            SchoolTypeClustering();

            foreach (var item in unatedStatedExamCluster)
            {
                FillTotalCluster(item);
            }
            foreach (var item in schoolMarkCluster)
            {
                FillTotalCluster(item);
            }
            foreach (var item in olympiadCluster)
            {
                FillTotalCluster(item);
            }
            foreach (var item in sectionCluster)
            {
                FillTotalCluster(item);
            }
            foreach (var item in hobbieCluster)
            {
                FillTotalCluster(item);
            }
            foreach (var item in schoolTypeCluster)
            {
                FillTotalCluster(item);
            }
        }

        private void FillTotalCluster(KeyValuePair<string, double> item)
        {
            if (!_totalCluster.ContainsKey(item.Key))
                _totalCluster.Add(item.Key, item.Value);
            else
                _totalCluster[item.Key] += item.Value;
        }
    }
}
