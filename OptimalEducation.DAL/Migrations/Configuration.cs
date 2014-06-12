namespace OptimalEducation.DAL.Migrations
{
    using OptimalEducation.DAL.Builders;
    using OptimalEducation.DAL.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
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

            // �������� ������
            BaseEntitiesInit(context);
            // ��� asp.net identity ���������(������������, �����)
            CreateCommonEntrant();

            //��������� ������(��� �� ��������, ����� ��� ������������ �����������)
            CreateIdealEntrant();
            CreateIdealEducationLine();

            RunSqlSqripts(context);
        }

        private void RunSqlSqripts(OptimalEducationDbContext context)
        {
            if(context.Cities.Count()==1)//���� ������ ����� �� �������� � �������� ��������
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\SQLScripts";
                //������� �����!
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.Cities.data.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.ExamDisciplines.data.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.HigherEducationInstitutions.data.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.Faculties.data.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.GeneralEducationLines.data.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.EducationLines.data.sql"));
                context.Database.ExecuteSqlCommand(File.ReadAllText(baseDir + "\\dbo.EducationLineRequirements.data.sql"));
            }
        }

        private void CreateCommonEntrant()
        {
            if(db.Entrants.SingleOrDefault(p=>p.Id==1)==null)
            {
                EntrantBuilder.Create("Alice", 1);
            }
        }

        private void CreateIdealEntrant()
        {
            if (db.Entrants.SingleOrDefault(p => p.Id == 2) == null)
            {
                IdealEntrantBuilder.Create();
            }
        }
        private void CreateIdealEducationLine()
        {
            if (db.EducationLines.SingleOrDefault(p => p.Name == "IDEAL") == null)
            {
                var idealEducationLine = new EducationLine
                {
                    Name = "IDEAL",
                    Actual = true,
                    RequiredSum = 300,
                    Code = "000000",
                    FacultyId = db.Faculties.Single(p => p.Name == "default_department").Id,
                    //GeneralEducationLineId = db.GeneralEducationLines.Single(p => p.Code == "1").Id,
                    EducationLinesRequirements = new List<EducationLineRequirement>
                    {
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="������� ����").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="����������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="�����������").Id
                        },

                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="�����").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="��������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="���������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="��������������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="�������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="����������").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="���������� ����").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="�������� ����").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="����������� ����").Id
                        },
                        new EducationLineRequirement 
                        {
                            Requirement=100, 
                            ExamDisciplineId=db.ExamDisciplines.Single(p=>p.Name=="��������� ����").Id
                        },
                    }
                };
                db.EducationLines.Add(idealEducationLine);
                db.SaveChanges();
            }
        }

        private static void BaseEntitiesInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            //������ ������� �� ������������ ����� AddOrUpdate - � ��� ����� �������. 
            //� ������� ������ ������ 1 ��� ����������� ������, ���� �� ����������.
            //�������������, ���� ��������� ����� ������ - ��� ��������� ��� ����������.
            //���� ������ ������������ - ��� �� ���������(������ ���� ������������� ����)

            var cities = CityInit(context);

            var higherEducationInstitutions = HigherEducationInsitutionInit(context, cities);

            FacultiesInit(context, higherEducationInstitutions);

            GeneralEducationLinesInit(context);

            var �haracterisics = CharacteristicInit(context);

            //������ ������(����. ���������� ��� ����������) ����� ����� ��������� �����. 
            //�� �� ����� = 1. � ������ ������ ��� ���������, ������� � ����������, ��������, ����������� ���������.

            Exams_Weight(context, �haracterisics);

            SchoolDisciplines_Weight(context, �haracterisics);

            Olympiads_Weight(context, �haracterisics);

            Sections_Weight(context, �haracterisics);

            Schools_Weight(context, �haracterisics);

            Hobbies_Weight(context, �haracterisics);


            EducationLines_Requirements(context);
        }

        private static List<Characteristic> CharacteristicInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            //�� ������ ������ ����������� �� ������ ��� ���������.
            //� ���������� ���������� �������� � ����������� ����� �������
            //� ������ ��������� �� "��������", "��� ������", "������"
            var �haracterisics = new List<Characteristic>
            {
                new Characteristic {Name = "����������",Type=CharacteristicType.Education},
                new Characteristic {Name = "�����������",Type=CharacteristicType.Education},
                new Characteristic {Name = "������",Type=CharacteristicType.Education},
                new Characteristic {Name = "�����",Type=CharacteristicType.Education},
                new Characteristic {Name = "��������",Type=CharacteristicType.Education},
                new Characteristic {Name = "���������",Type=CharacteristicType.Education},

                new Characteristic {Name = "����������",Type=CharacteristicType.Education},
                new Characteristic {Name = "�������",Type=CharacteristicType.Education},
                new Characteristic {Name = "��������������",Type=CharacteristicType.Education},

                new Characteristic {Name = "������� ����",Type=CharacteristicType.Education},
                new Characteristic {Name = "���������� ����",Type=CharacteristicType.Education},
                new Characteristic {Name = "�������� ����",Type=CharacteristicType.Education},
                new Characteristic {Name = "����������� ����",Type=CharacteristicType.Education},
                new Characteristic {Name = "��������� ����",Type=CharacteristicType.Education},

                new Characteristic {Name = "����",Type=CharacteristicType.Physical},
                new Characteristic {Name = "������������",Type=CharacteristicType.Physical},
                new Characteristic {Name = "�������� �������",Type=CharacteristicType.Physical},
                new Characteristic {Name = "������",Type=CharacteristicType.Physical},
            };
            foreach (var item in �haracterisics)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
                if (context.Characteristics.SingleOrDefault(p => p.Name == item.Name) == null)
                {
                    context.Characteristics.Add(item);
                }
            }
            context.SaveChanges();
            return �haracterisics;
        }

        private static void GeneralEducationLinesInit(OptimalEducation.DAL.Models.OptimalEducationDbContext context)
        {
            var generalEducationLines = new List<GeneralEducationLine>
            {
                //new GeneralEducationLine {Name = "� ���������� � �����������", Code="1"},
                //new GeneralEducationLine {Name = "� �����������", Code="2" },
                //new GeneralEducationLine {Name = "� ������", Code="3"}
            };
            foreach (var item in generalEducationLines)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new Faculty {Name = "default_department", Prestige = 100, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="difault_univercity").Id},
                //new Faculty {Name = "������� ���1", Prestige = 90, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="���").Id},
                //new Faculty {Name = "������� �����1", Prestige = 80, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="�����").Id },
                //new Faculty {Name = "������� ����1", Prestige = 60, HigherEducationInstitutionId=higherEducationInstitutions.Single(p=>p.Name=="����").Id}
            };
            foreach (var item in faculties)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new HigherEducationInstitution { Name = "difault_univercity", Prestige = 100, CityId=cities.Single(p=>p.Name=="default_city").Id, Type=HigherEducationInstitutionType.University},
                //new HigherEducationInstitution { Name = "���", Prestige = 90, CityId=cities.Single(p=>p.Name=="������").Id, Type=HigherEducationInstitutionType.University},
                //new HigherEducationInstitution { Name = "�����", Prestige = 80, CityId=cities.Single(p=>p.Name=="�����-���������").Id, Type=HigherEducationInstitutionType.University  },
                //new HigherEducationInstitution { Name = "����", Prestige = 60, CityId=cities.Single(p=>p.Name=="������������").Id, Type=HigherEducationInstitutionType.University }
            };
            foreach (var item in higherEducationInstitutions)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new City { Name = "default_city", Prestige = 100},
                //new City { Name = "������", Prestige = 90},
                //new City { Name = "�����-���������", Prestige = 80 },
                //new City { Name = "������������", Prestige = 60 }
            };
            foreach (var item in cities)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                //new EducationLine 
                //{
                //    Name = "���������� � �����������", 
                //    Actual=true,
                //    RequiredSum=260, 
                //    Code="1122",
                //    FacultyId=context.Faculties.First().Id,
                //    GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="1").Id,
                //    EducationLinesRequirements=new List<EducationLineRequirement>
                //    {
                //        new EducationLineRequirement 
                //        {
                //            Requirement=50, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="������� ����").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=80, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="����������").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=70, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="�����������").Id
                //        },
                //    }
                //},
                //new EducationLine 
                //{
                //    Name = "�����������", 
                //    Actual=true,
                //    RequiredSum=260, 
                //    Code="1123",
                //    FacultyId=context.Faculties.First().Id,
                //    GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="2").Id,
                //    EducationLinesRequirements=new List<EducationLineRequirement>
                //    {
                //        new EducationLineRequirement 
                //        {
                //            Requirement=50, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="������� ����").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=65, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="����������").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=85, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="�����������").Id
                //        },
                //    }
                //},
                //new EducationLine 
                //{
                //    Name = "������",
                //    Actual=true,RequiredSum=220,
                //    Code="1124",
                //    FacultyId=context.Faculties.First().Id,
                //    GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="3").Id,
                //    EducationLinesRequirements=new List<EducationLineRequirement>
                //    {
                //        new EducationLineRequirement 
                //        {
                //            Requirement=50, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="������� ����").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=70, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="����������").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=80, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="������").Id
                //        },
                //    }
                //},
                //new EducationLine 
                //{
                //    Name = "���������� � �������",
                //    Actual=true,RequiredSum=220,
                //    Code="6665",
                //    FacultyId=context.Faculties.Single(p=>p.Name=="������� �����1").Id,
                //    //GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="3").Id,
                //    EducationLinesRequirements=new List<EducationLineRequirement>
                //    {
                //        new EducationLineRequirement 
                //        {
                //            Requirement=70, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="������� ����").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=50, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="����������").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=80, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="����������").Id
                //        },
                //    }
                //},
                //new EducationLine 
                //{
                //    Name = "������������",
                //    Actual=true,RequiredSum=220,
                //    Code="6666",
                //    FacultyId=context.Faculties.Single(p=>p.Name=="������� �����1").Id,
                //    //GeneralEducationLineId=context.GeneralEducationLines.Single(p=>p.Code=="3").Id,
                //    EducationLinesRequirements=new List<EducationLineRequirement>
                //    {
                //        new EducationLineRequirement 
                //        {
                //            Requirement=60, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="������� ����").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=50, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="����������").Id
                //        },
                //        new EducationLineRequirement 
                //        {
                //            Requirement=80, 
                //            ExamDisciplineId=context.ExamDisciplines.Single(p=>p.Name=="�������").Id
                //        },
                //    }
                //}
            };
            foreach (var educationLine in educationLines)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new Hobbie {Name = "���������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.7},
                }},
                new Hobbie {Name = "����� �� �����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                }},
                new Hobbie {Name = "3-� �������������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.2},
                }},
                new Hobbie {Name = "�����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2},
                }},
                new Hobbie {Name = "���� �� ������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2},
                }},
                new Hobbie {Name = "�������� ������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.8},
                }},
            };

            foreach (var hobbie in hobbies)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new School {Name = "����� ����������� �����",EducationQuality=100, Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2},
                }},
                new School {Name = "������ �����", EducationQuality= 70, Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.5},
                }},
                new School {Name = "������� �����", EducationQuality=60, Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.3},
                }},
            };
            foreach (var school in schools)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new Section {Name = "������ ��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.3},
                }},
                new Section {Name = "������� ��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.2},
                }},
                new Section {Name = "������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.3},
                }},
                new Section {Name = "��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.3},
                }},
                new Section {Name = "������ �������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.4},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� �������"),Coefficient=0.3},
                }},
                new Section {Name = "����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� �������"),Coefficient=0.4},
                }},
                new Section {Name = "������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.3},
                }},
                new Section {Name = "��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.3},
                }},
                new Section {Name = "���������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������������"),Coefficient=0.5},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.3},
                }},
            };
            foreach (var section in sections)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
                if (context.Sections.SingleOrDefault(p => p.Name == section.Name) == null)
                {
                    context.Sections.Add(section);
                }
            }
            context.SaveChanges();
        }

        private static void Olympiads_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            //��� ���� ������, ���� ���� ������������(� ��� ����� ��� ����������� � �������������� ���)
            //���� �������� ����� �����(� �� �������, � �����)
            var olympiads = new List<Olympiad>
            {
                new Olympiad {Name = "������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.1},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.1},
                }},
                new Olympiad {Name = "����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new Olympiad {Name = "�����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.1}
                }},
                new Olympiad {Name = "������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.2}
                }},
                new Olympiad {Name = "�����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new Olympiad {Name = "��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new Olympiad {Name = "���������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������"),Coefficient=0.2}
                }},

                new Olympiad {Name = "��������������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2}
                }},
                new Olympiad {Name = "�������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.2}
                }},
                new Olympiad {Name = "����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2}
                }},

                new Olympiad {Name = "���������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� ����"),Coefficient=0.2}
                }},
                new Olympiad {Name = "�������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new Olympiad {Name = "����������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new Olympiad {Name = "��������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
            };
            foreach (var olympiad in olympiads)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
                if (context.Olympiads.SingleOrDefault(p => p.Name == olympiad.Name) == null)
                {
                    context.Olympiads.Add(olympiad);
                }
            }
            context.SaveChanges();
        }

        private static void SchoolDisciplines_Weight(OptimalEducation.DAL.Models.OptimalEducationDbContext context, List<Characteristic> Characterisics)
        {
            //�� ������ ������ ������ �� 11 �����
            var schoolDisciplines = new List<SchoolDiscipline>
            {
                new SchoolDiscipline {Name = "�������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "���������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.7},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.1}
                }},
                new SchoolDiscipline {Name = "�����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "�����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "���������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������"),Coefficient=0.2}
                }},

                new SchoolDiscipline {Name = "�������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "��������������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2},
                }},
                new SchoolDiscipline {Name = "����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2}
                }},
                
                new SchoolDiscipline {Name = "������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "���������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "�������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "����������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new SchoolDiscipline {Name = "��������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
            };
            foreach (var discipline in schoolDisciplines)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
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
                new ExamDiscipline {Name = "������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.1},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.1},
                }},
                new ExamDiscipline {Name = "����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "�����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.3},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.1}
                }},
                new ExamDiscipline {Name = "������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "�����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "��������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "���������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������"),Coefficient=0.2}
                }},

                new ExamDiscipline {Name = "��������������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������������"),Coefficient=0.8},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "�������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������������"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "����������", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������"),Coefficient=0.2}
                }},

                new ExamDiscipline {Name = "���������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� ����"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "�������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="�������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "����������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="����������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
                new ExamDiscipline {Name = "��������� ����", Weights=new List<Weight>()
                {
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="��������� ����"),Coefficient=0.6},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="���������� ����"),Coefficient=0.2},
                    new Weight(){Characterisic=Characterisics.Single(p=>p.Name=="������� ����"),Coefficient=0.2}
                }},
            };
            foreach (var discipline in examDisciplines)
            {
                //���� ���� ����� ������ - ���������. ����� ����������(�� ���� ���������)
                if (context.ExamDisciplines.SingleOrDefault(p => p.Name == discipline.Name) == null)
                {
                    context.ExamDisciplines.Add(discipline);
                }
            }
            context.SaveChanges();
        }
    }
}
