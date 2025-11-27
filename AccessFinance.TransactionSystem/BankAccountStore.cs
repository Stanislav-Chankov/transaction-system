using System.Collections.Concurrent;

using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem
{
    public class BankAccountStore : IBankAccountStore
    {
        private readonly ConcurrentDictionary<string, BankAccount> _accounts = new();

        public bool TryAdd(string accountNumber, BankAccount account) =>
            _accounts.TryAdd(accountNumber, account);

        public bool ContainsKey(string accountNumber) =>
            _accounts.ContainsKey(accountNumber);

        public bool TryGet(string accountNumber, out BankAccount? account) =>
            _accounts.TryGetValue(accountNumber, out account);

        public BankAccount? Get(string accountNumber) =>
            _accounts.TryGetValue(accountNumber, out var account) ? account : null;

        public IEnumerable<BankAccount> GetAllAccounts() => _accounts.Values;
    }
}