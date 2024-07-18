using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyForm
{
    public class DateLocaleConverter
    {
        private static DateLocaleConverter _instance;

        private DateLocaleConverter()
        {
        }

        public static DateLocaleConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DateLocaleConverter();

                return _instance;
            }
        }

        public string ConvertDateAccordingToLocale(string locale, string date)
        {

            string convertedDate = "";

            switch (locale)
            {
                case "en-GB":
                    convertedDate = enGBTodeDE(date);
                    break;
                default:
                    convertedDate = date;
                    break;
            }
            return convertedDate;
        }

        private string enGBTodeDE(string inputDate)
        {
            string convertedDate = "";

            //"German check"...
            string pattern = @"^\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}:\d{2}$";
            bool isValiddeDE = Regex.IsMatch(inputDate, pattern);
            if(isValiddeDE) { return inputDate;  }

            DateTime date = DateTime.ParseExact(inputDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            convertedDate = date.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return convertedDate;

        }
    }
}
