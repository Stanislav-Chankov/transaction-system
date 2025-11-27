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

        public void TransferMoney(string senderAccountNumber, string recipientAccountNumber, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(senderAccountNumber))
            {
                Console.WriteLine("Sender account number cannot be empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(recipientAccountNumber))
            {
                Console.WriteLine("Recipient account number cannot be empty.");
                return;
            }

            if (string.Equals(senderAccountNumber, recipientAccountNumber, StringComparison.Ordinal))
            {
                Console.WriteLine("Sender and recipient accounts must be different.");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
                return;
            }

            if (!_accountStore.TryGet(senderAccountNumber, out var senderAccount) || senderAccount == null)
            {
                Console.WriteLine("Sender account not found.");
                return;
            }

            if (!_accountStore.TryGet(recipientAccountNumber, out var recipientAccount) || recipientAccount == null)
            {
                Console.WriteLine("Recipient account not found.");
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
