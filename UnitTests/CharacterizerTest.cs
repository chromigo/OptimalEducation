using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OptimalEducation.DAL.Models;
using OptimalEducation.Logic.Characterizer;

namespace UnitTests
{
    [TestClass]
    public class CharacterizerTest
    {
        List<ExamDiscipline> examDisciplines=new List<ExamDiscipline>();
        List<SchoolDiscipline> schoolDisciplines=new List<SchoolDiscipline>();
        List<Olympiad> olympiads = new List<Olympiad>();
        List<Section> sections = new List<Section>();
        List<Hobbie> hobbies = new List<Hobbie>();
        List<School> schools = new List<School>();
        List<Cluster> clusters;
        //MethodName_Scenario_ExpectedBehavior
        public CharacterizerTest()
        {
            clusters = new List<Cluster>
            {
                new Cluster { Name = "Русский язык"},
                new Cluster { Name = "Математика"},
                new Cluster { Name = "Информатика"},
                new Cluster { Name = "Физика"},
                new Cluster { Name = "Химия"},
                new Cluster { Name = "Английский язык"},
            };

            ExamFill();

            SchoolDisciplineFill();

            OlympiadFill();

            SectionFill();

            HobbieFill();
            SchoolTypeFill();
        }
        private void SchoolTypeFill()
        {
            schools.Add(new School
            {
                Name = "{Школа по Математике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=100,Cluster=clusters.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=50,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                    },
            });
            schools.Add(new School
            {
                Name = "Школа по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=100,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=50,Cluster=clusters.Find(p=>p.Name=="Математика")},
                    },
            });
        }
        private void HobbieFill()
        {
            hobbies.Add(new Hobbie
            {
                Name = "{Хобби(лол) по Математике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=100,Cluster=clusters.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=50,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                    },
            });
            hobbies.Add(new Hobbie
            {
                Name = "Хобби(лол) по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=100,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=50,Cluster=clusters.Find(p=>p.Name=="Математика")},
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
                        new Weight(){Coefficient=100,Cluster=clusters.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=50,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                    },
            });
            sections.Add(new Section
            {
                Name = "Секция(лол) по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=100,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=50,Cluster=clusters.Find(p=>p.Name=="Математика")},
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
                        new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                        new Weight(){Coefficient=0.5,Cluster=clusters.Find(p=>p.Name=="Математика")},
                    },
            });
            olympiads.Add(new Olympiad
            {
                Name = "Олимпиада по Информатике",
                Weights = new List<Weight>()
                    {
                        new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Математика")},
                        new Weight(){Coefficient=0.5,Cluster=clusters.Find(p=>p.Name=="Информатика")},
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
                    new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Русский язык")},
                },
            });
            schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Математика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Математика")},
                    new Weight(){Coefficient=0.5,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                },
            });
            schoolDisciplines.Add(new SchoolDiscipline
            {
                Name = "Информатика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                    new Weight(){Coefficient=0.5,Cluster=clusters.Find(p=>p.Name=="Математика")},
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
                    new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Русский язык")},
                },
            });
            examDisciplines.Add(new ExamDiscipline
            {
                Name = "Математика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Математика")},
                    new Weight(){Coefficient=0.5,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                },
            });
            examDisciplines.Add(new ExamDiscipline
            {
                Name = "Информатика",
                Weights = new List<Weight>()
                {
                    new Weight(){Coefficient=1,Cluster=clusters.Find(p=>p.Name=="Информатика")},
                    new Weight(){Coefficient=0.5,Cluster=clusters.Find(p=>p.Name=="Математика")},
                },
            });
        }

        [TestMethod]
        public void EntrantCluster_GetCalculatedCluster_ReturnCorrectValue()
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
                        Result = 50,
                        SchoolDiscipline=schoolDisciplines[0],
                    },
                    new SchoolMark()
                    {
                        Result = 60,
                        SchoolDiscipline=schoolDisciplines[1],
                    },
                    new SchoolMark()
                    {
                        Result = 70,
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
            };

            var clusterizer = new EntrantCharacterizer(entrant);

            var rus = clusterizer.Characterisics["Русский язык"];
            var math = clusterizer.Characterisics["Математика"];
            var inf = clusterizer.Characterisics["Информатика"];

            //для данных значений (50,60,70 для егэ и школьн оценок должно получаться след. значение)
            //TODO: Подправить значения в зависимости от Enum-ов
            Assert.AreEqual(rus, 100);
            Assert.AreEqual(math, 190+105+(1.5*100+0.5*50)+(100+50)+(50*2));
            Assert.AreEqual(inf, 200 + 105 + (0.5 * 100 + 1.5 * 50)+(100+50)+(100*2));
        }

        [TestMethod]
        public void EducationLineCluster_GetCalculatedCluster_ReturnCorrectValue()
        {
            var educationLine = new EducationLine
            {
                EducationLinesRequirements = new List<EducationLineRequirement>
                {
                    new EducationLineRequirement{Requirement=60, ExamDiscipline=examDisciplines[0]},
                    new EducationLineRequirement{Requirement=70, ExamDiscipline=examDisciplines[1]},
                    new EducationLineRequirement{Requirement=80, ExamDiscipline=examDisciplines[2]},
                }
            };

            var clusterizer = new EducationLineCharacterizer(educationLine);

            var rus = clusterizer.Characteristics["Русский язык"];
            var math = clusterizer.Characteristics["Математика"];
            var inf = clusterizer.Characteristics["Информатика"];

            //для данных значений (50,60,70 для егэ и школьн оценок должно получаться след. значение)
            Assert.AreEqual(rus, 60);
            Assert.AreEqual(math, 70*1+80*0.5);
            Assert.AreEqual(inf, 80*1+70*0.5);

        }
    }
}
