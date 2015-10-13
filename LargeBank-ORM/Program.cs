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
        static void Main(string[] args)
        {
            try
            {
                // Display bank data for customer until <enter> is pressed
                string customerFirstName, customerLastName;
                while (getName
                        ("Enter the customer first name and last name (press <enter> to exit): ",
                         out customerFirstName, out customerLastName))
                {
                    //Console.WriteLine("Customer is: {0} {1}", customerFirstName, customerLastName);

                    // Display the bank data                               
                    if (!LargeBankService.displayBankData(customerFirstName, customerLastName))
                    {
                        Console.WriteLine("{0} {1} is not a customer of LargeBank",
                                            customerFirstName, customerLastName);
                    }

                }
            }                       

            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message); ;
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
                    Console.WriteLine("Please enter the first name and last name, separated by a space");
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
    }
}
