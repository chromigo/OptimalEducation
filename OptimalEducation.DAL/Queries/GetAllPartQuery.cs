﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimalEducation.DAL.Models;
using CQRS;

namespace OptimalEducation.DAL.Queries
{
    public class GetAllPartQuery : IQuery<TestCriteria, IEnumerable<ParticipationInOlympiad>>
    {
        private readonly IOptimalEducationDbContext _dbContext;

        public GetAllPartQuery(IOptimalEducationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //add async
        public IEnumerable<ParticipationInOlympiad> Ask(TestCriteria criterion)
        {
            var participationinolympiads = _dbContext.ParticipationInOlympiads
                .Include(p => p.Entrant)
                .Include(p => p.Olympiad)
                .Where(p => p.EntrantId == criterion.Id)
                .AsNoTracking()
                .ToList();
                //.ToListAsync();
            return participationinolympiads;
        }
    }

    public class TestCriteria : ICriterion
    {
        public int Id { get; set; }
    }
}
