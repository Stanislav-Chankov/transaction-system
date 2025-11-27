using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccessFinance.TransactionSystem;
using AccessFinance.TransactionSystem.Services;
using Xunit;

namespace AccessFinance.TransactionSystem.UnitTests;

public class ComplexConcurrentTransferTests
{
    [Fact]
    public void SimultaneousBidirectionalTransfers_HighConcurrency_NoDeadlocks()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 100000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 100000m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        const int numberOfTransfers = 1000;
        const decimal transferAmount = 1m;
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        var completedTransfers = new ConcurrentBag<string>();

        // Act - Start transfers in both directions simultaneously without any delays
        var startSignal = new CountdownEvent(1);

        // Transfer A -> B
        for (int i = 0; i < numberOfTransfers; i++)
        {
            var transferId = $"A->B-{i}";
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait(); // Wait for start signal
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        if (accountA.Balance >= transferAmount)
                        {
                            accountA.Withdraw(transferAmount);
                            accountB.Deposit(transferAmount);
                            completedTransfers.Add(transferId);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // Transfer B -> A
        for (int i = 0; i < numberOfTransfers; i++)
        {
            var transferId = $"B->A-{i}";
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait(); // Wait for start signal
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                    {
                        if (accountB.Balance >= transferAmount)
                        {
                            accountB.Withdraw(transferAmount);
                            accountA.Deposit(transferAmount);
                            completedTransfers.Add(transferId);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // Start all transfers simultaneously
        startSignal.Signal();
        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(60));

        // Assert
        Assert.Empty(exceptions);
        var totalCompleted = completedTransfers.Count;
        Assert.True(totalCompleted > 0, "At least some transfers should have completed");
        
        // Verify balances are consistent (total should remain 200000)
        var totalBalance = accountA.Balance + accountB.Balance;
        Assert.Equal(200000m, totalBalance);
    }

    [Fact]
    public void SimultaneousBidirectionalTransfers_MultipleAmounts_NoDeadlocks()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 50000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 50000m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        var amounts = new[] { 1m, 5m, 10m, 25m, 50m, 100m };
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        var transferCount = new ConcurrentBag<int>();

        // Act - Transfer different amounts in both directions simultaneously
        var startSignal = new CountdownEvent(1);

        foreach (var amount in amounts)
        {
            // A -> B
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    startSignal.Wait();
                    try
                    {
                        AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                        {
                            if (accountA.Balance >= amount)
                            {
                                accountA.Withdraw(amount);
                                accountB.Deposit(amount);
                                transferCount.Add(1);
                            }
                        }, timeoutMilliseconds: 30000);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }));
            }

            // B -> A
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    startSignal.Wait();
                    try
                    {
                        AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                        {
                            if (accountB.Balance >= amount)
                            {
                                accountB.Withdraw(amount);
                                accountA.Deposit(amount);
                                transferCount.Add(1);
                            }
                        }, timeoutMilliseconds: 30000);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }));
            }
        }

        startSignal.Signal();
        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(60));

        // Assert
        Assert.Empty(exceptions);
        var totalBalance = accountA.Balance + accountB.Balance;
        Assert.Equal(100000m, totalBalance);
    }

    [Fact]
    public void SimultaneousBidirectionalTransfers_ThreeWay_NoDeadlocks()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 100000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 100000m, "ACC_B");
        var accountC = new BankAccount("AccountC", 100000m, "ACC_C");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);
        store.TryAdd("ACC_C", accountC);

        const int numberOfTransfers = 500;
        const decimal transferAmount = 1m;
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();

        // Act - Create circular transfers: A->B, B->C, C->A simultaneously
        var startSignal = new CountdownEvent(1);

        // A -> B
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        if (accountA.Balance >= transferAmount)
                        {
                            accountA.Withdraw(transferAmount);
                            accountB.Deposit(transferAmount);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // B -> C
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_C", () =>
                    {
                        if (accountB.Balance >= transferAmount)
                        {
                            accountB.Withdraw(transferAmount);
                            accountC.Deposit(transferAmount);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // C -> A
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_C", "ACC_A", () =>
                    {
                        if (accountC.Balance >= transferAmount)
                        {
                            accountC.Withdraw(transferAmount);
                            accountA.Deposit(transferAmount);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        startSignal.Signal();
        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(60));

        // Assert
        Assert.Empty(exceptions);
        var totalBalance = accountA.Balance + accountB.Balance + accountC.Balance;
        Assert.Equal(300000m, totalBalance);
    }

    [Fact]
    public void SimultaneousBidirectionalTransfers_StressTest_NoDataCorruption()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 1000000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 1000000m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        const int numberOfTransfers = 10000;
        const decimal transferAmount = 0.01m;
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        var balanceSnapshots = new ConcurrentBag<(decimal A, decimal B)>();

        // Act - Extreme stress test with balance snapshots
        var startSignal = new CountdownEvent(1);

        // Transfer A -> B
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        if (accountA.Balance >= transferAmount)
                        {
                            accountA.Withdraw(transferAmount);
                            accountB.Deposit(transferAmount);
                            balanceSnapshots.Add((accountA.Balance, accountB.Balance));
                        }
                    }, timeoutMilliseconds: 60000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // Transfer B -> A
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                    {
                        if (accountB.Balance >= transferAmount)
                        {
                            accountB.Withdraw(transferAmount);
                            accountA.Deposit(transferAmount);
                            balanceSnapshots.Add((accountA.Balance, accountB.Balance));
                        }
                    }, timeoutMilliseconds: 60000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        startSignal.Signal();
        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(120));

        // Assert
        Assert.Empty(exceptions);
        
        // Verify final balance consistency
        var finalBalance = accountA.Balance + accountB.Balance;
        Assert.Equal(2000000m, finalBalance);

        // Verify all snapshots maintained consistency
        foreach (var snapshot in balanceSnapshots)
        {
            var snapshotTotal = snapshot.A + snapshot.B;
            Assert.Equal(2000000m, snapshotTotal);
        }
    }

    [Fact]
    public void SimultaneousBidirectionalTransfers_MixedOperations_NoDeadlocks()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 200000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 200000m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        var operationCount = new ConcurrentBag<string>();

        // Act - Mix of operations: transfers, balance checks, deposits, withdrawals
        var startSignal = new CountdownEvent(1);

        // A -> B transfers
        for (int i = 0; i < 200; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        if (accountA.Balance >= 10m)
                        {
                            accountA.Withdraw(10m);
                            accountB.Deposit(10m);
                            operationCount.Add("A->B");
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // B -> A transfers
        for (int i = 0; i < 200; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                    {
                        if (accountB.Balance >= 10m)
                        {
                            accountB.Withdraw(10m);
                            accountA.Deposit(10m);
                            operationCount.Add("B->A");
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // Balance checks (read-only, but still need locks)
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        var balanceA = accountA.Balance;
                        var balanceB = accountB.Balance;
                        var total = balanceA + balanceB;
                        if (total != 400000m)
                        {
                            throw new InvalidOperationException($"Balance inconsistency detected: {total}");
                        }
                        operationCount.Add("BalanceCheck");
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        startSignal.Signal();
        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(60));

        // Assert
        Assert.Empty(exceptions);
        var finalBalance = accountA.Balance + accountB.Balance;
        Assert.Equal(400000m, finalBalance);
    }

    [Fact]
    public void SimultaneousBidirectionalTransfers_RapidFire_NoDeadlocks()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 50000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 50000m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        const int numberOfTransfers = 2000;
        const decimal transferAmount = 0.1m;
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        var successCount = 0;
        var lockObject = new object();

        // Act - Rapid fire transfers in both directions
        // Start all tasks immediately without any coordination
        Parallel.For(0, numberOfTransfers, i =>
        {
            // A -> B
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        if (accountA.Balance >= transferAmount)
                        {
                            accountA.Withdraw(transferAmount);
                            accountB.Deposit(transferAmount);
                            Interlocked.Increment(ref successCount);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));

            // B -> A
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                    {
                        if (accountB.Balance >= transferAmount)
                        {
                            accountB.Withdraw(transferAmount);
                            accountA.Deposit(transferAmount);
                            Interlocked.Increment(ref successCount);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        });

        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(120));

        // Assert
        Assert.Empty(exceptions);
        var finalBalance = accountA.Balance + accountB.Balance;
        Assert.Equal(100000m, finalBalance);
        Assert.True(successCount > 0, "At least some transfers should have succeeded");
    }

    [Fact]
    public void SimultaneousBidirectionalTransfers_WithInsufficientFunds_HandlesGracefully()
    {
        // Arrange
        var store = new BankAccountStore();
        var accountA = new BankAccount("AccountA", 1000m, "ACC_A");
        var accountB = new BankAccount("AccountB", 1000m, "ACC_B");
        store.TryAdd("ACC_A", accountA);
        store.TryAdd("ACC_B", accountB);

        const int numberOfTransfers = 500;
        const decimal transferAmount = 5m; // Will cause some to fail due to insufficient funds
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        var successfulTransfers = 0;

        // Act - Transfer more than available, some should fail gracefully
        var startSignal = new CountdownEvent(1);

        // A -> B
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_A", "ACC_B", () =>
                    {
                        if (accountA.Balance >= transferAmount)
                        {
                            accountA.Withdraw(transferAmount);
                            accountB.Deposit(transferAmount);
                            Interlocked.Increment(ref successfulTransfers);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        // B -> A
        for (int i = 0; i < numberOfTransfers; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                startSignal.Wait();
                try
                {
                    AccountLockManager.ExecuteWithLocks("ACC_B", "ACC_A", () =>
                    {
                        if (accountB.Balance >= transferAmount)
                        {
                            accountB.Withdraw(transferAmount);
                            accountA.Deposit(transferAmount);
                            Interlocked.Increment(ref successfulTransfers);
                        }
                    }, timeoutMilliseconds: 30000);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        startSignal.Signal();
        Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(60));

        // Assert
        Assert.Empty(exceptions);
        var finalBalance = accountA.Balance + accountB.Balance;
        Assert.Equal(2000m, finalBalance);
        Assert.True(successfulTransfers > 0);
        Assert.True(accountA.Balance >= 0);
        Assert.True(accountB.Balance >= 0);
    }
}

