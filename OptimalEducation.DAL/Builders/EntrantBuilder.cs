using OptimalEducation.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.DAL.Builders
{
    //TODO: add async?
    public static class EntrantBuilder
    {
        public static Entrant Create(string name, int id=-1)
        {
            var entrant = new Entrant()
                {
                    FirstName=name
                };
            if (id >= 0) entrant.Id = id;

            using(var context = new OptimalEducationDbContext())
	        {
                //Добавляем ему результаты по ЕГЭ
                foreach (var discipline in context.ExamDisciplines.Where(p=>p.ExamType==ExamType.UnitedStateExam))
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
                foreach (var schoolDisc in context.SchoolDisciplines)
                {
                    entrant.SchoolMarks.Add(
                        new SchoolMark
                        {
                            SchoolDiscipline = schoolDisc,
                            Entrant = entrant,
                            Result = 4,
                        });
                }

                context.Entrants.Add(entrant);
                context.SaveChanges();
	        }

            return entrant;
        }
    }
}
