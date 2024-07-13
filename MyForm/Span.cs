using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MyForm
{
    public class Span
    {

        bool GetLessThan24Hours(string time1String, string time2String)
        {
            
            DateTime today = DateTime.Today;
            DateTime time1 = DateTime.ParseExact(time1String, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime time2 = DateTime.ParseExact(time2String, "HH:mm:ss", CultureInfo.InvariantCulture);

            time2 = time2.AddDays(1);
 
            TimeSpan timeDifference = time2 - time1;

            if (timeDifference.TotalHours < 24)
            {
                return true;
            }

            return false;
        }



        public List<DateTime> GetSelectedDateTimesByDateList(List<Date> datelist)
        {
            List<DateTime> dateTimes = new List<DateTime>(); 
            
            foreach (Date d in datelist) {
  
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            DateTime startDate = DateTime.ParseExact(d.Start, "dd.MM.yyyy HH:mm", culture);
            DateTime endDate = DateTime.ParseExact(d.End, "dd.MM.yyyy HH:mm", culture);

            string dateStartTime = Regex.Split(startDate.ToString(), @"\s+")[1];
            string dateEndTime = Regex.Split(endDate.ToString(), @"\s+")[1];
 
            bool isLessThan24Hours = GetLessThan24Hours(dateStartTime, dateEndTime);
            
            if (isLessThan24Hours)
            {
                // Schleife zur Generierung aller Termine zwischen startDate und endDate
                DateTime currentDateTime = startDate;
                while (currentDateTime <= endDate)
                {
                    dateTimes.Add(currentDateTime);
                    currentDateTime = currentDateTime.AddDays(1); 
                }

                currentDateTime = endDate;
                dateTimes.Add(currentDateTime);

            }
            else {

                // Schleife zur Generierung aller Termine zwischen startDate und endDate
                DateTime currentDateTime = startDate;
                while (currentDateTime <= endDate.AddDays(-1))
                {
                    dateTimes.Add(currentDateTime);
                    currentDateTime = currentDateTime.AddDays(1); 
                }

                currentDateTime = endDate;
                dateTimes.Add(currentDateTime);


            }

            }


            return dateTimes;
        }

        public Dictionary<Date, List<DateTime>> GetDisplayDatesToDateList(List<Date> datelist)
        {
            Dictionary<Date, List<DateTime>> res = new Dictionary<Date, List<DateTime>>();
            List<DateTime> dateTimes;

            foreach (Date d in datelist)
            {

                dateTimes = new List<DateTime>();

                CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

                DateTime startDate = DateTime.ParseExact(d.Start, "dd.MM.yyyy HH:mm", culture);
                DateTime endDate = DateTime.ParseExact(d.End, "dd.MM.yyyy HH:mm", culture);

                string dateStartTime = Regex.Split(startDate.ToString(), @"\s+")[1];
                string dateEndTime = Regex.Split(endDate.ToString(), @"\s+")[1];

                bool isLessThan24Hours = GetLessThan24Hours(dateStartTime, dateEndTime);

                if (isLessThan24Hours)
                {
                    DateTime currentDateTime = startDate;
                    while (currentDateTime <= endDate)
                    {
                        dateTimes.Add(currentDateTime);
                        currentDateTime = currentDateTime.AddDays(1);
                    }

                    currentDateTime = endDate;
                    dateTimes.Add(currentDateTime);

                }
                else
                {
                    DateTime currentDateTime = startDate;
                    while (currentDateTime <= endDate.AddDays(-1))
                    {
                        dateTimes.Add(currentDateTime);
                        currentDateTime = currentDateTime.AddDays(1); 
                    }

                    currentDateTime = endDate;
                    dateTimes.Add(currentDateTime);


                }

                res.Add(d, dateTimes);

            }


            return res;
        }
    }
}
