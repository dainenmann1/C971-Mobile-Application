using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WGU_C971.Models
{
    [Table("Assessment")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Notify { get; set; }
        public string Type { get; set; }
    }
}
