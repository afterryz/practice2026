using System;
using System.Collections.Generic;
using System.Linq;

namespace task02
{
    public class StudentService
    {
        private readonly List<Student> _students;

        public StudentService(List<Student> students) => _students = students;

        public IEnumerable<Student> GetStudentsByFaculty(string faculty)
        {
            return _students.Where(s => s.Faculty == faculty);
        }

        public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
        {
            return _students.Where(s => s.Grades.Average() >= minAverageGrade);
        }

        public IEnumerable<Student> GetStudentsOrderedByName()
        {
            return _students.OrderBy(s => s.Name);
        }

        public ILookup<string, Student> GroupStudentsByFaculty()
        {
            return _students.ToLookup(s => s.Faculty);
        }

        public string GetFacultyWithHighestAverageGrade()
        {
            return _students
                .GroupBy(s => s.Faculty)
                .OrderByDescending(group => group.SelectMany(s => s.Grades).Average())
                .First()
                .Key;
        }
    }
}
