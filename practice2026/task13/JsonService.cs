using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace task13;

public class JsonService
{
    private readonly JsonSerializerOptions _options;

    public JsonService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
        _options.Converters.Add(new CustomDateConverter());
    }

    public string StudentSerialize(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    public Student StudentDeserialize(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, _options);
        if (student == null)
        {
            throw new ArgumentException("Deserialization result is null.");
        }

        StudentValidate(student);
        return student;
    }

    public void SaveFile(Student student, string filepath)
    {
        File.WriteAllText(filepath, StudentSerialize(student));
    }

    public Student LoadFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException("Specified file not found.");
        }

        return StudentDeserialize(File.ReadAllText(filepath));
    }

    private static void StudentValidate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            throw new ArgumentException("First name must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new ArgumentException("Last name must not be empty.");
        }

        if (student.BirthDate > DateTime.Now)
        {
            throw new ArgumentException("Birth date cannot be in the future.");
        }

        if (student.Grades == null || !student.Grades.Any())
        {
            throw new ArgumentException("Grades list must contain at least one subject.");
        }

        foreach (var subject in student.Grades)
        {
            if (string.IsNullOrWhiteSpace(subject.Name))
            {
                throw new ArgumentException("Subject name must not be empty.");
            }

            if (subject.Grade < 2 || subject.Grade > 5)
            {
                throw new ArgumentException("Grade must be between 2 and 5.");
            }
        }
    }
}
