using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.DAL.Builders
{
    public static class IdealEntrantBuilder
    {
        static bool idealEntrantWasCreated;
        public static Entrant Create()
        {
            if (idealEntrantWasCreated == false)
            {
                var entrant = new Entrant()
                {
                    Id = 2,//(связано через claim с аккаунтом идеального пользователя)
                    FirstName = "IDEAL",
                    LastName = "Ерохин"
                };

                using (var context = new OptimalEducationDbContext())
                {
                    //Добавляем ему результаты по ЕГЭ
                    foreach (var discipline in context.ExamDisciplines.Where(p => p.ExamType == ExamType.UnitedStateExam))
                    {
                        entrant.UnitedStateExams.Add(
                            new UnitedStateExam
                            {
                                Discipline = discipline,
                                Entrant = entrant,
                                Result = 100,
                            });
                    }
                    //Добавляем ему результаты по школьным предметам
                    foreach (var schoolDisc in context.SchoolDisciplines)
                    {
                        entrant.SchoolMarks.Add(
                            new SchoolMark
                            {
                                SchoolDiscipline = schoolDisc,
                                Entrant = entrant,
                                Result = 5,
                            });
                    }
                    //Добавляем ему результаты по олимпиадам(по всем???)
                    for (int i = 0; i < 3; i++)
                    {
                        foreach (var olympiad in context.Olympiads)
                        {
                            entrant.ParticipationInOlympiads.Add(
                                new ParticipationInOlympiad()
                                {
                                    Entrant = entrant,
                                    Result = OlypmpiadResult.FirstPlace,
                                    Olympiad = olympiad
                                });
                        }
                    }

                    context.Entrants.Add(entrant);
                    context.SaveChanges();
                }

                idealEntrantWasCreated = true;
                return entrant;
            }
            else throw new Exception("Идеальный пользователь уже был создан в Seed методе. Повторный вызов запрещен.");
        }
    }
}
