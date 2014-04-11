using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalEducation.Logic.MulticriterialAnalysis.Models
{
    public class EducationLineAndClustersRow
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public Dictionary<string, double> Clusters { get; set; }

        public EducationLineAndClustersRow(int id)
        {
            Id = id;
            Clusters = new Dictionary<string, double>();
        }
    }
}
