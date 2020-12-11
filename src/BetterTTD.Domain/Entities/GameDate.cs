using System;

namespace BetterTTD.Domain.Entities
{
    public class GameDate
    {
        public int Year  { get; private set; } = 0;
        public int Month { get; private set; } = 0;
        public int Day { get; private set; } = 0;

        public GameDate(long date)
        {
            /* There are 97 leap years in 400 years */
            Year = (int) (400 * Math.Floor(date / (double) (365 * 400 + 97)));
            var rem = (int) (date % (365 * 400 + 97));

            /* There are 24 leap years in 100 years */
            Year += (int) (100 * Math.Floor(rem / (double) (365 * 100 + 24)));
            rem %= 365 * 100 + 24;

            /* There is 1 leap year every 4 years */
            Year += (int) (4 * Math.Floor(rem / (double) (365 * 4 + 1)));
            rem %= 365 * 4 + 1;

            while (rem >= (IsLeapYear(Year) ? 366 : 365)) {
                rem -= IsLeapYear(Year) ? 366 : 365;
                Year++;
            }

            Day = ++rem;
            SetMonthDay();
        }

        public GameDate(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            //TODO: note this -> Month = dateTime.Month + 1;
            Day = dateTime.Day;
        }

        public static bool IsLeapYear (double year)
        {
            return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
        }

        private void SetMonthDay()
        {
            var dateTime = new DateTime(Year).AddDays(Day);
            Month = dateTime.Month;
            Day = dateTime.Day;
        }
        
        public int GetYear ()
        {
            return Year;
        }

        public int GetMonth ()
        {
            return Month;
        }

        public int GetDay ()
        {
            return Day;
        }
    
        public int GetQuarter ()
        {
            return (Month + 2) / 3;
        }

        public GameDate PreviousQuarter()
        {
            var dateTime = new DateTime(Year, Month - 3, DateTime.DaysInMonth(Year, Month -3));
            return new GameDate(dateTime);
        }
        
        public override string ToString()
        {
            return $"{Year}-{Month}-{Day}";
        }
    }
}