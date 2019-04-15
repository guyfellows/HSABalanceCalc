using System;
using System.IO; //Using for file reading
using System.Linq; //Using for 'Skip' functionality

namespace ContBalanceCalculator
{
    class Program
    {
        static public User user1 = new User();
        static public int totalMonthsInReview = 60; //Number of months to review (Default 60)
        static public int initialAccountBalance = 2000;
        static void Main(string[] args)
        {  
            user1.SetAccountBalance(initialAccountBalance);
            System.Console.WriteLine("Welcome to Balance Forecaster. We are going to start by asking you some questions about you (The star of the show).");
            user1.GatherUserInfo();
            System.Console.WriteLine("Now, we will calculate your financial wellbeing for the next several years");
            for (var i = 0; i <= totalMonthsInReview -1; i++)
            {
                Tuple <string,int,int,int,string,int,int> monthlyResults = user1.EvaluateMonth(i); //Returns 1)date, 2)EE cont, 3)ER cont, 4)whether event happened, 5)event description, 6)amount of event, 7)OOPamount)
                System.Console.WriteLine("On {0} you will contribute ${1} and your employer will contribute ${2}", monthlyResults.Item1,monthlyResults.Item2, monthlyResults.Item3);
                if (monthlyResults.Item4 > 0 && monthlyResults.Item6 > 0)
                    {
                        System.Console.WriteLine("Fortune favors you, and this happens: {0}", monthlyResults.Item5);
                        System.Console.WriteLine("You gain an additional ${0}",monthlyResults.Item6);
                    }
                else if (monthlyResults.Item4 > 0 && monthlyResults.Item6 < 0)
                        {
                        System.Console.WriteLine("Unfortunately, luck fails you and this happens: {0}", monthlyResults.Item5);
                        System.Console.WriteLine("The whole ordeal costs you ${0}",Math.Abs(monthlyResults.Item6));
                        if (monthlyResults.Item4 > 1)
                        {  
                            System.Console.WriteLine("You were also responsible for ${0} out of pocket due to account balance.", monthlyResults.Item7);
                        }
                        }
                
            }
            System.Console.WriteLine("At the end of this crazy affair, here is your summary:");
            System.Console.WriteLine("You contributed ${0} to your account.", user1.ReturnLifeTimeContributions());
            System.Console.WriteLine("You spent ${0} from your account.", user1.ReturnLifetimeDistributions());
            System.Console.WriteLine("You spent ${0} in out of pocket expenses.", user1.ReturnLifetimeOOP());
            System.Console.WriteLine("And your total balance was ${0} in your account.", user1.ReturnTotalBalance());
            System.Console.WriteLine("The end");
            Console.ReadLine();
            return;

        }

        }
    }
