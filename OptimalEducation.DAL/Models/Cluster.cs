namespace OptimalEducation.DAL.Models
{

    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Кластер для весов (данный вес вносит свой вклад в данный кластер)
    /// </summary>
    public partial class Cluster
    {

        public Cluster()
        {

            this.Weights = new HashSet<Weight>();

        }


        public int Id { get; set; }

        public string Name { get; set; }


        public virtual ICollection<Weight> Weights { get; set; }

    }

}
