using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IBankAccountStore _accountStore;

        public BalanceService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore ?? throw new ArgumentNullException(nameof(accountStore));
        }

        public void CheckBalance(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                Console.WriteLine("Account number cannot be empty.");
                return;
            }

            if (!_accountStore.TryGet(accountNumber, out var account) || account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.WriteLine($"Account Holder: {account.Name}");
            Console.WriteLine($"Current Balance: {account.Balance:C}");
        }
    }
}
