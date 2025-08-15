using System;
using System.Collections.Generic;
using System.IO;

namespace Q4_Grading;

// a) Student
public class Student
{
    public int Id;
    public string FullName = "";
    public int Score;

    public string GetGrade() =>
        Score >= 80 ? "A" :
        Score >= 70 ? "B" :
        Score >= 60 ? "C" :
        Score >= 50 ? "D" : "F";

    public override string ToString() =>
        $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
}

// b,c) custom exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}
public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// d) processor
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using var sr = new StreamReader(inputFilePath);
        string? line;
        int lineNo = 0;

        while ((line = sr.ReadLine()) != null)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 3)
                throw new MissingFieldException($"Line {lineNo}: Expected 3 fields (Id, FullName, Score). Got {parts.Length}.");

            if (!int.TryParse(parts[0], out var id))
                throw new InvalidScoreFormatException($"Line {lineNo}: Invalid ID '{parts[0]}'.");

            if (!int.TryParse(parts[2], out var score))
                throw new InvalidScoreFormatException($"Line {lineNo}: Score '{parts[2]}' is not an integer.");

            var student = new Student { Id = id, FullName = parts[1], Score = score };
            students.Add(student);
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var sw = new StreamWriter(outputFilePath);
        foreach (var s in students)
        {
            sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }
}

public class Program
{
    // Usage:
    // dotnet run                -> creates demo 'students_input.txt' and processes it
    // dotnet run -- input.txt   -> processes your provided file path
    public static void Main(string[] args)
    {
        var processor = new StudentResultProcessor();

        // default demo files
        string input = "students_input.txt";
        string output = "students_report.txt";

        // if user provides a path, use it (and keep output file name)
        if (args.Length > 0) input = args[0];

        try
        {
            if (args.Length == 0 && !File.Exists(input))
            {
                // create a demo file so the program runs out-of-the-box
                File.WriteAllLines(input, new[]
                {
                    "101,Alice Smith,84",
                    "102,John Mensah,73",
                    "103,Ama Owusu,58",
                    "104,Kofi Asare,47"
                });
                Console.WriteLine($"[Demo] Created sample input at '{input}'.");
            }

            var students = processor.ReadStudentsFromFile(input);
            processor.WriteReportToFile(students, output);
            Console.WriteLine($"Report written to '{output}'.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Input file '{input}' not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Invalid score format: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing field: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
