using System;

namespace OptimalEducation.Implementation.Logic.AHP
{
    [Obsolete("АХТУНГ! Страшный говнокод! Был удален и возвращен по причине - нужно чтобы временно работало. В дальнейшем перепишется или выпилится насовсем.")]
    public class AhpEdLineSettings
    {
        public AhpEdLineSettings()
        {
        }

        public AhpEdLineSettings(double firstPriority, double secondPriority, double thirdPriority)
        {
            _firstCriterionPriority = firstPriority;
            _secondCriterionPriority = secondPriority;
            _thirdCriterionPriority = thirdPriority;
        }

        private readonly double _firstCriterionPriority = 0.4;
        private readonly double _secondCriterionPriority = 0.35;
        private readonly double _thirdCriterionPriority = 0.25;

        public double FirstCriterionPriority
        {
            get { return _firstCriterionPriority; }
        }

        public double SecondCriterionPriority
        {
            get { return _secondCriterionPriority; }
        }

        public double ThirdCriterionPriority
        {
            get { return _thirdCriterionPriority; }
        }
    }
}