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
                    accountService.CreateAccount();
                    break;
                case MenuOption.DepositMoney:
                    depositService.DepositMoney();
                    break;
                case MenuOption.WithdrawMoney:
                    withdrawService.WithdrawMoney();
                    break;
                case MenuOption.CheckBalance:
                    balanceService.CheckBalance();
                    break;
                case MenuOption.TransferMoney:
                    transferService.TransferMoney();
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
