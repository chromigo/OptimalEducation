namespace OptimalEducation.DAL.Models
{
    public class Weight
    {
        public int Id { get; set; }
        public double Coefficient { get; set; }
        public int CharacterisicId { get; set; }
        public int? SchoolId { get; set; }
        public int? SchoolDisciplineId { get; set; }
        public int? SectionId { get; set; }
        public int? HobbieId { get; set; }
        public int? OlympiadId { get; set; }
        public int? ExamDisciplineId { get; set; }
        public virtual Characteristic Characterisic { get; set; }
        public virtual School School { get; set; }
        public virtual SchoolDiscipline SchoolDiscipline { get; set; }
        public virtual Section Section { get; set; }
        public virtual Hobbie Hobbie { get; set; }
        public virtual Olympiad Olympiad { get; set; }
        public virtual ExamDiscipline ExamDiscipline { get; set; }
    }
}