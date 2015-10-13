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
            const string CURRENTCULTURE = "en-us";
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
                }
            }
            return true;
        }
    }
}
