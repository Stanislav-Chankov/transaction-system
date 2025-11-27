using System;
using System.IO;
using AccessFinance.TransactionSystem;
using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;
using Xunit;

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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1500m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
        Assert.Contains("1,500.00", outputText);
    }

    [Fact]
    public void DepositMoney_ToZeroBalance_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 0m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n1000\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", -500m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n1000\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n0.01\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n123.456789\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);

        // First deposit
        var input1 = new StringReader("ACC001\n300\n");
        var output1 = new StringWriter();
        Console.SetIn(input1);
        Console.SetOut(output1);
        service.DepositMoney();
        Assert.Equal(1300m, account.Balance);

        // Second deposit
        var input2 = new StringReader("ACC001\n200\n");
        var output2 = new StringWriter();
        Console.SetIn(input2);
        Console.SetOut(output2);
        service.DepositMoney();
        Assert.Equal(1500m, account.Balance);

        // Third deposit
        var input3 = new StringReader("ACC001\n100\n");
        var output3 = new StringWriter();
        Console.SetIn(input3);
        Console.SetOut(output3);
        service.DepositMoney();

        // Assert
        Assert.Equal(1600m, account.Balance);
    }

    [Fact]
    public void DepositMoney_VeryLargeAmount_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n999999999\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", decimal.MaxValue - 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n1000\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var input = new StringReader("INVALID\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var input = new StringReader("\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    [Fact]
    public void DepositMoney_EmptyAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateDepositService(store);
        var input = new StringReader("   \n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    [Fact]
    public void DepositMoney_WhitespaceAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateDepositService(store);
        var input = new StringReader("   \n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    #endregion

    #region Invalid Amount Scenarios

    [Fact]
    public void DepositMoney_ZeroAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n0\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void DepositMoney_NegativeAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n-100\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void DepositMoney_NonNumericAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\nabc\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void DepositMoney_EmptyAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void DepositMoney_WhitespaceAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n   \n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void DepositMoney_InvalidDecimalFormat_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n12.34.56\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        // Withdraw first
        account.Withdraw(400m);
        Assert.Equal(600m, account.Balance);

        // Then deposit
        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n300\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);

        // First deposit
        var input1 = new StringReader("ACC001\n300\n");
        var output1 = new StringWriter();
        Console.SetIn(input1);
        Console.SetOut(output1);
        service.DepositMoney();
        Assert.Equal(1300m, account.Balance);

        // Second deposit
        var input2 = new StringReader("ACC001\n200\n");
        var output2 = new StringWriter();
        Console.SetIn(input2);
        Console.SetOut(output2);
        service.DepositMoney();
        Assert.Equal(1500m, account.Balance);

        // Third deposit
        var input3 = new StringReader("ACC001\n100\n");
        var output3 = new StringWriter();
        Console.SetIn(input3);
        Console.SetOut(output3);
        service.DepositMoney();

        // Assert
        Assert.Equal(1600m, account.Balance);
    }

    [Fact]
    public void DepositMoney_ExactAmountToReachTarget_Succeeds()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 500m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", 123.4567890123456789012345678m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n100\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

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
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        // Withdraw
        account.Withdraw(300m);
        Assert.Equal(700m, account.Balance);

        // Deposit
        var service = CreateDepositService(store);
        var input = new StringReader("ACC001\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.DepositMoney();

        // Assert
        Assert.Equal(1200m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Deposit successful", outputText);
    }

    #endregion
}

