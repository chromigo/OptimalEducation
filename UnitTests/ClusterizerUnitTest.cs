using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimalEducation.Logic.Clusterizer;
using OptimalEducation.Models;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class ClusterizerUnitTest
    {
        List<ExamDiscipline> examDisciplines=new List<ExamDiscipline>();
        List<SchoolDiscipline> schoolDisciplines=new List<SchoolDiscipline>();
        List<Cluster> clusters;
        //MethodName_Scenario_ExpectedBehavior
        public ClusterizerUnitTest()
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

        [TestMethod]
        public void EntrantCluster_GetResultSum_ReturnCorrectValue()
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
                }
            };

            var clusterizer = new EntrantClusterizer(entrant);

            var rus = clusterizer.Cluster["Русский язык"];
            var math = clusterizer.Cluster["Математика"];
            var inf = clusterizer.Cluster["Информатика"];

            //для данных значений (50,60,70 для егэ и школьн оценок должно получаться след. значение)
            Assert.AreEqual(rus, 100);
            Assert.AreEqual(math, 190);
            Assert.AreEqual(inf, 200);
        }
    }
}
