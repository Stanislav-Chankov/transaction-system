namespace AccessFinance.TransactionSystem.Services.Abstract;

public interface IWithdrawService
{
    void WithdrawMoney(string accountNumber, decimal amount);
}