using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class TransferService : ITransferService
    {
        private readonly IBankAccountStore _accountStore;

        public TransferService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore ?? throw new ArgumentNullException(nameof(accountStore));
        }

        public void TransferMoney()
        {
            Console.Write("Enter your account number (sender): ");
            var senderAccountNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(senderAccountNumber) ||
                !_accountStore.TryGet(senderAccountNumber, out var senderAccount) ||
                senderAccount == null)
            {
                Console.WriteLine("Sender account not found.");
                return;
            }

            Console.Write("Enter recipient account number: ");
            var recipientAccountNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(recipientAccountNumber))
            {
                Console.WriteLine("Recipient account not found.");
                return;
            }

            if (string.Equals(senderAccountNumber, recipientAccountNumber, StringComparison.Ordinal))
            {
                Console.WriteLine("Sender and recipient accounts must be different.");
                return;
            }

            if (!_accountStore.TryGet(recipientAccountNumber, out var recipientAccount) || recipientAccount == null)
            {
                Console.WriteLine("Recipient account not found.");
                return;
            }

            Console.Write("Enter transfer amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
                return;
            }

            // Execute transfer with proper locking using AccountLockManager
            AccountLockManager.ExecuteWithLocks(senderAccountNumber, recipientAccountNumber, () =>
            {
                if (!senderAccount.Withdraw(amount))
                {
                    Console.WriteLine("Insufficient balance in sender account.");
                    return;
                }

                recipientAccount.Deposit(amount);
                Console.WriteLine(
                    $"Transfer successful. Sender new balance: {senderAccount.Balance:C}, Recipient new balance: {recipientAccount.Balance:C}");
            }, timeoutMilliseconds: 5_000);
        }
    }
}
