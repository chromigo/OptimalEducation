using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.DAL.Models;

namespace UnitTests
{
    [TestClass]
    public class CharacterizerTest
    {
        #region Fields
        List<ExamDiscipline> examDisciplines = new List<ExamDiscipline>();
        List<SchoolDiscipline> schoolDisciplines = new List<SchoolDiscipline>();
        List<Olympiad> olympiads = new List<Olympiad>();
        List<Section> sections = new List<Section>();
        List<Hobbie> hobbies = new List<Hobbie>();
        List<School> schools = new List<School>();
        List<Characteristic> characterisics; 
        #endregion
        //MethodName_Scenario_ExpectedBehavior
        public CharacterizerTest()
        {
            characterisics = new List<Characteristic>
            {
                new Characteristic { Name = "Русский язык",Type=CharacteristicType.Education},
                new Characteristic { Name = "Математика",Type=CharacteristicType.Education},
                new Characteristic { Name = "Информатика",Type=CharacteristicType.Education},
                new Characteristic { Name = "Физика",Type=CharacteristicType.Education},
                new Characteristic { Name = "Химия",Type=CharacteristicType.Education},
                new Characteristic { Name = "Английский язык",Type=CharacteristicType.Education},
            };

            ExamFill();

            SchoolDisciplineFill();

            OlympiadFill();

            SectionFill();

            HobbieFill();
            SchoolTypeFill();
        }
        #region Init helpers
        private void SchoolTypeFill()
        {
            schools.Add(new School
            {
                Name = "Школа по Математике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                    },
                    EducationQuality=90
            });
            schools.Add(new School
            {
                Name = "Школа по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                    },
                EducationQuality = 87
            });
        }
        private void HobbieFill()
        {
            hobbies.Add(new Hobbie
            {
                Name = "Хобби(лол) по Математике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                    },
            });
            hobbies.Add(new Hobbie
            {
                Name = "Хобби(лол) по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                    },
            });
        }
        private void SectionFill()
        {
            sections.Add(new Section
            {
                Name = "Секция(лол) по Математике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                    },
            });
            sections.Add(new Section
            {
                Name = "Секция(лол) по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                    },
            });
        }
        private void OlympiadFill()
        {
            olympiads.Add(new Olympiad
            {
                Name = "Олимпиада по Математике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                    },
            });
            olympiads.Add(new Olympiad
            {
                Name = "Олимпиада по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                    },
            });
        }
        private void SchoolDisciplineFill()
        {
            schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Русский язык",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Русский язык")},
                },
            });
            schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Математика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                    new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                },
            });
            schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Информатика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                    new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                },
            });
        }
        private void ExamFill()
        {
            examDisciplines.Add(new ExamDiscipline
            {
                Name = "Русский язык",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Русский язык")},
                },
            });
            examDisciplines.Add(new ExamDiscipline
            {
                Name = "Математика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                    new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                },
            });
            examDisciplines.Add(new ExamDiscipline
            {
                Name = "Информатика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=0.7,Characterisic=characterisics.Find(p=>p.Name=="Информатика")},
                    new Weight(){Coefficient=0.3,Characterisic=characterisics.Find(p=>p.Name=="Математика")},
                },
            });
        }

        private Entrant CreateEntrant()
        {
            var entrant = new Entrant()
            {
                UnitedStateExams = new List<UnitedStateExam>()
                {
                    new UnitedStateExam()
                    {
                        Result = 50,
                        Discipline=examDisciplines[0],
                    },
                    new UnitedStateExam()
                    {
                        Result = 60,
                        Discipline=examDisciplines[1],
                    },
                    new UnitedStateExam()
                    {
                        Result = 70,
                        Discipline=examDisciplines[2],
                    },
                },
                SchoolMarks = new List<SchoolMark>()
                {
                    new SchoolMark()
                    {
                        Result = 5,
                        SchoolDiscipline=schoolDisciplines[0],
                    },
                    new SchoolMark()
                    {
                        Result = 4,
                        SchoolDiscipline=schoolDisciplines[1],
                    },
                    new SchoolMark()
                    {
                        Result = 5,
                        SchoolDiscipline=schoolDisciplines[2],
                    },
                },
                ParticipationInOlympiads = new List<ParticipationInOlympiad> 
                {
                    new ParticipationInOlympiad
                    {
                        Result=OlypmpiadResult.SecondPlace,//70
                        Olympiad=olympiads[0]
                    },
                    new ParticipationInOlympiad
                    {
                        Result=OlypmpiadResult.SecondPlace,
                        Olympiad=olympiads[1]
                    }
                },
                ParticipationInSections = new List<ParticipationInSection> 
                {
                    new ParticipationInSection
                    {
                        YearPeriod=1.5,
                        Section=sections[0]
                    },
                    new ParticipationInSection
                    {
                        YearPeriod=0.5,
                        Section=sections[1]
                    }
                },
                ParticipationInSchools = new List<ParticipationInSchool> 
                {
                    new ParticipationInSchool
                    {
                        YearPeriod=5,
                        School=schools[0]
                    },
                    new ParticipationInSchool
                    {
                        YearPeriod=6,
                        School=schools[1]
                    }
                },
                Hobbies = hobbies
            };
            return entrant;
        } 
        #endregion
    }
}
