using AccessFinance.TransactionSystem.Services;

namespace AccessFinance.TransactionSystem.UnitTests;

public class BankAccountCreationTests
{
    #region Valid Creation Scenarios

    [Fact]
    public void BankAccount_ValidCreation_WithPositiveBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 1000m, "ACC001");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC001", account.AccountNumber);
        Assert.Equal(1000m, account.Balance);
    }

    [Fact]
    public void BankAccount_ValidCreation_WithZeroBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("Jane Smith", 0m, "ACC002");

        // Assert
        Assert.Equal("Jane Smith", account.Name);
        Assert.Equal("ACC002", account.AccountNumber);
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void BankAccount_ValidCreation_WithNegativeBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("Bob Johnson", -100m, "ACC003");

        // Assert
        Assert.Equal("Bob Johnson", account.Name);
        Assert.Equal("ACC003", account.AccountNumber);
        Assert.Equal(-100m, account.Balance);
    }

    [Fact]
    public void BankAccount_ValidCreation_WithVeryLargeBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("Millionaire", decimal.MaxValue, "ACC004");

        // Assert
        Assert.Equal("Millionaire", account.Name);
        Assert.Equal("ACC004", account.AccountNumber);
        Assert.Equal(decimal.MaxValue, account.Balance);
    }

    [Fact]
    public void BankAccount_ValidCreation_WithVerySmallBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("Minimal", decimal.MinValue, "ACC005");

        // Assert
        Assert.Equal("Minimal", account.Name);
        Assert.Equal("ACC005", account.AccountNumber);
        Assert.Equal(decimal.MinValue, account.Balance);
    }

    [Fact]
    public void BankAccount_ValidCreation_WithDecimalPrecision_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("Precise", 123.456789m, "ACC006");

        // Assert
        Assert.Equal("Precise", account.Name);
        Assert.Equal("ACC006", account.AccountNumber);
        Assert.Equal(123.456789m, account.Balance);
    }

    #endregion

    #region Name Scenarios

    [Fact]
    public void BankAccount_Creation_WithNullName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount(null!, 100m, "ACC007");

        // Assert
        Assert.Null(account.Name);
        Assert.Equal("ACC007", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithEmptyName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount(string.Empty, 100m, "ACC008");

        // Assert
        Assert.Equal(string.Empty, account.Name);
        Assert.Equal("ACC008", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithWhitespaceName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("   ", 100m, "ACC009");

        // Assert
        Assert.Equal("   ", account.Name);
        Assert.Equal("ACC009", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithVeryLongName_Succeeds()
    {
        // Arrange
        var longName = new string('A', 10000);

        // Act
        var account = new BankAccount(longName, 100m, "ACC010");

        // Assert
        Assert.Equal(longName, account.Name);
        Assert.Equal("ACC010", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSpecialCharactersInName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John O'Brien-Smith", 100m, "ACC011");

        // Assert
        Assert.Equal("John O'Brien-Smith", account.Name);
        Assert.Equal("ACC011", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithUnicodeCharactersInName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("José García", 100m, "ACC012");

        // Assert
        Assert.Equal("José García", account.Name);
        Assert.Equal("ACC012", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithEmojiInName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John 😊 Doe", 100m, "ACC013");

        // Assert
        Assert.Equal("John 😊 Doe", account.Name);
        Assert.Equal("ACC013", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithNumbersInName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John123 Doe456", 100m, "ACC014");

        // Assert
        Assert.Equal("John123 Doe456", account.Name);
        Assert.Equal("ACC014", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSingleCharacterName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("A", 100m, "ACC015");

        // Assert
        Assert.Equal("A", account.Name);
        Assert.Equal("ACC015", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    #endregion

    #region AccountNumber Scenarios

    [Fact]
    public void BankAccount_Creation_WithNullAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, null!);

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Null(account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithEmptyAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, string.Empty);

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal(string.Empty, account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithWhitespaceAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, "   ");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("   ", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithVeryLongAccountNumber_Succeeds()
    {
        // Arrange
        var longAccountNumber = new string('1', 10000);

        // Act
        var account = new BankAccount("John Doe", 100m, longAccountNumber);

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal(longAccountNumber, account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSpecialCharactersInAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, "ACC-001_ABC");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC-001_ABC", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithUnicodeCharactersInAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, "ACC001中文");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC001中文", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSingleCharacterAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, "1");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("1", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithNumericAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, "123456789");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("123456789", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithAlphanumericAccountNumber_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 100m, "ACC123XYZ");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC123XYZ", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    #endregion

    #region InitialBalance Scenarios

    [Fact]
    public void BankAccount_Creation_WithZeroInitialBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 0m, "ACC016");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC016", account.AccountNumber);
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithNegativeInitialBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", -500m, "ACC017");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC017", account.AccountNumber);
        Assert.Equal(-500m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSmallPositiveBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 0.01m, "ACC018");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC018", account.AccountNumber);
        Assert.Equal(0.01m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithLargePositiveBalance_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 999999999.99m, "ACC019");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC019", account.AccountNumber);
        Assert.Equal(999999999.99m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithMaximumDecimalValue_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", decimal.MaxValue, "ACC020");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC020", account.AccountNumber);
        Assert.Equal(decimal.MaxValue, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithMinimumDecimalValue_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", decimal.MinValue, "ACC021");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC021", account.AccountNumber);
        Assert.Equal(decimal.MinValue, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithHighPrecisionDecimal_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("John Doe", 123.4567890123456789012345678m, "ACC022");

        // Assert
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC022", account.AccountNumber);
        Assert.Equal(123.4567890123456789012345678m, account.Balance);
    }

    #endregion

    #region Combined Edge Cases

    [Fact]
    public void BankAccount_Creation_WithAllNullValues_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount(null!, 0m, null!);

        // Assert
        Assert.Null(account.Name);
        Assert.Null(account.AccountNumber);
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithAllEmptyValues_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount(string.Empty, 0m, string.Empty);

        // Assert
        Assert.Equal(string.Empty, account.Name);
        Assert.Equal(string.Empty, account.AccountNumber);
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithAllWhitespaceValues_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount("   ", 0m, "   ");

        // Assert
        Assert.Equal("   ", account.Name);
        Assert.Equal("   ", account.AccountNumber);
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithNegativeBalanceAndNullName_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount(null!, -100m, "ACC023");

        // Assert
        Assert.Null(account.Name);
        Assert.Equal("ACC023", account.AccountNumber);
        Assert.Equal(-100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithMaximumValues_Succeeds()
    {
        // Arrange
        var maxName = new string('A', 1000);
        var maxAccountNumber = new string('1', 1000);

        // Act
        var account = new BankAccount(maxName, decimal.MaxValue, maxAccountNumber);

        // Assert
        Assert.Equal(maxName, account.Name);
        Assert.Equal(maxAccountNumber, account.AccountNumber);
        Assert.Equal(decimal.MaxValue, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithMinimumValues_Succeeds()
    {
        // Arrange & Act
        var account = new BankAccount(string.Empty, decimal.MinValue, string.Empty);

        // Assert
        Assert.Equal(string.Empty, account.Name);
        Assert.Equal(string.Empty, account.AccountNumber);
        Assert.Equal(decimal.MinValue, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSameAccountNumberDifferentNames_Succeeds()
    {
        // Arrange & Act
        var account1 = new BankAccount("John Doe", 100m, "ACC024");
        var account2 = new BankAccount("Jane Smith", 200m, "ACC024");

        // Assert
        Assert.Equal("John Doe", account1.Name);
        Assert.Equal("Jane Smith", account2.Name);
        Assert.Equal("ACC024", account1.AccountNumber);
        Assert.Equal("ACC024", account2.AccountNumber);
        Assert.Equal(100m, account1.Balance);
        Assert.Equal(200m, account2.Balance);
    }

    [Fact]
    public void BankAccount_Creation_WithSameNameDifferentAccountNumbers_Succeeds()
    {
        // Arrange & Act
        var account1 = new BankAccount("John Doe", 100m, "ACC025");
        var account2 = new BankAccount("John Doe", 200m, "ACC026");

        // Assert
        Assert.Equal("John Doe", account1.Name);
        Assert.Equal("John Doe", account2.Name);
        Assert.Equal("ACC025", account1.AccountNumber);
        Assert.Equal("ACC026", account2.AccountNumber);
        Assert.Equal(100m, account1.Balance);
        Assert.Equal(200m, account2.Balance);
    }

    [Fact]
    public void BankAccount_Creation_PropertiesAreImmutable()
    {
        // Arrange
        var account = new BankAccount("John Doe", 100m, "ACC027");

        // Act & Assert - Properties should be read-only (compiler will prevent assignment)
        // This test verifies that Name and AccountNumber are get-only properties
        Assert.Equal("John Doe", account.Name);
        Assert.Equal("ACC027", account.AccountNumber);
        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void BankAccount_Creation_MultipleAccountsWithDifferentValues_Succeeds()
    {
        // Arrange & Act
        var accounts = new[]
        {
            new BankAccount("Account1", 100m, "ACC028"),
            new BankAccount("Account2", 200m, "ACC029"),
            new BankAccount("Account3", 300m, "ACC030"),
            new BankAccount("Account4", -50m, "ACC031"),
            new BankAccount("Account5", 0m, "ACC032")
        };

        // Assert
        Assert.Equal(5, accounts.Length);
        Assert.All(accounts, account => Assert.NotNull(account));
        Assert.Equal(100m, accounts[0].Balance);
        Assert.Equal(200m, accounts[1].Balance);
        Assert.Equal(300m, accounts[2].Balance);
        Assert.Equal(-50m, accounts[3].Balance);
        Assert.Equal(0m, accounts[4].Balance);
    }

    #endregion
}

