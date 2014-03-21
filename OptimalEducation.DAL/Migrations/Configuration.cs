namespace OptimalEducation.DAL.Migrations
{
    using OptimalEducation.DAL.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OptimalEducationDbContext>
    {
        OptimalEducationDbContext db = new OptimalEducationDbContext();
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OptimalEducationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            BaseEntitiesInit(context);
            //TODO: добавить заполнение инфы о вузе(направления обучения и пр)+ остальное на артема

            CreateEntrant();
        }

        private void CreateEntrant()
        {
            if(db.Entrants.SingleOrDefault(p=>p.Id==1)==null)
            {
                //Создаем абитуриента с базовой информацией
                var entrant = new Entrant()
                {
                    Id = 1,
                    FirstName = "Alice",
                    Gender = "Female",
                };
                //Добавляем ему результаты по ЕГЭ
                foreach (var discipline in db.ExamDisciplines)
                {
                    entrant.UnitedStateExams.Add(
                        new UnitedStateExam
                        {
                            Discipline = discipline,
                            Entrant = entrant,
                            Result = 50,
                        });
                }
                //Добавляем ему результаты по школьным предметам
                foreach (var schoolDisc in db.SchoolDisciplines)
                {
                    entrant.SchoolMarks.Add(
                        new SchoolMark
                        {
                            SchoolDiscipline = schoolDisc,
                            Entrant = entrant,
                            Result = 4,
                        });
                }
                db.Entrants.Add(entrant);
                db.SaveChanges();
            }
        }

        private static void BaseEntitiesInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            var cities = new List<City>
            {
                new City { Id=1, Name = "Москва", Prestige = 90 },
                new City { Id=2, Name = "Санкт-Петербург", Prestige = 80 },
                new City { Id=3, Name = "Екатеринбург", Prestige = 60 }
            };
            cities.ForEach(s => context.Cities.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var higherEducationInstitutions = new List<HigherEducationInstitution>
            {
                new HigherEducationInstitution { Id=1, Name = "МГУ", Prestige = 90, CityId=cities.Single(p=>p.Name=="Москва").Id, Type=HigherEducationInstitutionType.University},
                new HigherEducationInstitution { Id=2, Name = "СПбГУ", Prestige = 80, CityId=cities.Single(p=>p.Name=="Санкт-Петербург").Id, Type=HigherEducationInstitutionType.University  },
                new HigherEducationInstitution { Id=3, Name = "УРГУ", Prestige = 60, CityId=cities.Single(p=>p.Name=="Екатеринбург").Id, Type=HigherEducationInstitutionType.University }
            };
            higherEducationInstitutions.ForEach(s => context.HigherEducationInstitutions.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var faculties = new List<Faculty>
            {
                new Faculty {Id=1, Name = "Кафедра МГУ1", Prestige = 90, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="МГУ").Id},
                new Faculty {Id=2, Name = "Кафедра СПбГУ1", Prestige = 80, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="СПбГУ").Id },
                new Faculty {Id=3, Name = "Кафедра УРГУ1", Prestige = 60, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="УРГУ").Id}
            };
            faculties.ForEach(s => context.Faculties.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var educationLines = new List<EducationLine>
            {
                new EducationLine {Id=1, Name = "Математика и информатика", Actual=true,RequiredSum=250, Code="1122",FacultyId=context.Faculties.First().Id},
                new EducationLine {Id=2, Name = "Информатика", Actual=true,RequiredSum=260, Code="1122",FacultyId=context.Faculties.First().Id },
                new EducationLine {Id=3, Name = "Физика", Actual=true,RequiredSum=220, Code="1122",FacultyId=context.Faculties.First().Id}
            };
            educationLines.ForEach(s => context.EducationLines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var clusters = new List<Cluster>
            {
                new Cluster { Id=1, Name = "Русский язык"},
                new Cluster { Id=2, Name = "Математика"},
                new Cluster { Id=3, Name = "Информатика"},
                new Cluster { Id=4, Name = "Физика"},
                new Cluster { Id=5, Name = "Химия"},
                new Cluster { Id=6, Name = "Английский язык"},
            };
            clusters.ForEach(s => context.Clusters.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var disciplines = new List<ExamDiscipline>
            {
                new ExamDiscipline { Id=1, Name = "Русский язык"},
                new ExamDiscipline { Id=2, Name = "Математика"},
                new ExamDiscipline { Id=3, Name = "Информатика"},
                new ExamDiscipline { Id=4, Name = "Физика"},
                new ExamDiscipline { Id=5, Name = "Химия"},
                new ExamDiscipline { Id=6, Name = "Английский язык"},
            };
            disciplines.ForEach(s => context.ExamDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            var schoolDiscipline = new List<SchoolDiscipline>
            {
                new SchoolDiscipline {Id=1, Name = "Русский язык"},
                new SchoolDiscipline {Id=2, Name = "Математика"},
                new SchoolDiscipline {Id=3, Name = "Информатика"},
                new SchoolDiscipline {Id=4, Name = "Физика"},
                new SchoolDiscipline {Id=5, Name = "Химия"},
                new SchoolDiscipline {Id=6, Name = "Английский язык"},
            };
            schoolDiscipline.ForEach(s => context.SchoolDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            var olympiads = new List<Olympiad>
            {
                new Olympiad {Id=1, Name = "Русский язык"},
                new Olympiad {Id=2, Name = "Математика"},
                new Olympiad {Id=3, Name = "Информатика"},
                new Olympiad {Id=4, Name = "Физика"},
                new Olympiad {Id=5, Name = "Химия"},
                new Olympiad {Id=6, Name = "Английский язык"},
            };
            olympiads.ForEach(s => context.Olympiads.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            var sections = new List<Section>
            {
                new Section {Id=1, Name = "Бег"},
                new Section {Id=2, Name = "Матанир"},
                new Section {Id=3, Name = "Программир"},
                new Section {Id=4, Name = "Бомб"},
                new Section {Id=5, Name = "Азот"},
                new Section {Id=6, Name = "Инглишмен"},
            };
            sections.ForEach(s => context.Sections.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var schools = new List<School>
            {
                new School { Id=1, Name = "Школа русского",EducationQuality=3},
                new School { Id=2, Name = "Школа матана", EducationQuality= 2},
                new School { Id=3, Name = "Школа проги", EducationQuality=1}
            };
            schools.ForEach(s => context.Schools.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var hobbies = new List<Hobbie>
            {
                new Hobbie {Id=1, Name = "Хобби Русский язык"},
                new Hobbie {Id=2, Name = "Хобби Математика"},
                new Hobbie {Id=3, Name = "Хобби Информатика"},
                new Hobbie {Id=4, Name = "Хобби Физика"},
                new Hobbie {Id=5, Name = "Хобби Химия"},
                new Hobbie {Id=6, Name = "Хобби Английский язык"},
            };
            hobbies.ForEach(s => context.Hobbies.AddOrUpdate(s));
            context.SaveChanges();



            var educationLineRequirement = new List<EducationLineRequirement>
            {
                new EducationLineRequirement 
                {
                    Id=1, 
                    Requirement=50, 
                    EducationLineId=context.EducationLines.Single(p=>p.Name=="Математика и информатика").Id, 
                    ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id
                },
                new EducationLineRequirement 
                {
                    Id=2, 
                    Requirement=70, 
                    EducationLineId=context.EducationLines.Single(p=>p.Name=="Математика и информатика").Id, 
                    ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id
                },
                new EducationLineRequirement 
                {
                    Id=3, 
                    Requirement=70, 
                    EducationLineId=context.EducationLines.Single(p=>p.Name=="Математика и информатика").Id, 
                    ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id
                },
            };
            educationLineRequirement.ForEach(s => context.EducationLineRequirements.AddOrUpdate(s));
            context.SaveChanges();

            var weights = new List<Weight>
            {
                new Weight {Id=1, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Русский язык").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id},
                new Weight {Id=2, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=3, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=4, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id},
                new Weight {Id=5, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id},

                new Weight {Id=6, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Русский язык").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Русский язык").Id},
                new Weight {Id=7, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=8, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=9, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Информатика").Id},
                new Weight {Id=10, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Информатика").Id},

                new Weight {Id=11, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Русский язык").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Русский язык").Id},
                new Weight {Id=12, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=13, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=14, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Информатика").Id},
                new Weight {Id=15, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Информатика").Id},

                new Weight {Id=16, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Русский язык").Id,SectionId=context.Sections.Single(p=>p.Name=="Бег").Id},
                new Weight {Id=17, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,SectionId=context.Sections.Single(p=>p.Name=="Матанир").Id},
                new Weight {Id=18, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,SectionId=context.Sections.Single(p=>p.Name=="Матанир").Id},
                new Weight {Id=19, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,SectionId=context.Sections.Single(p=>p.Name=="Программир").Id},
                new Weight {Id=20, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,SectionId=context.Sections.Single(p=>p.Name=="Программир").Id},

                new Weight {Id=21, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Русский язык").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа русского").Id},
                new Weight {Id=22, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа матана").Id},
                new Weight {Id=23, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа матана").Id},
                new Weight {Id=24, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа проги").Id},
                new Weight {Id=25, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа проги").Id},

                new Weight {Id=26, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Русский язык").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Русский язык").Id},
                new Weight {Id=27, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Математика").Id},
                new Weight {Id=28, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Математика").Id},
                new Weight {Id=29, Coefficient = 1, ClusterId = context.Clusters.Single(p=>p.Name=="Информатика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Информатика").Id},
                new Weight {Id=30, Coefficient = 0.7, ClusterId = context.Clusters.Single(p=>p.Name=="Математика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Информатика").Id},
            };
            weights.ForEach(s => context.Weights.AddOrUpdate(s));
            context.SaveChanges();
        }
    }
}
