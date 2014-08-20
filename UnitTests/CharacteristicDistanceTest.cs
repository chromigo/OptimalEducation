using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.Logic.Characterizers;

namespace UnitTests
{
    [TestClass]
    public class CharacteristicDistanceTest
    {
        //Внимание! помнить про то что double не дает 100% точности при вычислениях
        //TODO: Добавить тесты, проверяющие результаты вычислений когда у пользователя и вуза отличается набор характеристик?
        readonly Dictionary<string, double> entrantCharacteristics=new Dictionary<string,double>();
        readonly Dictionary<string, double> educationLineCharacteristics = new Dictionary<string, double>();
        public CharacteristicDistanceTest()
        {
            entrantCharacteristics.Add("Русский", 0.6);
            entrantCharacteristics.Add("Математика", 0.8);
            entrantCharacteristics.Add("Информатика", 0.9);

            educationLineCharacteristics.Add("Русский", 0.4);
            educationLineCharacteristics.Add("Математика", 0.7);
            educationLineCharacteristics.Add("Информатика", 0.8);
        }

    }
}
