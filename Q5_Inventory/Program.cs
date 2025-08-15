using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Q5_Inventory;


public interface IInventoryEntity { int Id { get; } }


public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;


public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) => _filePath = filePath;

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new(_log);

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using var sw = new StreamWriter(_filePath);
            sw.Write(json);
            Console.WriteLine($"Saved {_log.Count} items -> {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Save Error] {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"[Load] File '{_filePath}' not found. Starting with empty log.");
                return;
            }
            using var sr = new StreamReader(_filePath);
            var json = sr.ReadToEnd();
            var data = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            _log.Clear();
            _log.AddRange(data);
            Console.WriteLine($"Loaded {_log.Count} items <- {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Load Error] {ex.Message}");
        }
    }
}


public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath) => _logger = new InventoryLogger<InventoryItem>(filePath);

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "USB-C Cable", 40, DateTime.Now));
        _logger.Add(new InventoryItem(2, "HDMI Adapter", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Notebook A5", 100, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Pen Blue", 250, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Stapler", 15, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine($"{item.Id}: {item.Name} | Qty={item.Quantity} | Added={item.DateAdded:g}");
    }

    public static void Main()
    {
        const string path = "inventory.json";

        
        var app = new InventoryApp(path);
        app.SeedSampleData();
        app.SaveData();

        
        var newSession = new InventoryApp(path);
        newSession.LoadData();
        newSession.PrintAllItems();
    }
}
