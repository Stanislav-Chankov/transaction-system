using AccessFinance.TransactionSystem.Services;

namespace AccessFinance.TransactionSystem.UnitTests;

public class SimultaneousBidirectionalTransferTest
{
    [Fact]
    public void TransferMoney_SimultaneousAtoBAndBtoA_CompletesSuccessfully()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 10m, "ACC_A");
        var accountB = new BankAccount("AccountB", 0m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        var startSignal = new CountdownEvent(1);
        var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();

        // Act - Start both transfers simultaneously
        var transfer1 = Task.Run(() =>
        {
            startSignal.Wait(); // Wait for start signal
            try
            {
                // First transfer: A -> B, transfer $10
                AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                {
                    accountA.Withdraw(10m);
                    accountB.Deposit(10m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        var transfer2 = Task.Run(() =>
        {
            startSignal.Wait(); // Wait for start signal
            try
            {
                // Second transfer: B -> A, transfer $10 (the $10 received from account A)
                AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                {
                    accountB.Withdraw(10m);
                    accountA.Deposit(10m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        // Start both transfers at the same time
        startSignal.Signal();
        Task.WaitAll(transfer1, transfer2);

        // Assert
        Assert.Empty(exceptions);
        Assert.Equal(10m, accountA.Balance); // A should have $10 back
        Assert.Equal(0m, accountB.Balance);  // B should have $0 (gave it back to A)
    }
}

