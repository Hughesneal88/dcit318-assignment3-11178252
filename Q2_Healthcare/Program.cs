using System;
using System.Collections.Generic;
using System.Linq;

namespace Q2_Healthcare;

// a) generic repository
public class Repository<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public List<T> GetAll() => new(_items);
    public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var idx = _items.FindIndex(x => predicate(x));
        if (idx >= 0) { _items.RemoveAt(idx); return true; }
        return false;
    }
}

// b) Patient
public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }

    public override string ToString() => $"Patient #{Id}: {Name}, {Age}, {Gender}";
}

// c) Prescription
public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
    }

    public override string ToString() =>
        $"Rx #{Id} for Patient {PatientId}: {MedicationName} on {DateIssued:d}";
}

public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    // d,e) build dictionary by patient id
    public List<Prescription> GetPrescriptionsByPatientId(int patientId) =>
        _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();

    // g) methods
    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "F"));
        _patientRepo.Add(new Patient(2, "John Mensah", 45, "M"));
        _patientRepo.Add(new Patient(3, "Ama Owusu", 22, "F"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin 500mg", DateTime.Today.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen 200mg", DateTime.Today.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Metformin 500mg", DateTime.Today.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(104, 3, "Cetirizine 10mg", DateTime.Today.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(105, 2, "Lisinopril 10mg", DateTime.Today));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo
            .GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("== Patients ==");
        foreach (var p in _patientRepo.GetAll())
            Console.WriteLine(p);
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        Console.WriteLine($"\n== Prescriptions for Patient {id} ==");
        var list = GetPrescriptionsByPatientId(id);
        if (list.Count == 0) Console.WriteLine("No prescriptions.");
        foreach (var rx in list) Console.WriteLine(rx);
    }

    public static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(2); // select a patient to display
    }
}
