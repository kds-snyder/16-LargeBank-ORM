using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LargeBank_ORM.Classes;

namespace LargeBank_ORM
{
    class Program
    {
        public static string custFirstName;
        public static string custLastName;

        static void Main(string[] args)
        {            
            try
            {
                while (true)
                {
                    Console.WriteLine("Enter action for LargeBank database: \n" +
                       "A -    Add data\nD -    Display data\n<Enter> - exit");
                    string userChoice = Console.ReadLine();
                    if (userChoice.Length ==0)
                    {
                        break;
                    }
                    switch(userChoice.ToLower())
                    {
                        case "a":
                            addBankData();
                            break;
                        case "d":
                            displayBankData();
                            break;
                        default:
                            break;
                    }
                }               
                   
            }                       
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
                Console.ReadLine();
            }
            //testDB();
            //Console.ReadLine();
        }

        // Add data to tables based on user input
        public static void addBankData()
        {
            // Get input customer data and add to DB
            int customerId = addCustomerData();

            // If added successfully, get input account data
            //  and add to DB
            if (customerId != 0)
            {
                addAccountData(customerId);
                Console.WriteLine("{0} {1} was added successfully to LargeBank\n",
                    custFirstName, custLastName);
            }
        }

        // Display: Prompt for customer name and display bank data until <enter> is pressed
        public static void displayBankData()
        {
            // Get customer name and display bank data until <enter> is pressed
            string customerFirstName, customerLastName;
            while (getName
                    ("Enter the customer first name and last name (press <enter> to exit): ",
                     out customerFirstName, out customerLastName))
            {
                //Console.WriteLine("Customer is: {0} {1}", customerFirstName, customerLastName);

                // Display the bank data                               
                if (!LargeBankService.displayBankData(customerFirstName, customerLastName))
                {
                    Console.WriteLine("{0} {1} is not a customer of LargeBank\n",
                                        customerFirstName, customerLastName);
                }
            }            
        }

        // Prompt with message, and then read input
        // If empty is not accepted, keep prompting until user enters text
        // Return the input text
        // If empty is accepted & user presses <Enter>, return empty string
        public static string getInputString(string message, bool emptyAccepted= false)
        {
            string result = "";
            bool done = false;
            do
            {
                Console.Write(message);
                result = Console.ReadLine();

                // If user pressed <Enter> and
                //  empty input is OK, set done indication
                if (result.Length == 0)
                {
                    if (emptyAccepted)
                    {
                        done = true;
                    }
                }
                // User typed text: set done indication
                else
                {
                    done = true;
                }
            } while (!done);

            return result;
        }

        // Get the customer data input from the user, 
        //  and write it to the DB
        // Return the customer ID
        public static int addCustomerData()
        {
            // Get the input for the customer record
            Console.WriteLine("Enter the customer data:");

            custFirstName = getInputString("Enter the first name: ");
            custLastName = getInputString("Enter the last name: ");
            string customerAddress1 = getInputString("Enter the address: ");
            string customerAddress2 =
                getInputString("Enter additional address information (press <Enter> to omit): ", true);
            if (customerAddress2.Length == 0)
            {
                customerAddress2 = null;
            }
            string customerCity = getInputString("Enter the city: ");
            string customerState = getInputString("Enter the state: ");
            string customerZip = getInputString("Enter zip code (press <Enter> to omit): ", true);
            if (customerZip.Length == 0)
            {
                customerZip = null;
            }

            // Add the customer
            return LargeBankService.addCustomer(custFirstName, custLastName,
                  customerAddress1, customerAddress2, customerCity, customerState, customerZip);
        }

        // Get the account data input from the user, 
        //  and write it to the DB with the input customer ID
        public static void addAccountData(int customerId)
        {
            Console.WriteLine("Enter the account data:");

            string accountNumber =
                getInputString("Enter the account number (press <Enter> to omit adding an account): ", true);
            if (accountNumber.Length > 0)
            {
                decimal accountBalance = getInputPosDecNum("Enter the account balance: $");
                LargeBankService.addAccount(customerId, accountNumber, accountBalance);
            }
        }

        // Output message, then read in a decimal number that must be greater than 0
        // If input is invalid, keep prompting
        // When input is valid, return the decimal number
        static decimal getInputPosDecNum(string message)
        {
            decimal resultNum = 0;
            while (true)
            {
                // Output message, and read user input
                Console.Write(message);
                string input = Console.ReadLine();

                // Attempt to convert input to decimal
                // If result is positive decimal number, return the result,
                //    else output appropriate message
                if (Decimal.TryParse(input, out resultNum))
                {
                    if (resultNum > 0)
                    {
                        return resultNum;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a positive number less than " +
                                    (Decimal.MaxValue).ToString("#,##0"));
                }
            }
        }

        // Prompt with message, and then 
        //  read name that should be first name followed by last name
        // Keep prompting until the name is entered correctly, or
        //  user presses <enter>
        // Return true if first & last name were entered 
        //  (first name is returned in firstName, last name returned in lastName)
        // Return false if <enter> was pressed
        public static bool getName
            (string message, out string firstName, out string lastName)
        {
            string fullName;
            bool gotName = false;
            firstName = "";
            lastName = "";

            // Prompt until user enters text or <enter>
            while (true)
            {
                Console.Write(message);
                fullName = Console.ReadLine();

                // Exit if user pressed <enter>
                if (fullName.Length == 0)
                {
                    break;
                }

                // Break text into first and last name if possible
                if (!getFirstAndLastName(fullName, out firstName, out lastName))
                {
                    Console.WriteLine("Please enter the first name and last name, separated by a space\n");
                }

                // First and last name were entered successfully: 
                //  set indicator and exit
                else
                {
                    gotName = true;
                    break;
                }
            }
            return gotName;
        }

        // Break input name into first and last name, placed into firstName and lastName
        // Return true if successful, otherwise return false
        public static bool getFirstAndLastName
            (string fullName, out string firstName, out string lastName)
        {
            string [] nameParts;
            firstName = "";
            lastName = "";

            // Split the name into words, delimited by blanks
            nameParts = fullName.Split(' ');

            // If split into two words, store them in the output and return true
            //  otherwise return false
            if (nameParts.Length == 2)
            {
                firstName = nameParts[0];
                lastName = nameParts[1];
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static void testDB()
        {
            
            string nullZip;
            nullZip = null;
            string nullAddress2;
            nullAddress2 = null;
            int custID = LargeBankService.addCustomer("Judy", "Kraus", "34 Celeste Drive",
                                    nullAddress2, "Rensselaer", "NY", nullZip);
            Console.WriteLine("Customer ID for new Judy Kraus: {0}", custID);

            custID = LargeBankService.getCustomerID("k", "s", DateTime.Now);
            Console.WriteLine("Customer ID for ks: {0}", custID);
            LargeBankService.addAccount(15, "102A", 245678.98M);
           
        }
    }
}
