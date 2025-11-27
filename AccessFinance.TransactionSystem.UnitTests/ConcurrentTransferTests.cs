using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccessFinance.TransactionSystem;
using AccessFinance.TransactionSystem.Services;
using AccessFinance.TransactionSystem.Services.Abstract;
using Xunit;

namespace AccessFinance.TransactionSystem.UnitTests;

public class ConcurrentTransferTests
{
    [Fact]
    public void ConcurrentTransfers_SameAccounts_AreSerialized()
    {
        // Arrange
        var store = new BankAccountStore();
        var sender = new BankAccount("Sender", 10000m, "ACC001");
        var recipient = new BankAccount("Recipient", 0m, "ACC002");
        store.TryAdd("ACC001", sender);
        store.TryAdd("ACC002", recipient);

        const int numberOfTransfers = 100;
        const decimal transferAmount = 10m;
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        // Act - Execute multiple transfers concurrently on the same accounts
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () =>
                    {
                        if (sender.Balance >= transferAmount)
                        {
                            sender.Withdraw(transferAmount);
                            recipient.Deposit(transferAmount);
                        }
                    }, timeoutMilliseconds: 5000);
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        Assert.Empty(exceptions);
        var expectedSenderBalance = 10000m - (numberOfTransfers * transferAmount);
        var expectedRecipientBalance = numberOfTransfers * transferAmount;
        Assert.Equal(expectedSenderBalance, sender.Balance);
        Assert.Equal(expectedRecipientBalance, recipient.Balance);
    }

    [Fact]
    public void ConcurrentTransfers_DifferentAccounts_CanProceedSimultaneously()
    {
        // Arrange
        var store = new BankAccountStore();
        var sender1 = new BankAccount("Sender1", 1000m, "ACC001");
        var recipient1 = new BankAccount("Recipient1", 0m, "ACC002");
        var sender2 = new BankAccount("Sender2", 1000m, "ACC003");
        var recipient2 = new BankAccount("Recipient2", 0m, "ACC004");
        store.TryAdd("ACC001", sender1);
        store.TryAdd("ACC002", recipient1);
        store.TryAdd("ACC003", sender2);
        store.TryAdd("ACC004", recipient2);

        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        // Act - Execute transfers on different account pairs concurrently
        tasks.Add(Task.Run(() =>
        {
            try
            {
                AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () =>
                {
                    Thread.Sleep(100); // Simulate some work
                    sender1.Withdraw(100m);
                    recipient1.Deposit(100m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        }));

        tasks.Add(Task.Run(() =>
        {
            try
            {
                AccountLockManager.ExecuteWithLocks("ACC003", "ACC004", () =>
                {
                    Thread.Sleep(100); // Simulate some work
                    sender2.Withdraw(200m);
                    recipient2.Deposit(200m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        }));

        Task.WaitAll(tasks.ToArray());

        // Assert - Both transfers should complete successfully
        Assert.Empty(exceptions);
        Assert.Equal(900m, sender1.Balance);
        Assert.Equal(100m, recipient1.Balance);
        Assert.Equal(800m, sender2.Balance);
        Assert.Equal(200m, recipient2.Balance);
    }

    [Fact]
    public void ConcurrentTransfers_OverlappingAccounts_AreSerialized()
    {
        // Arrange
        var store = new BankAccountStore();
        var account1 = new BankAccount("Account1", 1000m, "ACC001");
        var account2 = new BankAccount("Account2", 1000m, "ACC002");
        var account3 = new BankAccount("Account3", 1000m, "ACC003");
        store.TryAdd("ACC001", account1);
        store.TryAdd("ACC002", account2);
        store.TryAdd("ACC003", account3);

        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        // Act - Transfer from ACC001 to ACC002, and from ACC002 to ACC003 concurrently
        // These should be serialized because they share ACC002
        tasks.Add(Task.Run(() =>
        {
            try
            {
                AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () =>
                {
                    Thread.Sleep(50);
                    account1.Withdraw(100m);
                    account2.Deposit(100m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        }));

        tasks.Add(Task.Run(() =>
        {
            try
            {
                AccountLockManager.ExecuteWithLocks("ACC002", "ACC003", () =>
                {
                    Thread.Sleep(50);
                    account2.Withdraw(50m);
                    account3.Deposit(50m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        }));

        Task.WaitAll(tasks.ToArray());

        // Assert - Both transfers should complete successfully
        Assert.Empty(exceptions);
        Assert.Equal(900m, account1.Balance);
        Assert.Equal(1050m, account2.Balance); // +100 from first, -50 from second
        Assert.Equal(1050m, account3.Balance);
    }

    [Fact]
    public void ConcurrentTransfers_DeadlockPrevention_WorksCorrectly()
    {
        // Arrange
        var store = new BankAccountStore();
        var account1 = new BankAccount("Account1", 1000m, "ACC001");
        var account2 = new BankAccount("Account2", 1000m, "ACC002");
        store.TryAdd("ACC001", account1);
        store.TryAdd("ACC002", account2);

        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        var completedTransfers = 0;

        // Act - Transfer in both directions concurrently
        // This would deadlock without proper lock ordering, but should work with alphabetical ordering
        tasks.Add(Task.Run(() =>
        {
            try
            {
                AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () =>
                {
                    Interlocked.Increment(ref completedTransfers);
                    Thread.Sleep(50);
                    account1.Withdraw(100m);
                    account2.Deposit(100m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        }));

        tasks.Add(Task.Run(() =>
        {
            try
            {
                AccountLockManager.ExecuteWithLocks("ACC002", "ACC001", () =>
                {
                    Interlocked.Increment(ref completedTransfers);
                    Thread.Sleep(50);
                    account2.Withdraw(50m);
                    account1.Deposit(50m);
                }, timeoutMilliseconds: 5000);
            }
            catch (Exception ex)
            {
                lock (exceptions)
                {
                    exceptions.Add(ex);
                }
            }
        }));

        Task.WaitAll(tasks.ToArray());

        // Assert - Both transfers should complete without deadlock
        Assert.Empty(exceptions);
        Assert.Equal(2, completedTransfers);
        // Account1: 1000 - 100 + 50 = 950
        // Account2: 1000 + 100 - 50 = 1050
        Assert.Equal(950m, account1.Balance);
        Assert.Equal(1050m, account2.Balance);
    }

    [Fact]
    public void ConcurrentTransfers_Timeout_ThrowsTimeoutException()
    {
        // Arrange
        var lock1 = AccountLockManager.GetLock("ACC001");
        var lock2 = AccountLockManager.GetLock("ACC002");

        // Hold the lock in another thread
        var tcs = new TaskCompletionSource<bool>();
        var thread = new Thread(() =>
        {
            lock (lock1)
            {
                lock (lock2)
                {
                    tcs.Task.Wait(); // Hold locks indefinitely
                }
            }
        });
        thread.Start();

        // Wait a moment for thread to acquire locks
        Thread.Sleep(50);

        // Act & Assert - Try to acquire locks with short timeout
        Assert.Throws<TimeoutException>(() =>
        {
            AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () => { }, timeoutMilliseconds: 100);
        });

        // Cleanup
        tcs.SetResult(true);
        thread.Join();
    }
}

