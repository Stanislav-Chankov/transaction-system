using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly IBankAccountStore _accountStore;

        public AccountService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore ?? throw new ArgumentNullException(nameof(accountStore));
        }

        public void CreateAccount(string name, decimal initialBalance, string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                Console.WriteLine("Account number cannot be empty.");
                return;
            }

            if (initialBalance < 0)
            {
                Console.WriteLine("Invalid balance. Please enter a non-negative number.");
                return;
            }

            if (_accountStore.ContainsKey(accountNumber))
            {
                Console.WriteLine("Account number already exists.");
                return;
            }

            var account = new BankAccount(name, initialBalance, accountNumber);
            if (_accountStore.TryAdd(accountNumber, account))
            {
                Console.WriteLine("Account created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create account. Please try again.");
            }
        }
    }
}
