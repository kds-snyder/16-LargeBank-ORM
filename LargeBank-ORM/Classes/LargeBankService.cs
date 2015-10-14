using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeBank_ORM.Classes
{
    public class LargeBankService
    {

        // Display accounts, transactions, and statements for customer 
        //   with input first name & last name
        //  (there may be more than one customer with the same first name and last name) 
        // Return true if customer was found in DB, otherwise return false
        public static bool displayBankData(string custFirstName, string custLastName)
        {
            const string LINESEPARATOR = "--------------------------------------------------------";
            const string CURRENTCULTURE = "en-us"; // for formatting negative currency values

            using (var db = new LargeBankEntities())
            {
                // Get customer data (return if customer was not found)
                var customerList = db.Customers.Where(c => c.FirstName == custFirstName &&
                                                    c.LastName == custLastName);

                if (customerList.Count() == 0)
                {
                    return false;
                }

                // Print data for each customer that was found in the DB
                foreach (var customer in customerList)
                {
                    // Print the customer data 
                    Console.WriteLine(LINESEPARATOR);
                    Console.WriteLine("Customer: {0} {1} joined LargeBank on: {2}",
                        customer.FirstName, customer.LastName, customer.CreatedDate.ToShortDateString());
                    Console.WriteLine("Address: \n" + customer.Address1);
                    if (customer.Address2 != null)
                    {
                        Console.WriteLine(customer.Address2);
                    }
                    Console.Write(customer.City + ", " + customer.State);
                    if (customer.Zip == null)
                    {
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine(" " + customer.Zip);
                    }

                    // Set number format info to use negative sign instead of parentheses 
                    //  for negative numbers in the transaction data
                    var originalCultureInfo = new CultureInfo(CURRENTCULTURE);
                    var modifiedCultureInfo = (CultureInfo)originalCultureInfo.Clone();
                    modifiedCultureInfo.NumberFormat.CurrencyNegativePattern = 1;

                    // Print data for each account belonging to the customer
                    foreach (var account in customer.Accounts)
                    {
                        Console.WriteLine(LINESEPARATOR);
                        Console.WriteLine("--Balance for account# " + account.AccountNumber +
                                " is: " + account.Balance.ToString("C"));
                        
                        // Print the transaction data for the account
                        foreach (var transaction in account.Transactions)
                        {
                            Console.WriteLine("---Transaction date: {0}, amount: {1}",
                                   transaction.TransactionDate.ToShortDateString(),
                                   string.Format(modifiedCultureInfo, "{0:C}", transaction.Amount));
                        }

                        // Print the statement data for the account
                        foreach (var statement in account.Statements)
                        {
                            Console.WriteLine("---Statement start date: {0}, end date: {1}",
                                   statement.StartDate.ToShortDateString(),
                                    statement.EndDate.ToShortDateString());
                            if (statement.CreatedDate != null)
                            {
                                Console.WriteLine("----Created on: ", 
                                    statement.CreatedDate.Value.ToShortDateString());
                            }
                        }
                    }
                    Console.WriteLine(LINESEPARATOR + "\n");
                }
            }
            return true;
        }

        // Add customer to database, using input to fill the record fields
        // Set created date to current date
        // After writing, get the customer ID, and return it
        // If read/write to DB is not successful, throw exception to caller
        public static int addCustomer(string firstName, string lastName, string address1,
                                        string address2, string city, string state, string zip)
        {            
            try
            {
                using (var db = new LargeBankEntities())
                {
                    // Set up the customer record
                    Customer c = new Customer();
                    DateTime curDate = DateTime.Now;
                    c.CreatedDate = curDate;                   
                    c.FirstName = firstName;
                    c.LastName = lastName;
                    c.Address1 = address1;
                    c.Address2 = address2;
                    c.City = city;
                    c.State = state;
                    c.Zip = zip;

                    //Console.WriteLine("Added first name: {0}, last name: {0}, created: {2}",
                    //                    c.FirstName, c.LastName, curDate);

                    // Add the customer and save the changes to the DB
                    db.Customers.Add(c);
                    db.SaveChanges();

                    // Get the customer ID
                    return getCustomerID(c.FirstName, c.LastName, curDate);
                }                
            }

            // Let calling method handle exception if it occurs
            catch (Exception)
            {
                throw;                
            }
        }

        // Return customer ID of customer matching input parameters 
        //  (compare year, month, and day of createdDate)       
        // If customer is not found, set returned ID to 0
        // If read to DB is not successful, throw exception to caller
        public static int getCustomerID(string firstName, string lastName, DateTime createdDate)
        {
            try
            {
                using (var db = new LargeBankEntities())
                {
                    //testGetCustomerID(firstName, lastName, createdDate);
                                       
                    // Get customer data 
                    var customer = db.Customers.Where
                        (c => c.FirstName == firstName && c.LastName == lastName &&
                              c.CreatedDate.Year == createdDate.Year &&
                              c.CreatedDate.Month == createdDate.Month &&
                              c.CreatedDate.Day == createdDate.Day).SingleOrDefault();

                    // Return ID 0 if not found
                    if (customer == null)
                    {
                        return 0;
                    }

                    // Return customer ID
                    return customer.CustomerId;
                }
            }

            // Let calling method handle exception if it occurs
            catch (Exception)
            {
                throw;
            }
        }

        public static void testGetCustomerID(string firstName, string lastName, DateTime createdDate)
        {
            using (var db = new LargeBankEntities())
            {
                var customerList = db.Customers.Where
                         (c => c.FirstName == firstName &&
                                    c.LastName == lastName);
                if (customerList.Count() == 0)
                {
                    Console.WriteLine("Didn't find {0} {1}", firstName, lastName);
                }
                else
                {
                    Console.WriteLine("Found {0} {1}", firstName, lastName);
                    foreach (var cust in customerList)
                    {
                        Console.WriteLine("DB createdate: {0}, input createdate: {1}",
                            cust.CreatedDate, createdDate);
                        Console.WriteLine("Compare DB createdate to input createdate: {0}",
                            DateTime.Compare(cust.CreatedDate, createdDate));
                        Console.WriteLine("Compare TO DB createdate to input createdate: {0}",
                            cust.CreatedDate.CompareTo(createdDate));
                        Console.WriteLine("DB createdate == input createdate: {0}",
                            cust.CreatedDate == createdDate);

                        DateTime date1 = DateTime.Now;
                        DateTime date2 = date1;
                        Console.WriteLine("Compare date1 to date2: {0}",
                            DateTime.Compare(date1, date2));
                        Console.WriteLine("Compare TO date1 to date2: {0}",
                            date1.CompareTo(date2));

                    }
                }
            }
        }

        // Return account ID of account matching input parameters 
        // If account is not found, set returned ID to 0
        // If read to DB is not successful, throw exception to caller
        public static int getAccountID(int custId, string accountNumber)
        {
            try
            {
                using (var db = new LargeBankEntities())
                {                                   
                    // Get customer data 
                    var customer = db.Customers.Where
                        (c => c.CustomerId == custId).SingleOrDefault();
                    // Return ID 0 if not found
                    if (customer == null)
                    {
                        return 0;
                    }

                    // Get account ID matching account number
                    var account = customer.Accounts.Where
                        (a => a.AccountNumber == accountNumber).SingleOrDefault();
                    if (account == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return account.AccountId;
                    }
                    
                }
            }

            // Let calling method handle exception if it occurs
            catch (Exception)
            {
                throw;
            }
        }

        // Add account to database, using input to fill the record fields
        // If write to DB is not successful, throw exception to caller
        public static void addAccount(int CustomerId, string accountNumber, decimal balance)
        {
            try
            {
                using (var db = new LargeBankEntities())
                {
                    // Set up the account record
                    Account acct = new Account();
                    acct.CustomerId = CustomerId;
                    acct.CreatedDate = DateTime.Now;
                    acct.AccountNumber = accountNumber;
                    acct.Balance = balance;                  

                    // Add the customer and save the changes to the DB
                    db.Accounts.Add(acct);
                    db.SaveChanges();
               }
            }

            // Let calling method handle exception if it occurs
            catch (Exception)
            {
                throw;
            }
        }
    }
}
