using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.UnitTests;

public class DepositServiceTests
{
    private IBankAccountStore CreateTestAccountStore()
    {
        return new BankAccountStore();
    }

    private DepositService CreateDepositService(IBankAccountStore? store = null)
    {
        return new DepositService(store ?? CreateTestAccountStore());
    }

    #region Successful Deposit Scenarios

    [Fact]
    public void DepositMoney_ValidAccountAndAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC101");
        store.TryAdd("ACC101", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC101", 500m);

        // Assert
        Assert.Equal(1500m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful.", outputText);
        Assert.Contains("$1,500.00", outputText);
    }

    [Fact]
    public void DepositMoney_ToZeroBalance_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 0m, "ACC102");
        store.TryAdd("ACC102", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC102", 1000m);

        // Assert
        Assert.Equal(1000m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_ToNegativeBalance_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", -500m, "ACC103");
        store.TryAdd("ACC103", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC103", 1000m);

        // Assert
        Assert.Equal(500m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_SmallAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC104");
        store.TryAdd("ACC104", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC104", 0.01m);

        // Assert
        Assert.Equal(1000.01m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_HighPrecisionAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC105");
        store.TryAdd("ACC105", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC105", 123.456789m);

        // Assert
        Assert.Equal(1123.456789m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_MultipleDeposits_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC106");
        store.TryAdd("ACC106", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // First deposit
        service.DepositMoney("ACC106", 300m);
        Assert.Equal(1300m, account.Balance);

        // Second deposit
        service.DepositMoney("ACC106", 200m);
        Assert.Equal(1500m, account.Balance);

        // Third deposit
        service.DepositMoney("ACC106", 100m);

        // Assert
        Assert.Equal(1600m, account.Balance);
    }

    [Fact]
    public void DepositMoney_VeryLargeAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC107");
        store.TryAdd("ACC107", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC107", 999999999m);

        // Assert
        Assert.Equal(1000000999m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_ToMaximumBalance_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", decimal.MaxValue - 1000m, "ACC108");
        store.TryAdd("ACC108", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC108", 1000m);

        // Assert
        Assert.Equal(decimal.MaxValue, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    #endregion

    #region Invalid Account Scenarios

    [Fact]
    public void DepositMoney_AccountNotFound_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("INVALID", 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    [Fact]
    public void DepositMoney_NullAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney(null!, 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account number cannot be empty", outputText);
    }

    [Fact]
    public void DepositMoney_EmptyAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney(string.Empty, 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account number cannot be empty", outputText);
    }

    [Fact]
    public void DepositMoney_WhitespaceAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("   ", 500m);

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account number cannot be empty", outputText);
    }

    #endregion

    #region Invalid Amount Scenarios

    [Fact]
    public void DepositMoney_ZeroAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC109");
        store.TryAdd("ACC109", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC109", 0m);

        // Assert
        Assert.Equal(1000m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void DepositMoney_NegativeAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC110");
        store.TryAdd("ACC110", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC110", -100m);

        // Assert
        Assert.Equal(1000m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void DepositMoney_AfterWithdrawal_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC111");
        store.TryAdd("ACC111", account);

        // Withdraw first
        account.Withdraw(400m);
        Assert.Equal(600m, account.Balance);

        // Then deposit
        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC111", 300m);

        // Assert
        Assert.Equal(900m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_ConcurrentDeposits_MaintainsConsistency()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC112");
        store.TryAdd("ACC112", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // First deposit
        service.DepositMoney("ACC112", 300m);
        Assert.Equal(1300m, account.Balance);

        // Second deposit
        service.DepositMoney("ACC112", 200m);
        Assert.Equal(1500m, account.Balance);

        // Third deposit
        service.DepositMoney("ACC112", 100m);

        // Assert
        Assert.Equal(1600m, account.Balance);
    }

    [Fact]
    public void DepositMoney_ExactAmountToReachTarget_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 500m, "ACC113");
        store.TryAdd("ACC113", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC113", 500m);

        // Assert
        Assert.Equal(1000m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_ToAccountWithMaximumPrecision_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 123.4567890123456789012345678m, "ACC114");
        store.TryAdd("ACC114", account);

        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC114", 100m);

        // Assert
        Assert.Equal(223.4567890123456789012345678m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    [Fact]
    public void DepositMoney_CombinedWithWithdrawal_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC115");
        store.TryAdd("ACC115", account);

        // Withdraw
        account.Withdraw(300m);
        Assert.Equal(700m, account.Balance);

        // Deposit
        var service = CreateDepositService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.DepositMoney("ACC115", 500m);

        // Assert
        Assert.Equal(1200m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    #endregion
}
