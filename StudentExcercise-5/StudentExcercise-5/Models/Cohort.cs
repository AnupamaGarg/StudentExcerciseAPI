

using System;
using System.Collections.Generic;
using System.Text;
//using System.Linq;
//using System.Threading.Tasks;


namespace StudentExcercise_5.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public string CohortName { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Instructor> Instructors { get; set; } = new List<Instructor>();
    }
}
