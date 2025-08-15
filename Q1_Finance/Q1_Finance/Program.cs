using System;
using System.Collections.Generic;

namespace Q1_Finance;

// a) record
public readonly record struct Transaction(int Id, DateTime Date, decimal Amount, string Category);

// b) interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// c) processors
public sealed class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[BankTransfer] Processing {t.Amount:C} for '{t.Category}' on {t.Date:d}");
}

public sealed class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[MobileMoney] Processing {t.Amount:C} for '{t.Category}' on {t.Date:d}");
}

public sealed class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[Crypto] Processing {t.Amount:C} for '{t.Category}' on {t.Date:d}");
}

// d) base Account
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"[Account] Applied {transaction.Amount:C}. New balance: {Balance:C}");
    }
}

// e) sealed SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }
        Balance -= transaction.Amount;
        Console.WriteLine($"[SavingsAccount] Deducted {transaction.Amount:C}. Updated balance: {Balance:C}");
    }
}

// f) FinanceApp
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        var acct = new SavingsAccount("SA-001", 1000m);

        var t1 = new Transaction(1, DateTime.Today, 120m, "Groceries");
        var t2 = new Transaction(2, DateTime.Today, 300m, "Utilities");
        var t3 = new Transaction(3, DateTime.Today, 700m, "Entertainment");

        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        p1.Process(t1);
        p2.Process(t2);
        p3.Process(t3);

        acct.ApplyTransaction(t1);
        acct.ApplyTransaction(t2);
        acct.ApplyTransaction(t3);

        _transactions.AddRange(new[] { t1, t2, t3 });
        Console.WriteLine($"\nTotal transactions recorded: {_transactions.Count}");
    }
}

public class Program
{
    public static void Main()
    {
        new FinanceApp().Run();
    }
}
