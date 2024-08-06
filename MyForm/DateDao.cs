using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;


namespace MyForm
{
    public class DateDao
    {
        public void SaveAppointment(Date date)
        {
            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "INSERT INTO dates (text, start, end, repeat) VALUES (@text, @start, @end, @repeat)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@text", date.Text);
                    command.Parameters.AddWithValue("@start", date.Start);
                    command.Parameters.AddWithValue("@end", date.End);
                    command.Parameters.AddWithValue("@repeat", date.Repeat);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }


        
        public List<DateTime> GetSelectedDatesForMonthAndYear(int month, int year)
        {
            List<Date> dates = new List<Date>();

            string formattedMonth = month.ToString("D2"); // Ergebnis: "03"
            string formattedYear = year.ToString("D4");

            string monthYear = formattedMonth + "." + formattedYear;

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {

                connection.Open();

                string sql = $"SELECT * FROM dates WHERE((start LIKE '%.{monthYear}%' OR end LIKE '%.{monthYear}%') AND repeat = 'n') OR((start LIKE '%.{formattedMonth}.%') OR(end LIKE '%.{formattedMonth}.%') AND repeat = 'y') OR((start LIKE '%.{formattedYear}%' OR end LIKE '%.{formattedYear}%') AND repeat = 'm') ORDER BY substr(start, 7, 4) || '-' || substr(start, 4, 2) || '-' || substr(start, 1, 2) || ' ' || substr(start, 12, 5)";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string text = reader["text"].ToString();
                            string start = reader["start"].ToString();
                            string end = reader["end"].ToString();
                            string repeat = reader["repeat"].ToString();

                            Date d = new Date(text, start, end, repeat);

                            dates.Add(d);
                        }

                        CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
                        List<Date> newDates = new List<Date>();
                        

                        dates.AddRange(newDates);

                        //TODO: Soll ich oben bei jedem d als Jahr als das aktuelle Jahr setzen? Wenn nach Wochentag wiederholt werden soll (repeat = d), hier noch diese zu dates hinzufügen.

                        dates = SetCorrectDateForRepeatedDates(dates, month, year);

                    }
                }
                connection.Close();
            }

            Span span = new Span();
            List<DateTime> selectedDates = span.GetSelectedDateTimesByDateList(dates);
            return selectedDates;
        }
        



        public Dictionary<Date, List<DateTime>> GetSelectedTextDatesForMonthAndYear(int month, int year)
        {
            List<Date> dates = new List<Date>();


            string formattedMonth = month.ToString("D2");
            string formattedYear = year.ToString("D4");

            string monthYear = formattedMonth + "." + formattedYear;

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM dates WHERE((start LIKE '%.{monthYear}%' OR end LIKE '%.{monthYear}%') AND repeat = 'n') OR((start LIKE '%.{formattedMonth}.%') OR(end LIKE '%.{formattedMonth}.%') AND repeat = 'y') OR((start LIKE '%.{formattedYear}%' OR end LIKE '%.{formattedYear}%') AND repeat = 'm') ORDER BY substr(start, 7, 4) || '-' || substr(start, 4, 2) || '-' || substr(start, 1, 2) || ' ' || substr(start, 12, 5)";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string text = reader["text"].ToString();
                            string start = reader["start"].ToString();
                            string end = reader["end"].ToString();
                            string repeat = reader["repeat"].ToString();

                            Date d = new Date(text, start, end, repeat);

                            dates.Add(d);

                        }

                        //TODO: Soll ich oben bei jedem d als Jahr als das aktuelle Jahr setzen? Wenn nach Wochentag wiederholt werden soll (repeat = d), hier noch diese zu dates hinzufügen.

                        dates = SetCorrectDateForRepeatedDates(dates, month, year);

                        

                    }
                }
                connection.Close();
            }

            Span span = new Span();
            Dictionary<Date, List<DateTime>> selectedDates = span.GetDisplayDatesToDateList(dates);
            return selectedDates;

        }

        private List<Date> SetCorrectDateForRepeatedDates(List<Date> dates, int month, int year)
        {

            List<Date> datesresult = new List<Date>();
            
            foreach (Date d in dates) {

                if ("y".Equals(d.Repeat))
                {
                    GetYearly(year, datesresult, d);
                }

                else if ("m".Equals(d.Repeat))
                {
                    GetMonthly(month, datesresult, d);

                }

                else
                {

                    datesresult.Add(d);

                }
            }

            return datesresult;
        }

        private static void GetYearly(int year, List<Date> datesresult, Date d)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            DateTime parsedDateStart = DateTime.ParseExact(d.Start, "dd.MM.yyyy HH:mm", culture);
            DateTime parsedDateEnd = DateTime.ParseExact(d.End, "dd.MM.yyyy HH:mm", culture);

            DateTime updatedDateStart = new DateTime(year, parsedDateStart.Month, parsedDateStart.Day, parsedDateStart.Hour, parsedDateStart.Minute, parsedDateStart.Second);

            TimeSpan difference = parsedDateEnd - parsedDateStart;

            DateTime updatedDateEnd = updatedDateStart.Add(difference);

            string updatedStart = updatedDateStart.ToString();
            string updatedEnd = updatedDateEnd.ToString();

            if (updatedStart.Length > 3)
            {
                updatedStart = updatedStart.Substring(0, updatedStart.Length - 3);
            }

            if (updatedEnd.Length > 3)
            {
                updatedEnd = updatedEnd.Substring(0, updatedEnd.Length - 3);
            }

            d.Start = updatedStart;
            d.End = updatedEnd;

            datesresult.Add(d);
        }


        private void GetMonthly(int month, List<Date> datesresult, Date d)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            DateTime parsedDateStart = DateTime.ParseExact(d.Start, "dd.MM.yyyy HH:mm", culture);
            DateTime parsedDateEnd = DateTime.ParseExact(d.End, "dd.MM.yyyy HH:mm", culture);

            DateTime updatedDateStart = new DateTime(parsedDateStart.Year, month, AdjustLastDay(month, parsedDateStart.Day, parsedDateStart.Year), parsedDateStart.Hour, parsedDateStart.Minute, parsedDateStart.Second);

            TimeSpan difference = parsedDateEnd - parsedDateStart;

            DateTime updatedDateEnd = updatedDateStart.Add(difference);

            if (updatedDateStart.Month >= parsedDateStart.Month)
            {

                string updatedStart = updatedDateStart.ToString();
                string updatedEnd = updatedDateEnd.ToString();

                if (updatedStart.Length > 3)
                {
                    updatedStart = updatedStart.Substring(0, updatedStart.Length - 3);
                }

                if (updatedEnd.Length > 3)
                {
                    updatedEnd = updatedEnd.Substring(0, updatedEnd.Length - 3);
                }

                d.Start = updatedStart;
                d.End = updatedEnd;

                datesresult.Add(d);
                
            }
        }

        public void UpdateDateWithId(object id, string text, DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            string idd = id.ToString();
            string updatedText = text;
            string start = dateTimeStart.ToString();
            string end = dateTimeEnd.ToString();

            string locale = GetLocale();

            DateLocaleConverter converter = DateLocaleConverter.Instance;
            start = converter.ConvertDateAccordingToLocale(locale, start);
            end = converter.ConvertDateAccordingToLocale(locale, end);

            string pattern = @"(\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}):\d{2}";
            string replacement = "$1";

            start = Regex.Replace(start, pattern, replacement);
            end = Regex.Replace(end, pattern, replacement);

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "UPDATE dates SET text = @updatedText, start = @start, end = @end WHERE id = @idd";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@updatedText", updatedText);
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                    command.Parameters.AddWithValue("@idd", idd);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private int AdjustLastDay(int month, int day, int year)
        {
            int last = 28;

            if (day > 29)
            {

                int lastDay = day;

                switch (month)
                {
                    case 2:
                        lastDay = 28;
                        break;
                    case 4:
                        lastDay = 30;
                        break;
                    case 6:
                        lastDay = 30;
                        break;
                    case 9:
                        lastDay = 30;
                        break;
                    case 11:
                        lastDay = 30;
                        break;
                    default:
                        lastDay = day;
                        break;

                }

                //Wenn year ein Schaltjahr ist:
                bool isLeapYear = DateTime.IsLeapYear(year);

                if (isLeapYear)
                { lastDay = 29; }

                    last = lastDay;
            }

            else {
                last = day; 
            }

            return last;
        }

        public string GetLocale()
        {
            string l = "de-DE"; //App default...

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT locale FROM language";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            l = reader["locale"].ToString();

                        }
                    }
                }

                connection.Close();

                return l;
            }
        }

        public Date GetDateForId(object id)
        {
            Date d = null;
            
            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM dates WHERE id = '{id}'";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string text = reader["text"].ToString();
                            string start = reader["start"].ToString();
                            string end = reader["end"].ToString();
                            string repeat = reader["repeat"].ToString();

                            d = new Date(text, start, end, repeat);

                        }



                    }
                }
                connection.Close();
            }

            return d;
        }

        public void UpdateLocale(string locale)
        {
            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "UPDATE language SET locale = @locale";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@locale", locale);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

        }


        
        public DataSet GetDataSetDatesForSelectedDate(DateTime selectedDate)
        {

            string day = selectedDate.Day.ToString("D2");
            string month = selectedDate.Month.ToString("D2");
            string year = selectedDate.Year.ToString("D4");
            
            string selectedDay = day + "." + month + "." + year;
           
            string connectionString = "Data Source=cal.db;Version=3;";

            DataSet dataSet = new DataSet();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $@"SELECT * FROM dates";  

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@selectedDay", selectedDay); //TODO: Kann das hier weg?

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();

                        dataTable.Load(reader); 

                        dataSet.Tables.Add(dataTable);
                    }
                }
                connection.Close();
            }

            DataSet dataSet2 = new DataSet();

            foreach (DataTable table in dataSet.Tables)
            {
                DataTable newTable = new DataTable(table.TableName);

                foreach (DataColumn column in table.Columns)
                {
                    newTable.Columns.Add(column.ColumnName, column.DataType);
                }

                dataSet2.Tables.Add(newTable);
            }

            foreach (DataTable table in dataSet.Tables)
            {
                DataTable newTable = dataSet2.Tables[table.TableName];

                foreach (DataRow row in table.Rows)
                {
                    DataRow newRow = newTable.NewRow();

                    int c = 0;
                    foreach (DataColumn column in table.Columns)
                    {
                        object item = row[column];
                        
                        newRow[column.ColumnName] = row[column];
                        
                        c++;

                    }
                    
                    if (!IsDataRowEmpty(newRow)) {
                         
                        bool b = IsBetween(newRow[2].ToString(), selectedDay, newRow[3].ToString(), newRow[4].ToString());

                        if (b) {

                            newTable.Rows.Add(newRow);

                        }
                    }
                }
            }

            return dataSet2;
        }

        private bool IsBetween(string start, string selectedDate, string end, string repeat) {

            string pattern = @"^\d{2}\.\d{2}\.\d{4}( \d{2}:\d{2})?$";
            Regex regex = new Regex(pattern);

            bool isStart = regex.IsMatch(start);
            bool isSelectedDate = regex.IsMatch(selectedDate);
            bool isEnd = regex.IsMatch(end);

            if (isStart && isSelectedDate && isEnd) { 

                DateTime dt1 = DateTime.ParseExact(start.Substring(0, 10), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                DateTime dt2 = DateTime.ParseExact(selectedDate.Substring(0, 10), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                DateTime dt3 = DateTime.ParseExact(end.Substring(0, 10), "dd.MM.yyyy", CultureInfo.InvariantCulture);


                if ("n".Equals(repeat)) { 
                  return (dt1 <= dt2 && dt2 <= dt3);
                }

                if ("m".Equals(repeat))
                {
                    bool isInRange = (dt1.Year == dt2.Year && dt2.Year == dt3.Year) &&
                    (dt1.Day <= dt2.Day && dt2.Day <= dt3.Day);
                     
                    return isInRange;
                }

                if ("y".Equals(repeat))
                {
                    bool isInRange = (dt1.Month == dt2.Month && dt2.Month == dt3.Month) &&
                    (dt1.Day <= dt2.Day && dt2.Day <= dt3.Day);

                    return isInRange;
                }

               

                return false;

            }

            return false;
        }

        private bool IsDataRowEmpty(DataRow row)
        {
            foreach (var item in row.ItemArray)
            {
                if (item != null && item != DBNull.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetLocale(string locale)
        {
            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "INSERT INTO language (locale) VALUES (@locale)"; 
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@locale", locale);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

 
        public void DeleteEntryById(object idVar)
        {
            string id = idVar.ToString();

            string connectionString = "Data Source=cal.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"DELETE FROM dates WHERE id = @id";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}