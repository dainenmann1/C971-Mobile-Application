using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WGU_C971.Models
{
    [Table("SchoolCourse")]
    public class SchoolCourse
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TermId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Notify { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Note { get; set; }
    }
}
