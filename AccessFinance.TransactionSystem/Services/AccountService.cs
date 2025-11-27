using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly IBankAccountStore _accountStore;

        public AccountService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore;
        }

        public void CreateAccount()
        {
            Console.Write("Enter your name: ");
            var name = Console.ReadLine();

            Console.Write("Enter initial balance: ");
            if (!decimal.TryParse(Console.ReadLine(), out var initialBalance) || initialBalance < 0)
            {
                Console.WriteLine("Invalid balance. Please enter a non-negative number.");
                return;
            }

            Console.Write("Enter a unique account number: ");
            var accountNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(accountNumber) || _accountStore.ContainsKey(accountNumber!))
            {
                Console.WriteLine("Account number is invalid or already exists.");
                return;
            }

            var account = new BankAccount(name, initialBalance, accountNumber);
            if (_accountStore.TryAdd(accountNumber!, account))
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
