namespace AccessFinance.TransactionSystem.Services.Abstract;

public interface IAccountService
{
    void CreateAccount(string name, decimal initialBalance, string accountNumber);
}