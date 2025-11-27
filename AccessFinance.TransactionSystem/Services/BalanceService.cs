using System.Collections.Concurrent;

using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IBankAccountStore _accountStore;

        public BalanceService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore;
        }

        public void CheckBalance()
        {
            Console.Write("Enter account number: ");
            var accountNumber = Console.ReadLine();

            if (!_accountStore.TryGet(accountNumber!, out var account) || account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.WriteLine($"Account Holder: {account.Name}");
            Console.WriteLine($"Current Balance: {account.Balance:C}");
        }
    }
}
