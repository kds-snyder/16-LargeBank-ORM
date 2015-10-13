using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeBank_ORM.Classes
{
    public class LargeBankService
    {

        // Display accounts, transactions, and statements for input customer
        // Return true if customer was found in DB, otherwise return false
        public static bool displayBankData(string custFirstName, string custLastName)
        {
            const string lineSeparator = "--------------------------";
            using (var db = new LargeBankEntities())
            {
                // Get customer data (return if customer was not found)
                var customer = db.Customers.Where(c => c.FirstName == custFirstName &&
                                                    c.LastName == custLastName).FirstOrDefault();               
                if (customer == null)
                {
                    return false;
                }

                // Print the customer data
                Console.WriteLine(lineSeparator);
                Console.WriteLine("Customer: {0} {1} joined LargeBank on : {2}", 
                    custFirstName, custLastName, customer.CreatedDate);
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

                // Print the accounts for this customer
                foreach (var account in customer.Accounts)
                {
                    Console.WriteLine(lineSeparator);
                    Console.WriteLine("--Balance for account# " + account.AccountNumber +
                            " is: " + account.Balance.ToString("C"));

                    // Print the transaction data for the account
                    foreach (var transaction in account.Transactions)
                    {
                        Console.WriteLine("---Transaction date: {0}, amount: {1}",
                               transaction.TransactionDate,
                                transaction.Amount.ToString("C"));
                    }

                    // Print the statement data for the account
                    foreach (var statement in account.Statements)
                    {
                        Console.WriteLine("---Statement start date: {0}, end date: {1}",
                               statement.StartDate,
                                statement.EndDate);
                        if (statement.CreatedDate != null)
                        {
                            Console.WriteLine("----Created on: ", statement.CreatedDate);
                        }
                    }
                }                   
            }
              return true;
        }
    }
}
