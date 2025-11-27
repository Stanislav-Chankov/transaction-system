using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IBankAccountStore _accountStore;

        public WithdrawService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore ?? throw new ArgumentNullException(nameof(accountStore));
        }

        public void WithdrawMoney(string accountNumber, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                Console.WriteLine("Account number cannot be empty.");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
                return;
            }

            if (!_accountStore.TryGet(accountNumber, out var account) || account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            if (!account.Withdraw(amount))
            {
                Console.WriteLine("Insufficient balance.");
                return;
            }

            Console.WriteLine($"Withdrawal successful. New balance: {account.Balance:C}");
        }
    }
}
