using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class DepositService : IDepositService
    {
        private readonly IBankAccountStore _accountStore;

        public DepositService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore;
        }

        public void DepositMoney()
        {
            Console.Write("Enter account number: ");
            var accountNumber = Console.ReadLine();

            if (!_accountStore.TryGet(accountNumber!, out var account) || account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter deposit amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
                return;
            }

            account.Deposit(amount);
            Console.WriteLine($"Deposit successful. New balance: {account.Balance:C}");
        }
    }
}
