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
            
            // Konvertiere die Strings in DateTime-Objekte mit einem festen Datum (z.B. heute)
            DateTime today = DateTime.Today;
            DateTime time1 = DateTime.ParseExact(time1String, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime time2 = DateTime.ParseExact(time2String, "HH:mm:ss", CultureInfo.InvariantCulture);

            time2 = time2.AddDays(1);
 
            // Berechne die Differenz zwischen den beiden Zeiten
            TimeSpan timeDifference = time2 - time1;

            // Überprüfe, ob die Differenz weniger als 24 Stunden beträgt
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
            // Kulturinfo für das Datumsformat
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

                // Konvertieren von a und b in DateTime-Objekte
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
                    currentDateTime = currentDateTime.AddDays(1); // Verändern Sie diese Zeiteinheit nach Bedarf
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
                    currentDateTime = currentDateTime.AddDays(1); // Verändern Sie diese Zeiteinheit nach Bedarf
                }

                currentDateTime = endDate;
                dateTimes.Add(currentDateTime);


            }

            }


            return dateTimes;
        }


    }
}
