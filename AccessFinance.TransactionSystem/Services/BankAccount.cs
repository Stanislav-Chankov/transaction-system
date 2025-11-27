namespace AccessFinance.TransactionSystem.Services;

public class BankAccount
{
    private readonly object _lock = new();
    public string Name { get; }
    public string AccountNumber { get; }
    private decimal _balance;

    public BankAccount(string name, decimal initialBalance, string accountNumber)
    {
        Name = name;
        AccountNumber = accountNumber;
        _balance = initialBalance;
    }

    public decimal Balance
    {
        get
        {
            lock (_lock)
            {
                return _balance;
            }
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");

        lock (_lock)
        {
            _balance += amount;
        }
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be positive.");

        lock (_lock)
        {
            if (_balance < amount)
                return false;

            _balance -= amount;
            return true;
        }
    }
}
