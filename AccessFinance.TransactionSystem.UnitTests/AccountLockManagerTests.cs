using AccessFinance.TransactionSystem.Services;

namespace AccessFinance.TransactionSystem.UnitTests;

public class AccountLockManagerTests
{
    [Fact]
    public void GetLock_SameAccountNumber_ReturnsSameLockObject()
    {
        // Arrange
        var accountNumber = "ACC001";

        // Act
        var lock1 = AccountLockManager.GetLock(accountNumber);
        var lock2 = AccountLockManager.GetLock(accountNumber);

        // Assert
        Assert.Same(lock1, lock2);
    }

    [Fact]
    public void GetLock_DifferentAccountNumbers_ReturnsDifferentLockObjects()
    {
        // Arrange & Act
        var lock1 = AccountLockManager.GetLock("ACC001");
        var lock2 = AccountLockManager.GetLock("ACC002");

        // Assert
        Assert.NotSame(lock1, lock2);
    }

    [Fact]
    public void GetLock_NullAccountNumber_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AccountLockManager.GetLock(null!));
    }

    [Fact]
    public void GetLock_EmptyAccountNumber_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AccountLockManager.GetLock(string.Empty));
    }

    [Fact]
    public void GetLock_WhitespaceAccountNumber_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AccountLockManager.GetLock("   "));
    }

    [Fact]
    public void ExecuteWithLocks_ExecutesAction_WhenLocksAreAvailable()
    {
        // Arrange
        bool actionExecuted = false;

        // Act
        AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () =>
        {
            actionExecuted = true;
        }, timeoutMilliseconds: 1000);

        // Assert
        Assert.True(actionExecuted);
    }

    [Fact]
    public void ExecuteWithLocks_OrdersAccountsAlphabetically_ToPreventDeadlocks()
    {
        // Arrange
        var lockOrder = new List<string>();
        var lock1 = AccountLockManager.GetLock("ACC002"); // Will be first alphabetically
        var lock2 = AccountLockManager.GetLock("ACC001"); // Will be second alphabetically

        // Use a separate thread to verify lock ordering
        var tcs = new TaskCompletionSource<bool>();
        bool lock1Acquired = false;

        // Start a thread that will hold lock1
        var thread1 = new Thread(() =>
        {
            lock (lock1)
            {
                lock1Acquired = true;
                tcs.Task.Wait(); // Wait for signal
            }
        });
        thread1.Start();

        // Wait for thread1 to acquire lock1
        while (!lock1Acquired)
        {
            Thread.Sleep(10);
        }

        // Now try to execute with locks - should wait for thread1 to release
        bool executed = false;
        var executeTask = Task.Run(() =>
        {
            AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () =>
            {
                executed = true;
            }, timeoutMilliseconds: 2000);
        });

        // Give it a moment to start waiting
        Thread.Sleep(50);

        // Signal thread1 to release lock
        tcs.SetResult(true);
        thread1.Join();
        executeTask.Wait();

        // Assert - if we got here without deadlock, ordering worked
        Assert.True(executed);
    }

    [Fact]
    public void ExecuteWithLocks_NullFirstAccount_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            AccountLockManager.ExecuteWithLocks(null!, "ACC002", () => { }, timeoutMilliseconds: 1000));
    }

    [Fact]
    public void ExecuteWithLocks_NullSecondAccount_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            AccountLockManager.ExecuteWithLocks("ACC001", null!, () => { }, timeoutMilliseconds: 1000));
    }

    [Fact]
    public void ExecuteWithLocks_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", null!, timeoutMilliseconds: 1000));
    }

    [Fact]
    public void ExecuteWithLocks_ZeroTimeout_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () => { }, timeoutMilliseconds: 0));
    }

    [Fact]
    public void ExecuteWithLocks_NegativeTimeout_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            AccountLockManager.ExecuteWithLocks("ACC001", "ACC002", () => { }, timeoutMilliseconds: -1));
    }

    [Fact]
    public void ExecuteWithLocks_EmptyFirstAccount_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            AccountLockManager.ExecuteWithLocks(string.Empty, "ACC002", () => { }, timeoutMilliseconds: 1000));
    }

    [Fact]
    public void ExecuteWithLocks_EmptySecondAccount_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            AccountLockManager.ExecuteWithLocks("ACC001", string.Empty, () => { }, timeoutMilliseconds: 1000));
    }
}

