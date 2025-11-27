using System;

using AccessFinance.TransactionSystem.Services.Abstract;

namespace AccessFinance.TransactionSystem.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IBankAccountStore _accountStore;

        public WithdrawService(IBankAccountStore accountStore)
        {
            _accountStore = accountStore;
        }

        public void WithdrawMoney()
        {
            Console.Write("Enter account number: ");
            var accountNumber = Console.ReadLine();

            if (!_accountStore.TryGet(accountNumber!, out var account) || account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter withdrawal amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
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
