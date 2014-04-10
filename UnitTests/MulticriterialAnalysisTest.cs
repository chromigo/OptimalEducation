﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.Logic.MulticriterialAnalysis;

namespace UnitTests
{
    [TestClass]
    public class MulticriterialAnalysisTest
    {
        Dictionary<string, double> _userCluster;

        Dictionary<string, double> _universityCluster1;
        Dictionary<string, double> _universityCluster2;
        Dictionary<string, double> _universityCluster3;

        List<Dictionary<string, double>> _totalUniversityClusters;
        public MulticriterialAnalysisTest()
        {
            _userCluster = new Dictionary<string, double>();
            _userCluster.Add("Русский", 0.5);
            _userCluster.Add("Математика", 0.7);
            _userCluster.Add("Информатика", 0.8);
            _userCluster.Add("Физика", 0.4);

            var _universityCluster1 = new Dictionary<string, double>();
            _universityCluster1.Add("Русский", 0.5);
            _universityCluster1.Add("Математика", 0.7);
            _universityCluster1.Add("Информатика", 0.8);
            _universityCluster1.Add("Физика", 0.4);
            var _universityCluster2 = new Dictionary<string, double>();
            _universityCluster2.Add("Русский", 0.4);
            _universityCluster2.Add("Математика", 0.3);
            _universityCluster2.Add("Информатика", 0.5);
            _universityCluster2.Add("Физика", 0.4);
            var _universityCluster3 = new Dictionary<string, double>();
            _universityCluster3.Add("Русский", 0.8);
            _universityCluster3.Add("Математика", 0.3);
            _universityCluster3.Add("Информатика", 0.3);
            _universityCluster3.Add("Физика", 0.1);

            var _totalUniversityClusters = new List<Dictionary<string, double>>();
            _totalUniversityClusters.Add(_universityCluster1);
            _totalUniversityClusters.Add(_universityCluster2);
            _totalUniversityClusters.Add(_universityCluster3);
        }
        //Здесь определяются предпочтения пользователя
        //Можно напрямуе его опрашивать
        //но т.к. хочется автоматизма, вывод о предпочтениях будем строить на основании информации из кластера пользователя
        [TestMethod]
        public void CreateNewParetto()
        {
            //Определяем группу важных критериев
            var finder = new MulticriterialAnalysis(_userCluster, _totalUniversityClusters);
            var userPrefer = finder.GetPreferenceRelations();

            //Сужаем получившуюся таблицу - Паретто
        }



    }



}