using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.UnitTests;

public class TransferServiceTests
{
    private IBankAccountStore CreateTestAccountStore()
    {
        return new BankAccountStore();
    }

    private TransferService CreateTransferService(IBankAccountStore? store = null)
    {
        return new TransferService(store ?? CreateTestAccountStore());
    }

    [Fact]
    public void TransferMoney_SuccessfulTransfer_UpdatesBalancesCorrectly()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var sender = new BankAccount("Sender", 1000m, "ACC001");
        var recipient = new BankAccount("Recipient", 500m, "ACC002");
        store.TryAdd("ACC001", sender);
        store.TryAdd("ACC002", recipient);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "ACC002", 200m);

        // Assert
        Assert.Equal(800m, sender.Balance);
        Assert.Equal(700m, recipient.Balance);
        var outputText = output.ToString();
        Assert.Contains("Transfer successful", outputText);
    }

    [Fact]
    public void TransferMoney_InsufficientBalance_DoesNotTransfer()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var sender = new BankAccount("Sender", 100m, "ACC001");
        var recipient = new BankAccount("Recipient", 500m, "ACC002");
        store.TryAdd("ACC001", sender);
        store.TryAdd("ACC002", recipient);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "ACC002", 200m);

        // Assert
        Assert.Equal(100m, sender.Balance);
        Assert.Equal(500m, recipient.Balance);
        var outputText = output.ToString();
        Assert.Contains("Insufficient balance", outputText);
    }

    [Fact]
    public void TransferMoney_SenderAccountNotFound_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var recipient = new BankAccount("Recipient", 500m, "ACC002");
        store.TryAdd("ACC002", recipient);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("INVALID", "ACC002", 200m);

        // Assert
        Assert.Equal(500m, recipient.Balance);
        var outputText = output.ToString();
        Assert.Contains("Sender account not found", outputText);
    }

    [Fact]
    public void TransferMoney_RecipientAccountNotFound_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var sender = new BankAccount("Sender", 1000m, "ACC001");
        store.TryAdd("ACC001", sender);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "INVALID", 200m);

        // Assert
        Assert.Equal(1000m, sender.Balance);
        var outputText = output.ToString();
        Assert.Contains("Recipient account not found", outputText);
    }

    [Fact]
    public void TransferMoney_SameSenderAndRecipient_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var account = new BankAccount("Account", 1000m, "ACC001");
        store.TryAdd("ACC001", account);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "ACC001", 200m);

        // Assert
        Assert.Equal(1000m, account.Balance);
        var outputText = output.ToString();
        Assert.Contains("Sender and recipient accounts must be different", outputText);
    }

    [Fact]
    public void TransferMoney_InvalidAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var sender = new BankAccount("Sender", 1000m, "ACC001");
        var recipient = new BankAccount("Recipient", 500m, "ACC002");
        store.TryAdd("ACC001", sender);
        store.TryAdd("ACC002", recipient);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "ACC002", -100m);

        // Assert
        Assert.Equal(1000m, sender.Balance);
        Assert.Equal(500m, recipient.Balance);
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void TransferMoney_ZeroAmount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var sender = new BankAccount("Sender", 1000m, "ACC001");
        var recipient = new BankAccount("Recipient", 500m, "ACC002");
        store.TryAdd("ACC001", sender);
        store.TryAdd("ACC002", recipient);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "ACC002", 0m);

        // Assert
        Assert.Equal(1000m, sender.Balance);
        Assert.Equal(500m, recipient.Balance);
        var outputText = output.ToString();
        Assert.Contains("Invalid amount", outputText);
    }

    [Fact]
    public void TransferMoney_EmptySenderAccount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var recipient = new BankAccount("Recipient", 500m, "ACC002");
        store.TryAdd("ACC002", recipient);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("", "ACC002", 200m);

        // Assert
        Assert.Equal(500m, recipient.Balance);
        var outputText = output.ToString();
        Assert.Contains("Sender account number cannot be empty", outputText);
    }

    [Fact]
    public void TransferMoney_EmptyRecipientAccount_ReturnsError()
    {
        // Arrange
        var store = CreateTestAccountStore();
        var sender = new BankAccount("Sender", 1000m, "ACC001");
        store.TryAdd("ACC001", sender);

        var service = CreateTransferService(store);
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        service.TransferMoney("ACC001", "", 200m);

        // Assert
        Assert.Equal(1000m, sender.Balance);
        var outputText = output.ToString();
        Assert.Contains("Recipient account number cannot be empty", outputText);
    }
}
