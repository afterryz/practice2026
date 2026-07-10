using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;
using task13;

namespace task13tests;

public class StudentJsonTests
{
    private readonly JsonService _service = new();

    [Fact]
    public void Serialize_ExcludesNullFieldsCorrectly()
    {
        var student = new Student
        {
            FirstName = "Александр",
            LastName = "Сидоров",
            BirthDate = new DateTime(2006, 6, 24),
            Grades = new List<Subject>
            {
                new() { Name = "Математика", Grade = 5 }
            },
            Information = null
        };

        string json = _service.StudentSerialize(student);

        Assert.Contains("Александр", json);
        Assert.Contains("\"BirthDate\": \"2006-06-24\"", json);
        Assert.DoesNotContain("Information", json);
    }

    [Fact]
    public void Serialize_IncludesOptionalFieldsWhenNotNull()
    {
        var student = new Student
        {
            FirstName = "Варвара",
            LastName = "Иванова",
            BirthDate = new DateTime(2008, 2, 12),
            Grades = new List<Subject>
            {
                new() { Name = "Информатика", Grade = 5 }
            },
            Information = "Отличница"
        };

        string json = _service.StudentSerialize(student);

        Assert.Contains("Отличница", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsValidStudentObject()
    {
        string json = @"{
            ""FirstName"": ""Илья"",
            ""LastName"": ""Петров"",
            ""BirthDate"": ""2007-05-22"",
            ""Grades"": [
                { ""Name"": ""Физика"", ""Grade"": 4 }
            ]
        }";

        var student = _service.StudentDeserialize(json);

        Assert.Equal("Илья", student.FirstName);
        Assert.Equal("Петров", student.LastName);
        Assert.Equal(new DateTime(2007, 5, 22), student.BirthDate);
    }

    [Fact]
    public void Deserialize_InvalidDateFormat_ThrowsJsonException()
    {
        string json = @"{
            ""FirstName"": ""Екатерина"",
            ""LastName"": ""Владимирова"",
            ""BirthDate"": ""25-04-2010"",
            ""Grades"": [
                { ""Name"": ""История"", ""Grade"": 5 }
            ]
        }";

        Assert.Throws<JsonException>(() => _service.StudentDeserialize(json));
    }

    [Fact]
    public void Deserialize_EmptyName_ThrowsArgumentException()
    {
        string json = @"{
            ""FirstName"": """",
            ""LastName"": ""Сергеева"",
            ""BirthDate"": ""2005-01-01"",
            ""Grades"": [
                { ""Name"": ""Химия"", ""Grade"": 4 }
            ]
        }";

        Assert.Throws<ArgumentException>(() => _service.StudentDeserialize(json));
    }

    [Fact]
    public void Program_MainExecution_OutputsValidConsoleData()
    {
        using var output = new StringWriter();
        Console.SetOut(output);

        Program.Main();

        string result = output.ToString();
        Assert.Contains("Алиса Смирнова", result);
        Assert.Contains("2007-07-07", result);
        Assert.Contains("Информация сохранена в файл", result);
    }
}
