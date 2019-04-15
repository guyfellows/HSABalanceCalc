using System;
using System.IO; //Using for file reading
using System.Linq; //Using for 'Skip' functionality

namespace ContBalanceCalculator
{
    public class User
    {
        private static int _userAge;
        private static int _userActivity;
        private static int _userNutrition;
        private static bool _userMarital;
        private static int _userDependents;
        private static int _userSavings;
        private static int _employerBenefits;
        private static int _currentContributionYear;
        private static int _yearToDateContributions;
        private static int _lifetimeContributions;
        private static int _lifetimeDistributions;
        private static int _currentHSABalance;
        private static int _lifetimeOutOfPocket;
        private static int _singleHSAMaxContribution = 3500;
        private static int _marriedHSAMaxContribution = 7000;

        //Sets initial account balance
        public void SetAccountBalance(int initialBalance)
        {
            _currentHSABalance = initialBalance;
        }
        //Adds monthly contribution amount to balance, checks against YTD limits
        static public void ContributeHSAFunds(int contAmt, string currentContYear)
        {
            if (Convert.ToInt32(currentContYear.Substring(0, 4)) != _currentContributionYear)
            {
                _currentContributionYear = Convert.ToInt32(currentContYear.Substring(0, 4));
                _yearToDateContributions = 0;
            }
            if (_userMarital == true)
            {
                if (_marriedHSAMaxContribution - _yearToDateContributions < contAmt)
                {
                    _currentHSABalance += (_marriedHSAMaxContribution - _yearToDateContributions);
                    _lifetimeContributions += (_marriedHSAMaxContribution - _yearToDateContributions);
                    _yearToDateContributions += (_marriedHSAMaxContribution - _yearToDateContributions);
                }
                else
                {
                    _currentHSABalance += contAmt;
                    _lifetimeContributions += contAmt;
                    _yearToDateContributions += contAmt;
                }
            }
            else
            {
                if (_singleHSAMaxContribution - _yearToDateContributions < contAmt)
                {
                    _currentHSABalance += (_singleHSAMaxContribution - _yearToDateContributions);
                    _lifetimeContributions += (_singleHSAMaxContribution - _yearToDateContributions);
                    _yearToDateContributions += (_singleHSAMaxContribution - _yearToDateContributions);
                }
                else
                {
                    _currentHSABalance += contAmt;
                    _lifetimeContributions += contAmt;
                    _yearToDateContributions += contAmt;
                }
            }

        }
        static public int ApplyMonthlyEvent(int eventAmt, string currentEventYear)
        {
            var outOfPocketCost = 0;
            if (Convert.ToInt32(currentEventYear.Substring(0, 4)) != _currentContributionYear)
            {
                _currentContributionYear = Convert.ToInt32(currentEventYear.Substring(0, 4));
                _yearToDateContributions = 0;
            }
            //Determines OOP costs for negative event
            if (eventAmt < 0)
            {
                var eventAmtAbsolute = Math.Abs(eventAmt);
                if (eventAmtAbsolute > _currentHSABalance)
                {
                    outOfPocketCost = Math.Abs(_currentHSABalance - eventAmtAbsolute);
                    _currentHSABalance = 0;
                    _lifetimeOutOfPocket += outOfPocketCost;
                    _lifetimeDistributions = eventAmtAbsolute - outOfPocketCost;
                    return outOfPocketCost;
                }
                else
                {
                    _currentHSABalance += eventAmt;
                    _lifetimeDistributions += eventAmtAbsolute;
                    return 0;
                }
            }
            //Determines amount to add to balance, compares against YTD limit
            else
            {
                if (_userMarital == true)
                {
                    if (_marriedHSAMaxContribution - _yearToDateContributions > eventAmt)
                    {
                        _currentHSABalance += (_marriedHSAMaxContribution - _yearToDateContributions);
                        _lifetimeContributions += (_marriedHSAMaxContribution - _yearToDateContributions);
                        _yearToDateContributions += (_marriedHSAMaxContribution - _yearToDateContributions);
                    }
                    else
                    {
                        _currentHSABalance += eventAmt;
                        _lifetimeContributions += eventAmt;
                        _yearToDateContributions += eventAmt;
                    }
                    return 0;
                }
                else
                {
                    if (_singleHSAMaxContribution - _yearToDateContributions > eventAmt)
                    {
                        _currentHSABalance += (_singleHSAMaxContribution - _yearToDateContributions);
                        _lifetimeContributions += (_singleHSAMaxContribution - _yearToDateContributions);
                        _yearToDateContributions += (_singleHSAMaxContribution - _yearToDateContributions);
                    }
                    else
                    {
                        _currentHSABalance += eventAmt;
                        _lifetimeContributions += eventAmt;
                        _yearToDateContributions += eventAmt;
                    }
                    return 0;
                }
            }
        }
        static public bool GetValidMemberAge(string userAge)
        {
            bool validAge = Int32.TryParse(userAge, out var x);
            if (validAge && x > 0)
                return true;
            else
                return false;
        }
        static public bool GetValidRange(string userRange)
        {
            bool validRange = Int32.TryParse(userRange, out var userInputRange);
            if (validRange && userInputRange >= 1 && userInputRange <= 5)
                return true;
            else
                return false;
        }
        static public int ValidateMaritalStatus(string userInput)
        {
            var userInputReturn = userInput.ToLower();
            if ((userInputReturn.Equals("n", StringComparison.OrdinalIgnoreCase) || userInputReturn.Equals("no", StringComparison.OrdinalIgnoreCase)))
                return 0;
            else if ((userInputReturn.Equals("y", StringComparison.OrdinalIgnoreCase) || userInputReturn.Equals("yes", StringComparison.OrdinalIgnoreCase)))
                return 1;
            else
                return 3;
        }
        static public bool GetValidMemberDependents(string userDependents)
        {
            bool validDependents = Int32.TryParse(userDependents, out var x);
            if (validDependents && x >= 0)
                return true;
            else
                return false;
        }
        public Tuple<string, int, int, int, string, int, int> EvaluateMonth(int currentMonth)
        {
            var monthlyEventType = 0; //this value tracks the type of event 0 = no event, 1 = event occured without OOP expense, 2 = event occured w/ OOP expense 
            var monthlyOOPAmount = 0;
            Tuple<int, int, int, int, string> monthlyEvent = EventFileReader(currentMonth);
            string theDate = (Convert.ToString(monthlyEvent.Item1) + "." + Convert.ToString(monthlyEvent.Item2));
            var theLikelihood = monthlyEvent.Item3;
            var theValue = monthlyEvent.Item4;
            var theDescription = monthlyEvent.Item5;
            // Determines monthly EE/ER contributions
            var thisMonthEEContribution = EEMonthlyContributionCalc();
            var thisMonthERContribution = ERMonthlyContributionCalc();
            // Adds contributions to HSA balance
            ContributeHSAFunds(thisMonthEEContribution + thisMonthERContribution, theDate);
            // Determines likelihood score
            var thisMonthEventOccurenceRoll = MonthlyEventOccurenceCalc();
            // Determines if vent happens and adjusts balances
            if (thisMonthEventOccurenceRoll >= theLikelihood)
            {
                monthlyOOPAmount = ApplyMonthlyEvent(theValue, theDate);
                monthlyEventType += 1;
                if (monthlyOOPAmount > 0)
                    monthlyEventType += 1;

            }
            return Tuple.Create(theDate, thisMonthEEContribution, thisMonthERContribution, monthlyEventType, theDescription, theValue, monthlyOOPAmount);//string,int,int,int,string,int,int
        }

        //Pulls data from events file
        static public Tuple<int, int, int, int, string> EventFileReader(int eventCounter)
        {
            string line = File.ReadLines("C:\\Users\\mmahony\\Documents\\dev\\HSA Balance Calculator\\eventData\\events.txt").Skip(eventCounter).Take(1).First();
            string[] dataForThisYear = line.Split(',');
            var currentYear = Convert.ToInt32(dataForThisYear[0]);
            var currentMonth = Convert.ToInt32(dataForThisYear[1]);
            var currentLikelihood = Convert.ToInt32(dataForThisYear[2]);
            var currentValue = Convert.ToInt32(dataForThisYear[3]);
            var currentDescirption = (dataForThisYear[4]);
            return Tuple.Create(currentYear, currentMonth, currentLikelihood, currentValue, currentDescirption);
        }
        static public int EEMonthlyContributionCalc()
        {
            var totalEEMonthlyContribution = _userSavings * 150;
            if (_userMarital == true)
                totalEEMonthlyContribution *= 2;
            return totalEEMonthlyContribution;
        }
        static public int ERMonthlyContributionCalc()
        {
            var totalERMonthlyContribution = _employerBenefits * 150;
            if (_userMarital == true)
                totalERMonthlyContribution *= 2;
            return totalERMonthlyContribution;
        }
        static public int MonthlyEventOccurenceCalc()
        {
            var likelihoodScore = new Random().Next(1, 25);
            if (_userMarital == true)
                likelihoodScore += 5;
            likelihoodScore += _userDependents * 3;
            likelihoodScore += 10 - (2 * _userActivity);
            likelihoodScore += 10 - (2 * _userNutrition);
            if (_userAge <= 25)
                likelihoodScore += 5;
            else if (_userAge > 25 && _userAge <= 50)
                likelihoodScore += 10;
            else if (_userAge > 25 && _userAge <= 50)
                likelihoodScore += 10;
            else if (_userAge > 50 && _userAge <= 75)
                likelihoodScore += 15;
            else
                likelihoodScore += 20;
            return likelihoodScore;
        }
        public void GatherUserInfo()
        {
            //Gather user age
            System.Console.WriteLine("Please provide your age");
            while (true)
            {
                var userInputAge = Console.ReadLine();
                var userProvidedAge = GetValidMemberAge(userInputAge);
                if (userProvidedAge == true)
                {
                    _userAge = Convert.ToInt32(userInputAge);
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a valid age");
            }
            //Gather user activity level
            System.Console.WriteLine("Please rate your level of physical activity from 1 to 5 (Indolent sloth to Herculean strong man)");
            while (true)
            {
                var userInputActivity = Console.ReadLine();
                var userProvidedActivity = GetValidRange(userInputActivity);
                if (userProvidedActivity == true)
                {
                   _userActivity = Convert.ToInt32(userProvidedActivity);
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a valid number from 1 to 5");
            }
            //Gather user nutrition level
            System.Console.WriteLine("Please rate your level of nutrition from 1 to 5 (Junk food glutton to macrobiotic guru)");
            while (true)
            {
                var userInputNutrition = Console.ReadLine();
                var userProvidedNutrition = GetValidRange(userInputNutrition);
                if (userProvidedNutrition == true)
                {
                   _userNutrition = Convert.ToInt32(userProvidedNutrition);
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a valid number from 1 to 5");
            }
            //Gather user marital status level
            System.Console.WriteLine("Are you married (Y/N)");
            while (true)
            {
                var userInputMarital = Console.ReadLine();
                var userProvidedMarital = ValidateMaritalStatus(userInputMarital);
                if (userProvidedMarital == 0)
                {
                   _userMarital = false;
                    break;
                }
                if (userProvidedMarital == 1)
                {
                   _userMarital = true;
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a valid marital status (Y/N)");
            }
            //Gather user dependents
            System.Console.WriteLine("How many covered dependents are you responsible for");
            while (true)
            {
                var userInputDependents = Console.ReadLine();
                var userProvidedDependents = GetValidMemberDependents(userInputDependents);
                if (userProvidedDependents == true)
                {
                   _userDependents = Convert.ToInt32(userInputDependents);
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a numeric number of dependents");
            }
            //Gather user savings level
            System.Console.WriteLine("Please rate your dedication to savings from 1 to 5 (Imelda Marcos to Scrooge McDuck)");
            while (true)
            {
                var userInputSavings = Console.ReadLine();
                var userProvidedSavings = GetValidRange(userInputSavings);
                if (userProvidedSavings == true)
                {
                   _userSavings = Convert.ToInt32(userProvidedSavings);
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a valid number from 1 to 5");
            }
            //Gather employer benefits level
            System.Console.WriteLine("Please rate the level of your employer benefits from 1 to 5 (Late 1800s copper mine to post-World War 2 unionized auto manufacturer (I realize these are getting strained, but it's the last one))");
            while (true)
            {
                var userInputBenefits = Console.ReadLine();
                var userProvidedBenefits = GetValidRange(userInputBenefits);
                if (userProvidedBenefits == true)
                {
                   _employerBenefits = Convert.ToInt32(userProvidedBenefits);
                    break;
                }
                else
                    System.Console.WriteLine("Please enter a valid number from 1 to 5");
            }
        }
        public int ReturnLifeTimeContributions()
        {
            return _lifetimeContributions;
        }
        public int ReturnLifetimeDistributions()
        {
            return _lifetimeDistributions;
        }
        public int ReturnLifetimeOOP()
        {
            return _lifetimeOutOfPocket;
        }
        public int ReturnTotalBalance()
        {
            return _currentHSABalance;
        }
        public int ReturnYTDCont()
        {
            return _yearToDateContributions;
        }
    }
}
