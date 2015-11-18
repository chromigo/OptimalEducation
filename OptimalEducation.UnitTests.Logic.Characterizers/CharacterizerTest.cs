using System.Collections.Generic;
using NUnit.Framework;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.UnitTests.Logic.Characterizers
{
    [TestFixture]
    public class CharacterizerTest
    {
        //MethodName_Scenario_ExpectedBehavior
        public CharacterizerTest()
        {
            _characterisics = new List<Characteristic>
            {
                new Characteristic {Name = "Русский язык", Type = CharacteristicType.Education},
                new Characteristic {Name = "Математика", Type = CharacteristicType.Education},
                new Characteristic {Name = "Информатика", Type = CharacteristicType.Education},
                new Characteristic {Name = "Физика", Type = CharacteristicType.Education},
                new Characteristic {Name = "Химия", Type = CharacteristicType.Education},
                new Characteristic {Name = "Английский язык", Type = CharacteristicType.Education}
            };

            ExamFill();

            SchoolDisciplineFill();

            OlympiadFill();

            SectionFill();

            HobbieFill();
            SchoolTypeFill();
        }

        #region Fields

        private readonly List<ExamDiscipline> _examDisciplines = new List<ExamDiscipline>();
        private readonly List<SchoolDiscipline> _schoolDisciplines = new List<SchoolDiscipline>();
        private readonly List<Olympiad> _olympiads = new List<Olympiad>();
        private readonly List<Section> _sections = new List<Section>();
        private readonly List<Hobbie> _hobbies = new List<Hobbie>();
        private readonly List<School> _schools = new List<School>();
        private readonly List<Characteristic> _characterisics;

        #endregion

        #region Init helpers

        private void SchoolTypeFill()
        {
            _schools.Add(new School
            {
                Name = "Школа по Математике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Математика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Информатика")}
                },
                EducationQuality = 90
            });
            _schools.Add(new School
            {
                Name = "Школа по Информатике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Информатика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Математика")}
                },
                EducationQuality = 87
            });
        }

        private void HobbieFill()
        {
            _hobbies.Add(new Hobbie
            {
                Name = "Хобби(лол) по Математике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Математика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Информатика")}
                }
            });
            _hobbies.Add(new Hobbie
            {
                Name = "Хобби(лол) по Информатике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Информатика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Математика")}
                }
            });
        }

        private void SectionFill()
        {
            _sections.Add(new Section
            {
                Name = "Секция(лол) по Математике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Математика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Информатика")}
                }
            });
            _sections.Add(new Section
            {
                Name = "Секция(лол) по Информатике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Информатика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Математика")}
                }
            });
        }

        private void OlympiadFill()
        {
            _olympiads.Add(new Olympiad
            {
                Name = "Олимпиада по Математике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Информатика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Математика")}
                }
            });
            _olympiads.Add(new Olympiad
            {
                Name = "Олимпиада по Информатике",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Математика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Информатика")}
                }
            });
        }

        private void SchoolDisciplineFill()
        {
            _schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Русский язык",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Русский язык")}
                }
            });
            _schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Математика",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Математика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Информатика")}
                }
            });
            _schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Информатика",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Информатика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Математика")}
                }
            });
        }

        private void ExamFill()
        {
            _examDisciplines.Add(new ExamDiscipline
            {
                Name = "Русский язык",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Русский язык")}
                }
            });
            _examDisciplines.Add(new ExamDiscipline
            {
                Name = "Математика",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Математика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Информатика")}
                }
            });
            _examDisciplines.Add(new ExamDiscipline
            {
                Name = "Информатика",
                Weights = new List<Weight>
                {
                    new Weight {Coefficient = 0.7, Characterisic = _characterisics.Find(p => p.Name == "Информатика")},
                    new Weight {Coefficient = 0.3, Characterisic = _characterisics.Find(p => p.Name == "Математика")}
                }
            });
        }

        #endregion
    }
}