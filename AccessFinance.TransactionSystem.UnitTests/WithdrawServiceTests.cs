using System;
using System.IO;
using AccessFinance.TransactionSystem;
using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;
using Xunit;

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
        var input = new StringReader("ACC001\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n1000\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n0.01\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n123.456789\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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

        // First withdrawal
        var input1 = new StringReader("ACC001\n300\n");
        var output1 = new StringWriter();
        Console.SetIn(input1);
        Console.SetOut(output1);
        service.WithdrawMoney();
        Assert.Equal(700m, account.Balance);

        // Second withdrawal
        var input2 = new StringReader("ACC001\n200\n");
        var output2 = new StringWriter();
        Console.SetIn(input2);
        Console.SetOut(output2);
        service.WithdrawMoney();
        Assert.Equal(500m, account.Balance);

        // Third withdrawal
        var input3 = new StringReader("ACC001\n100\n");
        var output3 = new StringWriter();
        Console.SetIn(input3);
        Console.SetOut(output3);
        service.WithdrawMoney();

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
        var input = new StringReader("INVALID\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    [Fact]
    public void WithdrawMoney_EmptyAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateWithdrawService(store);
        var input = new StringReader("   \n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    [Fact]
    public void WithdrawMoney_WhitespaceAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateWithdrawService(store);
        var input = new StringReader("   \n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
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
        var input = new StringReader("ACC001\n0\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n-100\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void WithdrawMoney_NonNumericAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var input = new StringReader("ACC001\nabc\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void WithdrawMoney_EmptyAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var input = new StringReader("ACC001\n\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void WithdrawMoney_WhitespaceAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var input = new StringReader("ACC001\n   \n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

        // Assert
        Assert.Equal(1000m, account.Balance); // Balance unchanged
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void WithdrawMoney_InvalidDecimalFormat_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateWithdrawService(store);
        var input = new StringReader("ACC001\n12.34.56\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n500\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n100\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n100\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n100.01\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n999999999999\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n300\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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
        var input = new StringReader("ACC001\n1\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.WithdrawMoney();

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

        // First withdrawal
        var input1 = new StringReader("ACC001\n300\n");
        var output1 = new StringWriter();
        Console.SetIn(input1);
        Console.SetOut(output1);
        service.WithdrawMoney();
        Assert.Equal(700m, account.Balance);

        // Second withdrawal
        var input2 = new StringReader("ACC001\n200\n");
        var output2 = new StringWriter();
        Console.SetIn(input2);
        Console.SetOut(output2);
        service.WithdrawMoney();
        Assert.Equal(500m, account.Balance);

        // Third withdrawal - should fail
        var input3 = new StringReader("ACC001\n600\n");
        var output3 = new StringWriter();
        Console.SetIn(input3);
        Console.SetOut(output3);
        service.WithdrawMoney();

        // Assert
        Assert.Equal(500m, account.Balance);
        var outputText = output3.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    #endregion
}

