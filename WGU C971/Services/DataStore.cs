using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using WGU_C971.Models;

namespace WGU_C971.Services
{
    internal class DataStore
    {
        public static void GenerateData(int termId)
        {
            SchoolTerm term = new SchoolTerm()
            {
                Name = $"Term {termId}",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3)
            };
            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.Insert(term);
            }

            SchoolCourse course = new SchoolCourse()
            {
                Name = $"C971 - Mobile Application Development",
                TermId = term.Id,
                Status = "Anticipate To Take",
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(6),
                InstructorName = "Dainen Mann",
                InstructorEmail = "dmann21@wgu.edu",
                InstructorPhone = "801-989-2159",
                Notify = true,
                Note = $"Welcome to WGU. This is C971."
            };

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.Insert(course);
            }

            Assessment performanceAssessment = new Assessment()
            {
                Name = "PEF1",
                CourseId = course.Id,
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(3).AddHours(2.5),
                Notify = true,
                Type = "Performance"
            };

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.Insert(performanceAssessment);
            }

            Assessment objectiveAssessment = new Assessment()
            {
                Name = "OBJ1",
                CourseId = course.Id,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(2).AddHours(2.5),
                Notify = true,
                Type = "Objective"
            };

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                connection.Insert(objectiveAssessment);
            }
        }
    }
}
