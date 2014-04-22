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

            // основные данные
            BaseEntitiesInit(context);
            // для asp.net identity контескта(пользователи, права)
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
            //Принял решение не использовать метод AddOrUpdate - с ним много проблем. 
            //В текущей версии просто 1 раз заполняются данные, если не существуют.
            //Соответсвеноо, если добавлять новые записи - они добавятся при обновлении.
            //Если менять существующие - они не обновятся(только если пересоздавать базу)

            var cities = CityInit(context);

            var higherEducationInstitutions = HigherEducationInsitutionInit(context, cities);

            FacultiesInit(context, higherEducationInstitutions);

            GeneralEducationLinesInit(context);

            var сharacterisics = CharacteristicInit(context);

            //Каждый объект(напр. дисциплина егэ математики) может иметь несколько весов. 
            //Но их сумма = 1. В данном случае это упрощение, которое в дальнейшем, возможно, потребуется поправить.

            Exams_Weight(context, сharacterisics);

            SchoolDisciplines_Weight(context, сharacterisics);

            Olympiads_Weight(context, сharacterisics);

            Sections_Weight(context, сharacterisics);

            Schools_Weight(context, сharacterisics);

            Hobbies_Weight(context, сharacterisics);


            EducationLines_Requirements(context);
        }

        private static List<Characteristic> CharacteristicInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            //на данный момент формируется на основе егэ экзаменов.
            //В дальнейшем необходимо подумать и перерасбить более разумно
            //В идеале разделить на "предметы", "физ данные", "прочие"
            var сharacterisics = new List<Characteristic>
            {
                new Characteristic {Name = "Математика",Type=CharacteristicType.Education},
                new Characteristic {Name = "Информатика",Type=CharacteristicType.Education},
                new Characteristic {Name = "Физика",Type=CharacteristicType.Education},
                new Characteristic {Name = "Химия",Type=CharacteristicType.Education},
                new Characteristic {Name = "Биология",Type=CharacteristicType.Education},
                new Characteristic {Name = "География",Type=CharacteristicType.Education},

                new Characteristic {Name = "Литература",Type=CharacteristicType.Education},
                new Characteristic {Name = "История",Type=CharacteristicType.Education},
                new Characteristic {Name = "Обществознание",Type=CharacteristicType.Education},

                new Characteristic {Name = "Русский язык",Type=CharacteristicType.Education},
                new Characteristic {Name = "Английский язык",Type=CharacteristicType.Education},
                new Characteristic {Name = "Немецкий язык",Type=CharacteristicType.Education},
                new Characteristic {Name = "Французский язык",Type=CharacteristicType.Education},
                new Characteristic {Name = "Испанский язык",Type=CharacteristicType.Education},

                new Characteristic {Name = "Сила",Type=CharacteristicType.Physical},
                new Characteristic {Name = "Выносливость",Type=CharacteristicType.Physical},
                new Characteristic {Name = "Скорость реакции",Type=CharacteristicType.Physical},
                new Characteristic {Name = "Зрение",Type=CharacteristicType.Physical},
            };
            foreach (var item in сharacterisics)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.Characteristics.SingleOrDefault(p => p.Name == item.Name) == null)
                {
                    context.Characteristics.Add(item);
                }
            }
            context.SaveChanges();
            return сharacterisics;
        }

        private static void GeneralEducationLinesInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            var generalEducationLines = new List<GeneralEducationLine>
            {
                new GeneralEducationLine {Name = "Г Математика и информатика", Code="1"},
                new GeneralEducationLine {Name = "Г Информатика", Code="2" },
                new GeneralEducationLine {Name = "Г Физика", Code="3"}
            };
            foreach (var item in generalEducationLines)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.GeneralEducationLines.SingleOrDefault(p => p.Name == item.Name) == null)
                {
                    context.GeneralEducationLines.Add(item);
                }
            }
            context.SaveChanges();
        }

        private static void FacultiesInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<HigherEducationInstitution> higherEducationInstitutions)
        {
            var faculties = new List<Faculty>
            {
                new Faculty {Name = "Кафедра МГУ1", Prestige = 90, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="МГУ").Id},
                new Faculty {Name = "Кафедра СПбГУ1", Prestige = 80, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="СПбГУ").Id },
                new Faculty {Name = "Кафедра УРГУ1", Prestige = 60, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="УРГУ").Id}
            };
            foreach (var item in faculties)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.Faculties.SingleOrDefault(p => p.Name == item.Name) == null)
                {
                    context.Faculties.Add(item);
                }
            }
            context.SaveChanges();
        }

        private static List<HigherEducationInstitution> HigherEducationInsitutionInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<City> cities)
        {
            var higherEducationInstitutions = new List<HigherEducationInstitution>
            {
                new HigherEducationInstitution { Name = "МГУ", Prestige = 90, CityId=cities.Single(p=>p.Name=="Москва").Id, Type=HigherEducationInstitutionType.University},
                new HigherEducationInstitution { Name = "СПбГУ", Prestige = 80, CityId=cities.Single(p=>p.Name=="Санкт-Петербург").Id, Type=HigherEducationInstitutionType.University  },
                new HigherEducationInstitution { Name = "УРГУ", Prestige = 60, CityId=cities.Single(p=>p.Name=="Екатеринбург").Id, Type=HigherEducationInstitutionType.University }
            };
            foreach (var item in higherEducationInstitutions)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.HigherEducationInstitutions.SingleOrDefault(p => p.Name == item.Name) == null)
                {
                    context.HigherEducationInstitutions.Add(item);
                }
            }
            context.SaveChanges();
            return higherEducationInstitutions;
        }

        private static List<City> CityInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            var cities = new List<City>
            {
                new City { Name = "Москва", Prestige = 90},
                new City { Name = "Санкт-Петербург", Prestige = 80 },
                new City { Name = "Екатеринбург", Prestige = 60 }
            };
            foreach (var item in cities)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.Cities.SingleOrDefault(p => p.Name == item.Name) == null)
                {
                    context.Cities.Add(item);
                }
            }
            context.SaveChanges();
            return cities;
        }

        private static void EducationLines_Requirements(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            var educationLines = new List<EducationLine>
            {
                new EducationLine 
                {
                    Name = "Математика и информатика", 
                    Actual=true,
                    RequiredSum=260, 
                    Code="1122",
                    FacultyId=context.Faculties.First().Id,
                    GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="1").Id,
                    EducationLinesRequirements=new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement 
                        {
                            Requirement=50, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=80, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=70, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id
                        },
                    }
                },
                new EducationLine 
                {
                    Name = "Информатика", 
                    Actual=true,
                    RequiredSum=260, 
                    Code="1123",
                    FacultyId=context.Faculties.First().Id,
                    GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="2").Id,
                    EducationLinesRequirements=new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement 
                        {
                            Requirement=50, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=65, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=85, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Информатика").Id
                        },
                    }
                },
                new EducationLine 
                {
                    Name = "Физика",
                    Actual=true,RequiredSum=220,
                    Code="1124",
                    FacultyId=context.Faculties.First().Id,
                    GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="3").Id,
                    EducationLinesRequirements=new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement 
                        {
                            Requirement=50, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=70, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=80, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Физика").Id
                        },
                    }
                },
                new EducationLine 
                {
                    Name = "Литературы и искуств",
                    Actual=true,RequiredSum=220,
                    Code="6665",
                    FacultyId=context.Faculties.Single(p=>p.Name=="Кафедра СПбГУ1").Id,
                    //GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="3").Id,
                    EducationLinesRequirements=new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement 
                        {
                            Requirement=70, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=50, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=80, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Литература").Id
                        },
                    }
                },
                new EducationLine 
                {
                    Name = "Исторический",
                    Actual=true,RequiredSum=220,
                    Code="6666",
                    FacultyId=context.Faculties.Single(p=>p.Name=="Кафедра СПбГУ1").Id,
                    //GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="3").Id,
                    EducationLinesRequirements=new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement 
                        {
                            Requirement=60, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Русский язык").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=50, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="Математика").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=80, 
                            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="История").Id
                        },
                    }
                }
            };
            foreach (var educationLine in educationLines)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.EducationLines.SingleOrDefault(p => p.Name == educationLine.Name) == null)
                {
                    context.EducationLines.Add(educationLine);
                }
            }
            context.SaveChanges();
        }

        private static void Hobbies_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            var hobbies = new List<Hobbie>
            {
                new Hobbie {Name = "Рисование", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.7},
                }},
                new Hobbie {Name = "Лепка из глины", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.2},
                }},
                new Hobbie {Name = "3-д моделирование", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Информатика"),Coefficient=0.2},
                }},
                new Hobbie {Name = "Пение", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2},
                }},
                new Hobbie {Name = "Игра на гитаре", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.2},
                }},
                new Hobbie {Name = "Культура японии", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Литература"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="История"),Coefficient=0.8},
                }},
            };

            foreach (var hobbie in hobbies)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if(context.Hobbies.SingleOrDefault(p=>p.Name==hobbie.Name)==null)
                {
                    context.Hobbies.Add(hobbie);
                }
            }
            context.SaveChanges();
        }

        private static void Schools_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            var schools = new List<School>
            {
                new School {Name = "Школа английского языка",EducationQuality=100, Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Английский язык"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.2},
                }},
                new School {Name = "Физмат лицей", EducationQuality= 70, Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Физика"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.5},
                }},
                new School {Name = "Обычная школа", EducationQuality=60, Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Русский язык"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Математика"),Coefficient=0.3},
                }},
            };
            foreach (var school in schools)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.Schools.SingleOrDefault(p => p.Name == school.Name) == null)
                {
                    context.Schools.Add(school);
                }
            }
            context.SaveChanges();
        }

        private static void Sections_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            var sections = new List<Section>
            {
                new Section {Name = "Легкая атлетика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.3},
                }},
                new Section {Name = "Тяжелая атлетика", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.2},
                }},
                new Section {Name = "Теннис", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.3},
                }},
                new Section {Name = "Плавание", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.3},
                }},
                new Section {Name = "Боевых искуств", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.4},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Скорость реакции"),Coefficient=0.3},
                }},
                new Section {Name = "Бокс", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Скорость реакции"),Coefficient=0.4},
                }},
                new Section {Name = "Футбол", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Зрение"),Coefficient=0.3},
                }},
                new Section {Name = "Волейбол", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Зрение"),Coefficient=0.3},
                }},
                new Section {Name = "Баскетбол", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Выносливость"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Сила"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="Зрение"),Coefficient=0.3},
                }},
            };
            foreach (var section in sections)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.Sections.SingleOrDefault(p => p.Name == section.Name) == null)
                {
                    context.Sections.Add(section);
                }
            }
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
            foreach (var olympiad in olympiads)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.Olympiads.SingleOrDefault(p => p.Name == olympiad.Name) == null)
                {
                    context.Olympiads.Add(olympiad);
                }
            }
            context.SaveChanges();
        }

        private static void SchoolDisciplines_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            //На данный момент только за 11 класс
            var schoolDisciplines = new List<SchoolDiscipline>
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
            foreach (var discipline in schoolDisciplines)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.SchoolDisciplines.SingleOrDefault(p => p.Name == discipline.Name) == null)
                {
                    context.SchoolDisciplines.Add(discipline);
                }
            }
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
            foreach (var discipline in examDisciplines)
            {
                //Если нету такой записи - добавляем. Иначе игнорируем(не надо обновлять)
                if (context.ExamDisciplines.SingleOrDefault(p => p.Name == discipline.Name) == null)
                {
                    context.ExamDisciplines.Add(discipline);
                }
            }
            context.SaveChanges();
        }
    }
}
