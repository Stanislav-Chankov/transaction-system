using AccessFinance.TransactionSystem.Services;

namespace AccessFinance.TransactionSystem.Services.Abstract
{
    public interface IBankAccountStore
    {
        bool TryAdd(string accountNumber, BankAccount account);

        bool ContainsKey(string accountNumber);

        bool TryGet(string accountNumber, out BankAccount? account);

        BankAccount? Get(string accountNumber);
    }
}