using System;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.Implementation.Logic.AHP
{
    [Obsolete("АХТУНГ! Страшный говнокод! Был удален и возвращен по причине - нужно чтобы временно работало. В дальнейшем перепишется или выпилится насовсем.")]
    public class AhpUserSettings
    {
        private readonly OptimalEducationDbContext _context = new OptimalEducationDbContext();
        private readonly int _firstCriterionLazyGap = 10;
        private readonly double _firstCriterionPriority = 0.40;
        private readonly int _fourthCriterionCityId;
        private readonly bool _fourthCriterionExactLocation;
        private readonly double _fourthCriterionPriority;
        private readonly double _secondCriterionPriority = 0.35;
        private readonly double _thirdCriterionPriority = 0.25;

        public AhpUserSettings()
        {
            if (_fourthCriterionPriority > 0)
            {
                if (_fourthCriterionCityId != 0)
                {
                    if (_context.Cities.Find(_fourthCriterionCityId).Location == null)
                    {
                        _fourthCriterionExactLocation = true;
                    }
                }
                else _fourthCriterionPriority = 0;
            }
        }

        public AhpUserSettings(double firstPriority, double secondPriority, double thirdPriority, double fourthPriority,
            int lazyGap, bool exactLocation, int cityId)
        {
            _firstCriterionPriority = firstPriority;
            _secondCriterionPriority = secondPriority;
            _thirdCriterionPriority = thirdPriority;
            _fourthCriterionPriority = fourthPriority;

            _firstCriterionLazyGap = lazyGap;
            _fourthCriterionExactLocation = exactLocation;
            _fourthCriterionCityId = cityId;


            if (_fourthCriterionPriority > 0)
            {
                if (_fourthCriterionCityId != 0)
                {
                    if (_context.Cities.Find(_fourthCriterionCityId).Location == null)
                    {
                        _fourthCriterionExactLocation = true;
                    }
                }
                else _fourthCriterionPriority = 0;
            }
        }

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

        public double FourthCriterionPriority
        {
            get { return _fourthCriterionPriority; }
        }

        public int FirstCriterionLazyGap
        {
            get { return _firstCriterionLazyGap; }
        }

        public bool FourthCriterionExactLocation
        {
            get { return _fourthCriterionExactLocation; }
        }

        public int FourthCriterionCityId
        {
            get { return _fourthCriterionCityId; }
        }
    }
}