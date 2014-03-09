namespace OptimalEducation.Models
{
	using System;
	using System.Collections.Generic;
	
public partial class Entrant
{

	public Entrant()
	{

        this.ParticipationInSections = new HashSet<ParticipationInSection>();

		this.Hobbies = new HashSet<Hobbie>();

		this.Schools = new HashSet<School>();

		this.Preferences = new HashSet<Preference>();

		this.ParticipationInOlympiads = new HashSet<ParticipationInOlympiad>();

		this.UnitedStateExams = new HashSet<UnitedStateExam>();

		this.SchoolMarks = new HashSet<SchoolMark>();

	}

	public int Id { get; set; }

	public string FirstName { get; set; }

	public string LastName { get; set; }

	public string Gender { get; set; }

	public string SchoolEducation { get; set; }

	public string Medal { get; set; }

	public string Citizenship { get; set; }

	public Nullable<double> AverageMark { get; set; }



	public virtual ICollection<ParticipationInSection> ParticipationInSections { get; set; }

	public virtual ICollection<Hobbie> Hobbies { get; set; }

	public virtual ICollection<School> Schools { get; set; }

	public virtual ICollection<Preference> Preferences { get; set; }

	public virtual ICollection<ParticipationInOlympiad> ParticipationInOlympiads { get; set; }

	public virtual ICollection<UnitedStateExam> UnitedStateExams { get; set; }

	public virtual ICollection<SchoolMark> SchoolMarks { get; set; }

}

}
