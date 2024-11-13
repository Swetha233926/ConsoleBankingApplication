using System;
using System.Collections.Generic;

namespace ConsoleBankingApp
{
    // User class to store user information
    public class User
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }

    // Account class to represent a bank account
    public class Account
    {
        public static int AccountNumberSeed = 1000;
        public int AccountNumber { get; private set; }
        public string HolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

        public Account(string holderName, string accountType, decimal initialDeposit)
        {
            AccountNumber = AccountNumberSeed++;
            HolderName = holderName;
            AccountType = accountType;
            Balance = initialDeposit;
            Transactions.Add(new Transaction("Initial Deposit", initialDeposit));
        }

        // Method to deposit money
        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
                Transactions.Add(new Transaction("Deposit", amount));
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Amount must be positive.");
            }
        }

        // Method to withdraw money with overdraft protection
        public void Withdraw(decimal amount)
        {
            if (amount > 0 && amount <= Balance)
            {
                Balance -= amount;
                Transactions.Add(new Transaction("Withdrawal", amount));
                Console.WriteLine($"Amount {amount} withdrawn successfully.");

            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Invalid amount or insufficient funds.");
            }
        }

        // Method to add monthly interest
        public void AddMonthlyInterest(decimal interestRate)
        {
            if (AccountType.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                decimal interest = Balance * interestRate;
                Balance += interest;
                Transactions.Add(new Transaction("Monthly Interest", interest));
                Console.WriteLine($"Interest of {interest} added to Savings account.");
            }
        }


        // Method to check balance
        public decimal GetBalance() => Balance;
    }

    // Transaction class to log transactions
    public class Transaction
    {
        public DateTime Date { get; }
        public string Type { get; }
        public decimal Amount { get; }

        public Transaction(string type, decimal amount)
        {
            Date = DateTime.Now;
            Type = type;
            Amount = amount;
        }
    }

    // Main Banking Application
    public class BankingApp
    {
        private List<User> users = new List<User>();
        private User loggedInUser = null;

        public void Register(string username, string password)
        {
            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("Username already exists.");
                return;
            }

            // Validate password strength
            while (!ValidatePassword(password))
            {
                Console.Write("Enter a strong password: ");
                password = Console.ReadLine();
            }

            users.Add(new User { Username = username, Password = password });
            Console.WriteLine("Registration successful.");
        }

        private bool ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                Console.WriteLine("Password must be at least 8 characters long.");
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]"))
            {
                Console.WriteLine("Password must contain at least one uppercase letter.");
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[a-z]"))
            {
                Console.WriteLine("Password must contain at least one lowercase letter.");
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[0-9]"))
            {
                Console.WriteLine("Password must contain at least one digit.");
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]"))
            {
                Console.WriteLine("Password must contain at least one special character (e.g., !@#$%^&*).");
                return false;
            }
            return true;
        }


        public void Login(string username, string password)
        {
            loggedInUser = users.Find(u => u.Username == username && u.Password == password);
            if (loggedInUser == null)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid credentials.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Login successful.");
            }
        }

        public void OpenAccount(string holderName)
        {
            if (loggedInUser != null)
            {
                Console.WriteLine("Select Account Type:");
                Console.WriteLine("1. Savings");
                Console.WriteLine("2. Checking");
                Console.WriteLine("3. Business");
                Console.WriteLine("4. Student");
                Console.WriteLine("5. Joint");
                Console.WriteLine("6. Fixed Deposit");
                Console.WriteLine("7. Other");
                Console.Write("Enter choice (1-7): ");

                string accountTypeChoice = Console.ReadLine();
                string accountType;

                switch (accountTypeChoice)
                {
                    case "1":
                        accountType = "Savings";
                        break;
                    case "2":
                        accountType = "Checking";
                        break;
                    case "3":
                        accountType = "Business";
                        break;
                    case "4":
                        accountType = "Student";
                        break;
                    case "5":
                        accountType = "Joint";
                        break;
                    case "6":
                        accountType = "Fixed Deposit";
                        break;
                    case "7":
                        Console.Write("Enter custom account type: ");
                        accountType = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(accountType))
                        {
                            Console.WriteLine("Custom account type cannot be empty.");
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid account type selected.");
                        return;
                }

                Console.Write("Enter Initial Deposit: ");
                decimal initialDeposit = decimal.Parse(Console.ReadLine());
                Account newAccount = new Account(holderName, accountType, initialDeposit);
                loggedInUser.Accounts.Add(newAccount);
                Console.WriteLine();
                Console.WriteLine($"Account created successfully. Account Number: {newAccount.AccountNumber}");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Please log in to open an account.");
            }
        }


        public void ProcessTransaction(int accountNumber, string transactionType, decimal amount)
        {
            Account account = FindAccount(accountNumber);
            if (account != null)
            {
                if (transactionType.ToLower() == "deposit")
                {
                    account.Deposit(amount);
                    if (amount > 0)
                        Console.WriteLine($"Amount {amount} deposited successfully.");
                }
                else if (transactionType.ToLower() == "withdraw")
                {
                    account.Withdraw(amount);
                    //Console.WriteLine($"Amount {amount} withdrawn successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid transaction type.");
                }
            }
            else
            {
                Console.WriteLine("Account not found.");
            }
        }


        public void GenerateStatement(int accountNumber)
        {
            Account account = FindAccount(accountNumber);
            if (account != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Transaction history for Account {account.AccountNumber}");
                foreach (var transaction in account.Transactions)
                {
                    Console.WriteLine($"{transaction.Date} - {transaction.Type}: {transaction.Amount}");
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Account not found.");
            }
        }

        public void CheckBalance(int accountNumber)
        {
            Account account = FindAccount(accountNumber);
            if (account != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Current Balance for Account {account.AccountNumber}: {account.Balance}");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Account not found.");
            }
        }

        private Account FindAccount(int accountNumber)
        {
            return loggedInUser?.Accounts.Find(a => a.AccountNumber == accountNumber);
        }

        public void CalculateInterest(decimal interestRate)
        {
            foreach (var account in loggedInUser.Accounts)
            {
                account.AddMonthlyInterest(interestRate);
            }
            Console.WriteLine();
            Console.WriteLine("Interest calculated for all savings accounts.");
        }

        public void ViewAllAccounts()
        {
            if (loggedInUser != null)
            {
                if (loggedInUser.Accounts.Count > 0)
                {
                    Console.WriteLine("All Accounts:");
                    foreach (var account in loggedInUser.Accounts)
                    {
                        Console.WriteLine($"Account Number: {account.AccountNumber}, Holder Name: {account.HolderName}, Account Type: {account.AccountType}, Balance: {account.Balance}");
                    }
                }
                else
                {
                    Console.WriteLine("No accounts found for this user.");
                }
            }
            else
            {
                Console.WriteLine("Please log in to view accounts.");
            }
        }

    }

    // Program class to run the application
    class Program
    {
        static void Main(string[] args)
        {
            BankingApp app = new BankingApp();
            bool running = true;

            while (running)
            {
                Console.WriteLine("Welcome to Console Banking");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Open Account");
                Console.WriteLine("4. Deposit");
                Console.WriteLine("5. Withdraw");
                Console.WriteLine("6. Generate Statement");
                Console.WriteLine("7. Check Balance");
                Console.WriteLine("8. Calculate Interest");
                Console.WriteLine("9. View All Accounts");
                Console.WriteLine("10. Exit");
                Console.Write("Select an option: ");
                Console.WriteLine();

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.Write("Enter Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Enter Password: ");
                        string password = Console.ReadLine();
                        app.Register(username, password);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "2":
                        Console.Write("Enter Username: ");
                        username = Console.ReadLine();
                        Console.Write("Enter Password: ");
                        password = Console.ReadLine();
                        app.Login(username, password);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "3":
                        Console.Write("Enter Account Holder Name: ");
                        string name = Console.ReadLine();
                        app.OpenAccount(name);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "4":
                        Console.Write("Enter Account Number: ");
                        int accountNumber = int.Parse(Console.ReadLine());
                        Console.Write("Enter Deposit Amount: ");
                        decimal depositAmount = decimal.Parse(Console.ReadLine());
                        app.ProcessTransaction(accountNumber, "deposit", depositAmount);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "5":
                        Console.Write("Enter Account Number: ");
                        accountNumber = int.Parse(Console.ReadLine());
                        Console.Write("Enter Withdrawal Amount: ");
                        decimal withdrawalAmount = decimal.Parse(Console.ReadLine());
                        app.ProcessTransaction(accountNumber, "withdraw", withdrawalAmount);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "6":
                        Console.Write("Enter Account Number: ");
                        accountNumber = int.Parse(Console.ReadLine());
                        app.GenerateStatement(accountNumber);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "7":
                        Console.Write("Enter Account Number: ");
                        accountNumber = int.Parse(Console.ReadLine());
                        app.CheckBalance(accountNumber);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "8":
                        Console.Write("Enter Interest Rate (as decimal): ");
                        decimal interestRate = decimal.Parse(Console.ReadLine());
                        app.CalculateInterest(interestRate);
                        Console.WriteLine("-----------------------------------------------------------");
                        break;
                    case "9":
                        app.ViewAllAccounts();
                        break;
                    case "10":
                        running = false;
                        Console.WriteLine("Exiting application.");
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
