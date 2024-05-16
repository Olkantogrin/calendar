using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyForm.reader
{
    public class SpecificReaderProductICS : IProdukt
    {
        public Date Operation(string content)
        {

            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string text = ""; string start = ""; string end = "";
            foreach (string line in lines)
            {
                string s;
                if (TryReadLine(line, "SUMMARY:", out s))
                    text = s;
                if (TryReadLine(line, "DTSTART:", out s))
                    start = s;
                if (TryReadLine(line, "DTEND:", out s))
                    end = s;
            }

            //string text = lines[8]; //TEXT 
            //string start = lines[18].Split(':')[1]; //START
            //string end = lines[20].Split(':')[1]; //END


            if ((!IsZuluTime(start) || !IsZuluTime(end)))
            {
                MessageBox.Show("Reader Exception");
                throw new MeineException("Reader Exception");

            }
            else {

                /*
                int index = text.IndexOf("SUMMARY:");
                if (index != -1)
                {
                    text = text.Substring(index + "SUMMARY:".Length);
                }
                */

                DateTime dateTimeStart = DateTime.ParseExact(start, "yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                //string formattedStringStart = dateTimeStart.ToString("dd.MM.yyyy HH:mm");

                DateTime dateTimeEnd = DateTime.ParseExact(end, "yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                //string formattedStringEnd = dateTimeEnd.ToString("dd.MM.yyyy HH:mm");

                DateTimeConverter dtConverter = new DateTimeConverter();

                DateTime dateTimeStartMez = dtConverter.ConvertZuluToMezOrMeSz(dateTimeStart);

                string formattedDateTimeStartMez = dateTimeStartMez.ToString("dd.MM.yyyy HH:mm");

                DateTime dateTimeEndMez = dtConverter.ConvertZuluToMezOrMeSz(dateTimeEnd);

                string formattedDateTimeEndMez = dateTimeEndMez.ToString("dd.MM.yyyy HH:mm");


                Date date = new Date(text, formattedDateTimeStartMez, formattedDateTimeEndMez);

                return date;
            }
        }

        bool TryReadLine(string line, string keyword, out string value)
        {
            value = "";
            if (line.StartsWith(keyword))
            {
                value = line.Substring(keyword.Length);
                return true;
            }

            return false;
        }

        private bool IsZuluTime(string input)
        {
            Regex zuluTimeRegex = new Regex(@"^\d{4}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])T([01]\d|2[0-3])[0-5]\d[0-5]\dZ$");

            return zuluTimeRegex.IsMatch(input);
        }
    }
}