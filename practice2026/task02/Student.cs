using System.Collections.Generic;

namespace task02
{
    public class Student
    {
        public required string Name { get; set; }
        public required string Faculty { get; set; }
        public required List<int> Grades { get; set; }
    }
}
