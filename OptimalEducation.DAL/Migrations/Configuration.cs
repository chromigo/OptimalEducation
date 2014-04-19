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
                new City { Id=1, Name = "Москва", Prestige = 90},
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

            var generalEducationLines = new List<GeneralEducationLine>
            {
                new GeneralEducationLine {Id=1, Name = "Г Математика и информатика", Code="1"},
                new GeneralEducationLine {Id=2, Name = "Г Информатика", Code="2" },
                new GeneralEducationLine {Id=3, Name = "Г Физика", Code="3"}
            };
            generalEducationLines.ForEach(s => context.GeneralEducationLines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var educationLines = new List<EducationLine>
            {
                new EducationLine {Id=1, Name = "Математика и информатика", Actual=true,RequiredSum=250, Code="1122",
                    FacultyId=context.Faculties.First().Id,
                    GeneralEducationLine=context.GeneralEducationLines.Single(p=>p.Code=="1")},
                new EducationLine {Id=2, Name = "Информатика", Actual=true,RequiredSum=260, Code="1122",
                    FacultyId=context.Faculties.First().Id,
                    GeneralEducationLine=context.GeneralEducationLines.Single(p=>p.Code=="2")},
                new EducationLine {Id=3, Name = "Физика", Actual=true,RequiredSum=220, Code="1122",
                    FacultyId=context.Faculties.First().Id,
                    GeneralEducationLine=context.GeneralEducationLines.Single(p=>p.Code=="3")}
            };
            educationLines.ForEach(s => context.EducationLines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            //на данный момент формируется на основе егэ экзаменов.
            //В дальнейшем необходимо подумать и перерасбить более разумно
            var Characterisics = new List<Characteristic>
            {
                new Characteristic {Name = "Русский язык"},
                new Characteristic {Name = "Математика"},
                new Characteristic {Name = "Информатика"},
                new Characteristic {Name = "Физика"},
                new Characteristic {Name = "Химия"},
                new Characteristic {Name = "Английский язык"},
                new Characteristic {Name = "Литература"},
                new Characteristic {Name = "История"},
                new Characteristic {Name = "Обществознание"},
                new Characteristic {Name = "Биология"},
                new Characteristic {Name = "География"},
                new Characteristic {Name = "Немецкий язык"},
                new Characteristic {Name = "Французский язык"},
                new Characteristic {Name = "Испанский язык"},
            };
            Characterisics.ForEach(s => context.Characteristics.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            Exams_Weight(context, Characterisics);
            SchoolDisciplines_Weight(context, Characterisics);
            Olympiads_Weight(context, Characterisics);
            var sections = new List<Section>
            {
                new Section {Name = "Бег"},
                new Section {Name = "Матанир"},
                new Section {Name = "Программир"},
                new Section {Name = "Бомб"},
                new Section {Name = "Азот"},
                new Section {Name = "Инглишмен"},
            };
            sections.ForEach(s => context.Sections.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var schools = new List<School>
            {
                new School {Name = "Школа русского",EducationQuality=3},
                new School {Name = "Школа матана", EducationQuality= 2},
                new School {Name = "Школа проги", EducationQuality=1}
            };
            schools.ForEach(s => context.Schools.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var hobbies = new List<Hobbie>
            {
                new Hobbie {Name = "Хобби Русский язык"},
                new Hobbie {Name = "Хобби Математика"},
                new Hobbie {Name = "Хобби Информатика"},
                new Hobbie {Name = "Хобби Физика"},
                new Hobbie {Name = "Хобби Химия"},
                new Hobbie {Name = "Хобби Английский язык"},
            };
            hobbies.ForEach(s => context.Hobbies.AddOrUpdate(p => p.Name, s));
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
        }

        private static void Olympiads_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            //Тут кучи разных, есть даже экзотические(я так понля для поступления в соотвествующий вуз)
            //Пока заполним малую часть(и не частные, а общие)
            var olympiads = new List<Olympiad>
            {
                new Olympiad {Name = "Русский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.1},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.1},
                }},
                new Olympiad {Name = "Математика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new Olympiad {Name = "Информатика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.1}
                }},
                new Olympiad {Name = "Физика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.2}
                }},
                new Olympiad {Name = "Химия", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                }},
                new Olympiad {Name = "Английский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Немецкий язык"),Coefficient=0.1},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.1},
                }},
            };
            olympiads.ForEach(s => context.Olympiads.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
        }

        private static void SchoolDisciplines_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            //На данный момент только за 11 класс
            var schoolDiscipline = new List<SchoolDiscipline>
            {
                new SchoolDiscipline {Name = "Алгебра", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Геометрия", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="География"),Coefficient=0.1}
                }},
                new SchoolDiscipline {Name = "Информатика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Физика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Химия", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Биология", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Биология"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "География", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="География"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Биология"),Coefficient=0.2}
                }},

                new SchoolDiscipline {Name = "История", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Обществознание", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Обществознание"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2},
                }},
                new SchoolDiscipline {Name = "Литература", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2}
                }},
                
                new SchoolDiscipline {Name = "Русский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Английский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Немецкий язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Немецкий язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Немецкий язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Французский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Французский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "Испанский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Испанский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
            };
            schoolDiscipline.ForEach(s => context.SchoolDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
        }

        private static void Exams_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            var examDisciplines = new List<ExamDiscipline>
            {
                new ExamDiscipline {Name = "Русский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.1},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.1},
                }},
                new ExamDiscipline {Name = "Математика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Информатика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.1}
                }},
                new ExamDiscipline {Name = "Физика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Химия", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Биология", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Биология"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Химия"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "География", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="География"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Биология"),Coefficient=0.2}
                }},

                new ExamDiscipline {Name = "Обществознание", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Обществознание"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "История", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Обществознание"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="География"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Литература", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.2}
                }},

                new ExamDiscipline {Name = "Английский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Немецкий язык"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Немецкий язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Немецкий язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Французский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Французский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "Испанский язык", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Испанский язык"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2}
                }},
            };
            examDisciplines.ForEach(s => context.ExamDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
        }
    }
}
