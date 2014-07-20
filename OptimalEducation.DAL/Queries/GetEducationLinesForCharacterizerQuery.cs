﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS;
using OptimalEducation.DAL.Models;

namespace OptimalEducation.DAL.Queries
{
    public class GetEducationLinesForCharacterizerQuery : EFBaseQuery, IQuery<GetEducationLinesForCharacterizerCriterion, Task<List<EducationLine>>>
    {
        public GetEducationLinesForCharacterizerQuery(IOptimalEducationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<List<EducationLine>> Ask(GetEducationLinesForCharacterizerCriterion criterion)
        {
            var edLines = await _dbContext.EducationLines
                .Include(edl => edl.EducationLinesRequirements.Select(edlReq => edlReq.ExamDiscipline.Weights.Select(w => w.Characterisic)))
                .Include(edl => edl.Faculty.HigherEducationInstitution)
                .Where(p => p.Actual == true && p.Name != "IDEAL")
                .AsNoTracking()
                .ToListAsync();
            return edLines;
        }
    }

    public class GetEducationLinesForCharacterizerCriterion : ICriterion
    {
    }
}
