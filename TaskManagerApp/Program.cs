using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

class Program
{
    static List<TaskItem> tasks = new();
    static int nextId = 1;
    static string storageType = "Json";

    static void Main()
    {
        LoadConfig();
        LoadTasks();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Менеджер задач ===");
            Console.WriteLine("1. Все задачи");
            Console.WriteLine("2. Добавить");
            Console.WriteLine("3. Завершить");
            Console.WriteLine("4. Удалить");
            Console.WriteLine("5. Выход");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ShowTasks(); break;
                case "2": AddTask(); break;
                case "3": CompleteTask(); break;
                case "4": DeleteTask(); break;
                case "5":
                    SaveTasks();
                    return;
            }

            Console.ReadKey();
        }
    }

    static void LoadConfig()
    {
        if (File.Exists("appsettings.json"))
        {
            var json = File.ReadAllText("appsettings.json");
            var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            storageType = doc["StorageType"];
        }
    }

    static void SaveTasks()
    {
        if (storageType == "Xml")
        {
            var serializer = new XmlSerializer(typeof(List<TaskItem>));
            using var fs = new FileStream("tasks.xml", FileMode.Create);
            serializer.Serialize(fs, tasks);
        }
        else
        {
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("tasks.json", json);
        }
    }

    static void LoadTasks()
    {
        try
        {
            if (storageType == "Xml" && File.Exists("tasks.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<TaskItem>));
                using var fs = new FileStream("tasks.xml", FileMode.Open);
                tasks = (List<TaskItem>)serializer.Deserialize(fs);
            }
            else if (File.Exists("tasks.json"))
            {
                var json = File.ReadAllText("tasks.json");
                tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
            }

            if (tasks.Any())
                nextId = tasks.Max(t => t.Id) + 1;
        }
        catch
        {
            tasks = new List<TaskItem>();
        }
    }

    static void ShowTasks()
    {
        foreach (var t in tasks)
        {
            Console.ForegroundColor = t.IsCompleted ? ConsoleColor.Green : ConsoleColor.Yellow;
            Console.WriteLine($"{t.Id}. {t.Title} | {t.DueDate:d} | {(t.IsCompleted ? "✔" : "✘")}");
        }
        Console.ResetColor();
    }

    static void AddTask()
    {
        Console.Write("Название: ");
        var title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title)) return;

        Console.Write("Описание: ");
        var desc = Console.ReadLine();

        Console.Write("Дата (yyyy-mm-dd): ");
        DateTime.TryParse(Console.ReadLine(), out var date);

        Console.Write("Теги (через запятую): ");
        var tags = Console.ReadLine()?.Split(',').Select(x => x.Trim()).ToList();

        tasks.Add(new TaskItem
        {
            Id = nextId++,
            Title = title,
            Description = desc,
            DueDate = date,
            Tags = tags
        });
    }

    static void CompleteTask()
    {
        Console.Write("ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
                task.IsCompleted = true;
        }
    }

    static void DeleteTask()
    {
        Console.Write("ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
                tasks.Remove(task);
        }
    }
}
