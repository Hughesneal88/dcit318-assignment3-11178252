using System;
using System.Collections.Generic;

namespace Q3_Warehouse;

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }

    public override string ToString() => $"Electronics #{Id}: {Name} ({Brand}), Qty={Quantity}, Warranty={WarrantyMonths}m";
}


public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
    }

    public override string ToString() => $"Grocery #{Id}: {Name}, Qty={Quantity}, Expires={ExpiryDate:d}";
}


public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }


public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id); // may throw ItemNotFoundException
        item.Quantity = newQuantity;
    }
}


public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "Router", 15, "TP-Link", 18));

        _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 50, DateTime.Today.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(102, "Milk 1L", 80, DateTime.Today.AddDays(20)));
        _groceries.AddItem(new GroceryItem(103, "Bread", 30, DateTime.Today.AddDays(3)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var current = repo.GetItemById(id);
            repo.UpdateQuantity(id, current.Quantity + quantity);
            Console.WriteLine($"[OK] Increased stock for #{id} by {quantity}. New Qty={current.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] IncreaseStock failed: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"[OK] Removed item #{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] RemoveItem failed: {ex.Message}");
        }
    }

    public static void Main()
    {
        var mgr = new WareHouseManager();
        mgr.SeedData();

        Console.WriteLine("== Grocery Items ==");
        mgr.PrintAllItems(mgr._groceries);

        Console.WriteLine("\n== Electronic Items ==");
        mgr.PrintAllItems(mgr._electronics);

        Console.WriteLine("\n== Exception Scenarios ==");
        // Add duplicate
        try
        {
            mgr._groceries.AddItem(new GroceryItem(101, "Duplicate Rice", 10, DateTime.Today.AddMonths(6)));
        }
        catch (Exception ex) { Console.WriteLine($"[Caught] {ex.Message}"); }

        // Remove non-existent
        mgr.RemoveItemById(mgr._electronics, 999);

        // Update invalid quantity
        try
        {
            mgr._electronics.UpdateQuantity(1, -5);
        }
        catch (Exception ex) { Console.WriteLine($"[Caught] {ex.Message}"); }
    }
}
