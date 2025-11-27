using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.UnitTests;

public class BalanceServiceTests
{
    private IBankAccountStore CreateTestAccountStore()
    {
        return new BankAccountStore();
    }

    private BalanceService CreateBalanceService(IBankAccountStore? store = null)
    {
        return new BalanceService(store ?? CreateTestAccountStore());
    }

    #region Successful Balance Check Scenarios

    [Fact]
    public void CheckBalance_ValidAccountWithPositiveBalance_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC001\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("1,000.00", outputText);
    }

    [Fact]
    public void CheckBalance_ValidAccountWithZeroBalance_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Jane Smith", 0m, "ACC002");
        store.TryAdd("ACC002", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC002\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: Jane Smith", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("$0.00", outputText);
    }

    [Fact]
    public void CheckBalance_ValidAccountWithSmallBalance_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Alice Brown", 0.01m, "ACC004");
        store.TryAdd("ACC004", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC004\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: Alice Brown", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("0.01", outputText);
    }

    [Fact]
    public void CheckBalance_ValidAccountWithLargeBalance_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Millionaire", 999999999.99m, "ACC005");
        store.TryAdd("ACC005", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC005\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: Millionaire", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_ValidAccountWithHighPrecisionBalance_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Precise Account", 123.456789m, "ACC006");
        store.TryAdd("ACC006", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC006\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: Precise Account", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_ValidAccountWithMaximumDecimalValue_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Max Account", decimal.MaxValue, "ACC007");
        store.TryAdd("ACC007", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC007\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: Max Account", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_ValidAccountWithMinimumDecimalValue_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Min Account", decimal.MinValue, "ACC008");
        store.TryAdd("ACC008", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC008\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: Min Account", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_AccountWithNullName_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount(null!, 1000m, "ACC009");
        store.TryAdd("ACC009", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC009\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder:", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_AccountWithEmptyName_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount(string.Empty, 1000m, "ACC010");
        store.TryAdd("ACC010", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC010\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder:", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_AccountWithSpecialCharactersInName_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("José García-O'Brien", 1000m, "ACC011");
        store.TryAdd("ACC011", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC011\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: José García-O'Brien", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_MultipleAccounts_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account1 = new BankAccount("Account One", 1000m, "ACC012");
        var account2 = new BankAccount("Account Two", 2000m, "ACC013");
        store.TryAdd("ACC012", account1);
        store.TryAdd("ACC013", account2);

        var service = CreateBalanceService(store);

        // Check first account
        var input1 = new StringReader("ACC012\n");
        var output1 = new StringWriter();
        Console.SetIn(input1);
        Console.SetOut(output1);
        service.CheckBalance();
        var outputText1 = output1.ToString();
        Assert.Contains("Account Holder: Account One", outputText1);
        Assert.Contains("1,000.00", outputText1);

        // Check second account
        var input2 = new StringReader("ACC013\n");
        var output2 = new StringWriter();
        Console.SetIn(input2);
        Console.SetOut(output2);
        service.CheckBalance();

        // Assert
        var outputText2 = output2.ToString();
        Assert.Contains("Account Holder: Account Two", outputText2);
        Assert.Contains("2,000.00", outputText2);
    }

    #endregion

    #region Invalid Account Scenarios

    [Fact]
    public void CheckBalance_AccountNotFound_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateBalanceService(store);
        var input = new StringReader("INVALID\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
        Assert.DoesNotContain("Account Holder:", outputText);
        Assert.DoesNotContain("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_NullAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateBalanceService(store);
        var input = new StringReader("\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
        Assert.DoesNotContain("Account Holder:", outputText);
        Assert.DoesNotContain("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_EmptyAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateBalanceService(store);
        var input = new StringReader("   \n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
        Assert.DoesNotContain("Account Holder:", outputText);
        Assert.DoesNotContain("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_WhitespaceAccountNumber_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateBalanceService(store);
        var input = new StringReader("   \n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
        Assert.DoesNotContain("Account Holder:", outputText);
        Assert.DoesNotContain("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_AccountNumberWithSpecialCharacters_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var service = CreateBalanceService(store);
        var input = new StringReader("ACC-001_ABC\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account not found", outputText);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void CheckBalance_AfterDeposit_ShowsUpdatedBalance()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC014");
        store.TryAdd("ACC014", account);

        // Deposit first
        account.Deposit(500m);
        Assert.Equal(1500m, account.Balance);

        // Then check balance
        var service = CreateBalanceService(store);
        var input = new StringReader("ACC014\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("1,500.00", outputText);
    }

    [Fact]
    public void CheckBalance_AfterWithdrawal_ShowsUpdatedBalance()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC015");
        store.TryAdd("ACC015", account);

        // Withdraw first
        account.Withdraw(300m);
        Assert.Equal(700m, account.Balance);

        // Then check balance
        var service = CreateBalanceService(store);
        var input = new StringReader("ACC015\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("700.00", outputText);
    }

    [Fact]
    public void CheckBalance_AfterMultipleOperations_ShowsCorrectBalance()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC016");
        store.TryAdd("ACC016", account);

        // Perform multiple operations
        account.Deposit(500m);
        account.Withdraw(200m);
        account.Deposit(100m);
        Assert.Equal(1400m, account.Balance);

        // Then check balance
        var service = CreateBalanceService(store);
        var input = new StringReader("ACC016\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("1,400.00", outputText);
    }

    [Fact]
    public void CheckBalance_MultipleChecks_SameAccount_ShowsConsistentBalance()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC017");
        store.TryAdd("ACC017", account);

        var service = CreateBalanceService(store);

        // First check
        var input1 = new StringReader("ACC017\n");
        var output1 = new StringWriter();
        Console.SetIn(input1);
        Console.SetOut(output1);
        service.CheckBalance();
        var outputText1 = output1.ToString();
        Assert.Contains("1,000.00", outputText1);

        // Second check
        var input2 = new StringReader("ACC017\n");
        var output2 = new StringWriter();
        Console.SetIn(input2);
        Console.SetOut(output2);
        service.CheckBalance();

        // Assert
        var outputText2 = output2.ToString();
        Assert.Contains("1,000.00", outputText2);
        Assert.Equal(1000m, account.Balance);
    }

    [Fact]
    public void CheckBalance_AccountWithZeroAfterWithdrawal_ShowsZero()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC018");
        store.TryAdd("ACC018", account);

        // Withdraw all
        account.Withdraw(1000m);
        Assert.Equal(0m, account.Balance);

        // Then check balance
        var service = CreateBalanceService(store);
        var input = new StringReader("ACC018\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
        Assert.Contains("$0.00", outputText);
    }

    [Fact]
    public void CheckBalance_AccountWithVeryLongName_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var longName = new string('A', 1000);
        var account = new BankAccount(longName, 1000m, "ACC021");
        store.TryAdd("ACC021", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC021\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder:", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_AccountWithUnicodeAccountNumber_DisplaysCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1000m, "ACC001中文");
        store.TryAdd("ACC001中文", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC001中文\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
    }

    [Fact]
    public void CheckBalance_Formatting_DisplaysCurrencyFormat()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("John Doe", 1234.56m, "ACC022");
        store.TryAdd("ACC022", account);

        var service = CreateBalanceService(store);
        var input = new StringReader("ACC022\n");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        // Act
        service.CheckBalance();

        // Assert
        var outputText = output.ToString();
        Assert.Contains("Account Holder: John Doe", outputText);
        Assert.Contains("Current Balance", outputText);
        // Balance should be formatted as currency (C format)
        Assert.True(outputText.Contains("$") || outputText.Contains("1,234.56") || outputText.Contains("1234.56"));
    }

    #endregion
}

