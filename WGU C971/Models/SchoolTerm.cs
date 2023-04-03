using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WGU_C971.Models
{
    [Table("SchoolTerm")]
    public class SchoolTerm
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
