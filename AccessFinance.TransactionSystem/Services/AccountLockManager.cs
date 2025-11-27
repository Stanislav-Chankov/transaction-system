using System.Collections.Concurrent;

namespace AccessFinance.TransactionSystem.Services;

/// <summary>
/// Manages thread-safe locking for bank accounts to prevent deadlocks and race conditions.
/// Ensures that transfers involving the same accounts are serialized while allowing
/// transfers on different accounts to proceed concurrently.
/// Uses Monitor.TryEnter with timeout for production-ready deadlock prevention.
/// </summary>
public sealed class AccountLockManager
{
    private static readonly ConcurrentDictionary<string, object> AccountLocks = new();

    /// <summary>
    /// Gets a unique lock object for the specified account number.
    /// The same lock object is returned for the same account number, ensuring thread-safe access.
    /// </summary>
    /// <param name="accountNumber">The account number to get a lock for.</param>
    /// <returns>A lock object associated with the account number.</returns>
    /// <exception cref="ArgumentNullException">Thrown when accountNumber is null or empty.</exception>
    public static object GetLock(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentNullException(nameof(accountNumber), "Account number cannot be null or empty.");
        }

        return AccountLocks.GetOrAdd(accountNumber, _ => new object());
    }

    /// <summary>
    /// Executes an action while holding locks on two accounts in a consistent order with a specified timeout.
    /// </summary>
    /// <param name="firstAccountNumber">The first account number to lock.</param>
    /// <param name="secondAccountNumber">The second account number to lock.</param>
    /// <param name="action">The action to execute while holding both locks.</param>
    /// <param name="timeoutMilliseconds">Maximum time to wait for lock acquisition in milliseconds.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null or account numbers are empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when timeoutMilliseconds is less than or equal to zero.</exception>
    /// <exception cref="TimeoutException">Thrown when lock acquisition times out.</exception>
    public static void ExecuteWithLocks(string firstAccountNumber, string secondAccountNumber, Action action, int timeoutMilliseconds)
    {
        if (string.IsNullOrWhiteSpace(firstAccountNumber))
        {
            throw new ArgumentNullException(nameof(firstAccountNumber), "First account number cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(secondAccountNumber))
        {
            throw new ArgumentNullException(nameof(secondAccountNumber), "Second account number cannot be null or empty.");
        }

        action = action ?? throw new ArgumentNullException(nameof(action));

        if (timeoutMilliseconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds), "Timeout must be greater than zero.");
        }

        // Order accounts alphabetically to prevent deadlocks
        var orderedAccounts = new[] { firstAccountNumber, secondAccountNumber };
        Array.Sort(orderedAccounts, StringComparer.Ordinal);

        var firstLock = GetLock(orderedAccounts[0]);
        var secondLock = GetLock(orderedAccounts[1]);

        // Use Monitor.TryEnter with timeout for production-ready deadlock prevention
        bool firstLockTaken = false;
        bool secondLockTaken = false;

        try
        {
            // Attempt to acquire first lock with timeout
            if (!Monitor.TryEnter(firstLock, timeoutMilliseconds))
            {
                throw new TimeoutException(
                    $"Failed to acquire lock for account '{orderedAccounts[0]}' within {timeoutMilliseconds}ms. " +
                    "The system may be experiencing high load or a deadlock condition.");
            }

            firstLockTaken = true;

            // Attempt to acquire second lock with timeout
            if (!Monitor.TryEnter(secondLock, timeoutMilliseconds))
            {
                throw new TimeoutException(
                    $"Failed to acquire lock for account '{orderedAccounts[1]}' within {timeoutMilliseconds}ms. " +
                    "The system may be experiencing high load or a deadlock condition.");
            }

            secondLockTaken = true;

            // Execute the action while holding both locks
            action();
        }
        finally
        {
            // Always release locks in reverse order
            if (secondLockTaken)
            {
                Monitor.Exit(secondLock);
            }

            if (firstLockTaken)
            {
                Monitor.Exit(firstLock);
            }
        }
    }
}

