using AccessFinance.TransactionSystem.Enums;
using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;

using Microsoft.Extensions.DependencyInjection;

namespace AccessFinance.TransactionSystem;

internal partial class Program
{
    private static void Main()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IBankAccountStore, BankAccountStore>();
        serviceCollection.AddSingleton<IAccountService, AccountService>();
        serviceCollection.AddSingleton<IDepositService, DepositService>();
        serviceCollection.AddSingleton<IWithdrawService, WithdrawService>();
        serviceCollection.AddSingleton<IBalanceService, BalanceService>();
        serviceCollection.AddSingleton<ITransferService, TransferService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var accountService = serviceProvider.GetRequiredService<IAccountService>();
        var depositService = serviceProvider.GetRequiredService<IDepositService>();
        var withdrawService = serviceProvider.GetRequiredService<IWithdrawService>();
        var balanceService = serviceProvider.GetRequiredService<IBalanceService>();
        var transferService = serviceProvider.GetRequiredService<ITransferService>();

        Console.WriteLine("Welcome to AccessFinance Transaction System!");

        while (true)
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Create an Account");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Check Account Balance");
            Console.WriteLine("5. Transfer Money");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out var intChoice) ||
                !Enum.IsDefined(typeof(MenuOption), intChoice))
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }

            var choice = (MenuOption)intChoice;

            switch (choice)
            {
                case MenuOption.CreateAccount:
                    Console.Write("Enter your name: ");
                    var name = Console.ReadLine();
                    Console.Write("Enter initial balance: ");
                    if (decimal.TryParse(Console.ReadLine(), out var initialBalance))
                    {
                        Console.Write("Enter a unique account number: ");
                        var accountNumber = Console.ReadLine();
                        accountService.CreateAccount(name!, initialBalance, accountNumber!);
                    }
                    else
                    {
                        Console.WriteLine("Invalid balance. Please enter a valid number.");
                    }
                    break;
                case MenuOption.DepositMoney:
                    Console.Write("Enter account number: ");
                    var depositAccountNumber = Console.ReadLine();
                    Console.Write("Enter deposit amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out var depositAmount))
                    {
                        depositService.DepositMoney(depositAccountNumber!, depositAmount);
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                    }
                    break;
                case MenuOption.WithdrawMoney:
                    Console.Write("Enter account number: ");
                    var withdrawAccountNumber = Console.ReadLine();
                    Console.Write("Enter withdrawal amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out var withdrawAmount))
                    {
                        withdrawService.WithdrawMoney(withdrawAccountNumber!, withdrawAmount);
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                    }
                    break;
                case MenuOption.CheckBalance:
                    Console.Write("Enter account number: ");
                    var balanceAccountNumber = Console.ReadLine();
                    balanceService.CheckBalance(balanceAccountNumber!);
                    break;
                case MenuOption.TransferMoney:
                    Console.Write("Enter your account number (sender): ");
                    var senderAccountNumber = Console.ReadLine();
                    Console.Write("Enter recipient account number: ");
                    var recipientAccountNumber = Console.ReadLine();
                    Console.Write("Enter transfer amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out var transferAmount))
                    {
                        transferService.TransferMoney(senderAccountNumber!, recipientAccountNumber!, transferAmount);
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                    }
                    break;
                case MenuOption.Exit:
                    ExitProgram();
                    return;
                default:
                    InvalidChoice();
                    break;
            }
        }
    }

    private static void InvalidChoice()
    {
        Console.WriteLine("Invalid choice. Please try again.");
    }

    private static void ExitProgram()
    {
        Console.WriteLine("Thank you for using AccessFinance. Goodbye!");
    }
}
