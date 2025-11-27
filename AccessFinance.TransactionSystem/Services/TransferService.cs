using System;

using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class TransferService : ITransferService
    {
        private readonly IBankAccountStore _accountStore;

        public TransferService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore;
        }

        public void TransferMoney()
        {
            Console.Write("Enter your account number (sender): ");
            var senderAccountNumber = Console.ReadLine();

            if (!_accountStore.TryGet(senderAccountNumber!, out var senderAccount) || senderAccount == null)
            {
                Console.WriteLine("Sender account not found.");
                return;
            }

            Console.Write("Enter recipient account number: ");
            var recipientAccountNumber = Console.ReadLine();

            if (!_accountStore.TryGet(recipientAccountNumber!, out var recipientAccount) || recipientAccount == null)
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

            var firstLock = string.Compare(senderAccountNumber, recipientAccountNumber, StringComparison.Ordinal) < 0
                ? senderAccount
                : recipientAccount;
            var secondLock = firstLock == senderAccount ? recipientAccount : senderAccount;

            lock (firstLock)
            {
                lock (secondLock)
                {
                    if (senderAccount.Balance < amount)
                    {
                        Console.WriteLine("Insufficient balance in sender account.");
                        return;
                    }

                    senderAccount.Withdraw(amount);
                    recipientAccount.Deposit(amount);

                    Console.WriteLine($"Transfer successful. Sender new balance: {senderAccount.Balance:C}, Recipient new balance: {recipientAccount.Balance:C}");
                }
            }
        }
    }
}
