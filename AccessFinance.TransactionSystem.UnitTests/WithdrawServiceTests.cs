using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.UnitTests;

public class WithdrawServiceTests
{
    private IBankAccountStore CreateTestAccountStore()
    {
        return new BankAccountStore();
    }

    private WithdrawService CreateWithdrawService(IBankAccountStore? store = null)
    {
        return new WithdrawService(store ?? CreateTestAccountStore());
    }

    #region Successful Withdrawal Scenarios

    [Fact]
    public void WithdrawMoney_ValidAccountAndAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 500m);

        // Assert
        Assert.Equal(500m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Withdrawal successful", outputText);
        Assert.Contains("500.00", outputText);
    }

    [Fact]
    public void WithdrawMoney_ExactBalanceAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 1000m);

        // Assert
        Assert.Equal(0m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Withdrawal successful", outputText);
    }

    [Fact]
    public void WithdrawMoney_SmallAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 0.01m);

        // Assert
        Assert.Equal(999.99m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Withdrawal successful", outputText);
    }

    [Fact]
    public void WithdrawMoney_HighPrecisionAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 123.456789m);

        // Assert
        Assert.Equal(876.543211m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Withdrawal successful", outputText);
    }

    [Fact]
    public void WithdrawMoney_MultipleWithdrawals_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // First withdrawal
        service.WithdrawMoney("ACC001", 300m);
        Assert.Equal(700m, account.Balance);

        // Second withdrawal
        service.WithdrawMoney("ACC001", 200m);
        Assert.Equal(500m, account.Balance);

        // Third withdrawal
        service.WithdrawMoney("ACC001", 100m);

        // Assert
        Assert.Equal(400m, account.Balance);
    }

    #endregion

    #region Invalid Account Scenarios

    [Fact]
    public void WithdrawMoney_AccountNotFound_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("INVALID", 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    [Fact]
    public void WithdrawMoney_NullAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney(null!, 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account number cannot be empty", outputText);
    }

    [Fact]
    public void WithdrawMoney_EmptyAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney(string.Empty, 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account number cannot be empty", outputText);
    }

    [Fact]
    public void WithdrawMoney_WhitespaceAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("   ", 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account number cannot be empty", outputText);
    }

    #endregion

    #region Invalid Amount Scenarios

    [Fact]
    public void WithdrawMoney_ZeroAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 0m);

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void WithdrawMoney_NegativeAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", -100m);

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    #endregion

    #region Insufficient Balance Scenarios

    [Fact]
    public void WithdrawMoney_InsufficientBalance_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 100m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 500m);

        // Assert
        Assert.Equal(100m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    [Fact]
    public void WithdrawMoney_ZeroBalance_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 0m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 100m);

        // Assert
        Assert.Equal(0m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    [Fact]
    public void WithdrawMoney_NegativeBalance_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", -50m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 100m);

        // Assert
        Assert.Equal(-50m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    [Fact]
    public void WithdrawMoney_AmountGreaterThanBalance_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 100m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 100.01m);

        // Assert
        Assert.Equal(100m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void WithdrawMoney_VeryLargeAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 999999999999m);

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    [Fact]
    public void WithdrawMoney_AfterDeposit_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 100m, "ACC001");
        store.TryAdd("ACC001", account);

        // Deposit first
        account.Deposit(400m);
        Assert.Equal(500m, account.Balance);

        // Then withdraw
        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 300m);

        // Assert
        Assert.Equal(200m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Withdrawal successful", outputText);
    }

    [Fact]
    public void WithdrawMoney_MaximumDecimalValue_HandlesCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", decimal.MaxValue, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.WithdrawMoney("ACC001", 1m);

        // Assert
        Assert.Equal(decimal.MaxValue - 1m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Withdrawal successful", outputText);
    }

    [Fact]
    public void WithdrawMoney_ConcurrentWithdrawals_MaintainsConsistency()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // First withdrawal
        service.WithdrawMoney("ACC001", 300m);
        Assert.Equal(700m, account.Balance);

        // Second withdrawal
        service.WithdrawMoney("ACC001", 200m);
        Assert.Equal(500m, account.Balance);

        // Third withdrawal - should fail
        service.WithdrawMoney("ACC001", 600m);

        // Assert
        Assert.Equal(500m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    #endregion
}
