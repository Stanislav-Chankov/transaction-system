namespace AccessFinance.TransactionSystem.Services.Abstract;

public interface ITransferService
{
    void TransferMoney(string senderAccountNumber, string recipientAccountNumber, decimal amount);
}