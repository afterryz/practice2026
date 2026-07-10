using System;
using System.Collections.Generic;
using System.IO;

namespace task13;

public class Program
{
    public static void Main()
    {
        try
        {
            var service = new JsonService();

            var student = new Student
            {
                FirstName = "Алиса",
                LastName = "Смирнова",
                BirthDate = new DateTime(2007, 7, 7),
                Grades = new List<Subject>
                {
                    new() { Name = "Алгоритмизация и Программирование", Grade = 5 },
                    new() { Name = "Иностранный язык", Grade = 5 },
                    new() { Name = "Математическая логика", Grade = 4 },
                    new() { Name = "Алгебра", Grade = 5 },
                    new() { Name = "Практика речевой деятельности", Grade = 4 }
                },
                Information = "Имеет повышенную стипендию за хорошую успеваемость"
            };

            string jsonStudent = service.StudentSerialize(student);
            Console.WriteLine(jsonStudent);

            string path = "student.json";
            service.SaveFile(student, path);
            Console.WriteLine("Информация сохранена в файл 'student.json'");

            var loaded = service.LoadFile(path);
            Console.WriteLine($"Загружен student: {loaded.FirstName} {loaded.LastName}");
            Console.WriteLine($"Дата рождения: {loaded.BirthDate:yyyy-MM-dd}");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Ошибка {exception.GetType().Name}: {exception.Message}");
        }
    }
}
