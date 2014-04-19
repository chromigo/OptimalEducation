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

            var examDisciplines = new List<ExamDiscipline>
            {
                new ExamDiscipline {Name = "Русский язык"},
                new ExamDiscipline {Name = "Математика"},

                new ExamDiscipline {Name = "Информатика"},
                new ExamDiscipline {Name = "Физика"},
                new ExamDiscipline {Name = "Химия"},
                new ExamDiscipline {Name = "Биология"},
                new ExamDiscipline {Name = "География"},

                new ExamDiscipline {Name = "Обществознание"},
                new ExamDiscipline {Name = "История"},
                new ExamDiscipline {Name = "Литература"},

                new ExamDiscipline {Name = "Английский язык"},
                new ExamDiscipline {Name = "Немецкий язык"},
                new ExamDiscipline {Name = "Французский язык"},
                new ExamDiscipline {Name = "Испанский язык"},
            };
            examDisciplines.ForEach(s => context.ExamDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            //На данный момент только за 11 класс
            var schoolDiscipline = new List<SchoolDiscipline>
            {
                new SchoolDiscipline {Name = "Алгебра"},
                new SchoolDiscipline {Name = "Геометрия"},
                new SchoolDiscipline {Name = "Информатика"},
                new SchoolDiscipline {Name = "Физика"},
                new SchoolDiscipline {Name = "Химия"},
                new SchoolDiscipline {Name = "Биология"},
                new SchoolDiscipline {Name = "География"},

                new SchoolDiscipline {Name = "История"},
                new SchoolDiscipline {Name = "Обществознание"},
                new SchoolDiscipline {Name = "Литература"},
                
                new SchoolDiscipline {Name = "Русский язык"},
                new SchoolDiscipline {Name = "Английский язык"},
                new SchoolDiscipline {Name = "Немецкий язык"},
                new SchoolDiscipline {Name = "Французский язык"},
                new SchoolDiscipline {Name = "Испанский язык"},
            };
            schoolDiscipline.ForEach(s => context.SchoolDisciplines.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
            var olympiads = new List<Olympiad>
            {
                new Olympiad {Name = "Русский язык"},
                new Olympiad {Name = "Математика"},
                new Olympiad {Name = "Информатика"},
                new Olympiad {Name = "Физика"},
                new Olympiad {Name = "Химия"},
                new Olympiad {Name = "Английский язык"},
            };
            olympiads.ForEach(s => context.Olympiads.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();
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

            //Здесь задаем начальные веса для наших предметов, олимпиада, школ и пр.
            var weights = new List<Weight>
            {
                //(смотреть справа - налево)
                //ЕГЭ
                new Weight {Id=1, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Русский язык").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id},
                new Weight {Id=2, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=3, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=4, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id},
                new Weight {Id=5, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id},
                //Школьные предметы
                new Weight {Id=6, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Русский язык").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Русский язык").Id},
                new Weight {Id=7, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Алгебра").Id},
                new Weight {Id=8, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Алгебра").Id},
                new Weight {Id=9, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Информатика").Id},
                new Weight {Id=10, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,SchoolDisciplineId=context.SchoolDisciplines.Single(p=>p.Name=="Информатика").Id},
                //Олимпиады
                new Weight {Id=11, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Русский язык").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Русский язык").Id},
                new Weight {Id=12, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=13, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Математика").Id},
                new Weight {Id=14, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Информатика").Id},
                new Weight {Id=15, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,OlympiadId=context.Olympiads.Single(p=>p.Name=="Информатика").Id},
                //Секции
                new Weight {Id=16, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Русский язык").Id,SectionId=context.Sections.Single(p=>p.Name=="Бег").Id},
                new Weight {Id=17, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,SectionId=context.Sections.Single(p=>p.Name=="Матанир").Id},
                new Weight {Id=18, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,SectionId=context.Sections.Single(p=>p.Name=="Матанир").Id},
                new Weight {Id=19, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,SectionId=context.Sections.Single(p=>p.Name=="Программир").Id},
                new Weight {Id=20, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,SectionId=context.Sections.Single(p=>p.Name=="Программир").Id},
                //Школа
                new Weight {Id=21, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Русский язык").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа русского").Id},
                new Weight {Id=22, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа матана").Id},
                new Weight {Id=23, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа матана").Id},
                new Weight {Id=24, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа проги").Id},
                new Weight {Id=25, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,SchoolId=context.Schools.Single(p=>p.Name=="Школа проги").Id},
                //Хобби
                new Weight {Id=26, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Русский язык").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Русский язык").Id},
                new Weight {Id=27, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Математика").Id},
                new Weight {Id=28, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Математика").Id},
                new Weight {Id=29, Coefficient = 1, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Информатика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Информатика").Id},
                new Weight {Id=30, Coefficient = 0.7, CharacterisicId = context.Characteristics.Single(p=>p.Name=="Математика").Id,HobbieId=context.Hobbies.Single(p=>p.Name=="Хобби Информатика").Id},
            };
            weights.ForEach(s => context.Weights.AddOrUpdate(s));
            context.SaveChanges();
        }
    }
}
