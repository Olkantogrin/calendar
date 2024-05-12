using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Globalization;

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

                string sql = "INSERT INTO dates (text, start, end) VALUES (@text, @start, @end)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@text", date.Text);
                    command.Parameters.AddWithValue("@start", date.Start);
                    command.Parameters.AddWithValue("@end", date.End);

                    command.ExecuteNonQuery();
                }
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

                string sql = $"SELECT * FROM dates WHERE start LIKE '%.{monthYear}%' OR end LIKE '%.{monthYear}%' ORDER BY substr(start, 7, 4) || '-' || substr(start, 4, 2) || '-' || substr(start, 1, 2)|| ' ' || substr(start, 12, 5)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Hier können Sie auf die Daten zugreifen, z.B.:
                            string text = reader["text"].ToString();
                            string start = reader["start"].ToString();
                            string end = reader["end"].ToString();

                            Date d = new Date(text, start, end);
                            dates.Add(d);

                        }
                    }
                }
            }

            Span span = new Span();
            List<DateTime> selectedDates = span.GetSelectedDateTimesByDateList(dates);
            return selectedDates;
        }

        public Dictionary<Date, List<DateTime>> GetSelectedTextDatesForMonthAndYear(int month, int year)
        {
            List<Date> dates = new List<Date>();

            string formattedMonth = month.ToString("D2"); // Ergebnis: "03"
            string formattedYear = year.ToString("D4");

            string monthYear = formattedMonth + "." + formattedYear;

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM dates WHERE start LIKE '%.{monthYear}%' OR end LIKE '%.{monthYear}%' ORDER BY substr(start, 7, 4) || '-' || substr(start, 4, 2) || '-' || substr(start, 1, 2)|| ' ' || substr(start, 12, 5)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Hier können Sie auf die Daten zugreifen, z.B.:
                            string text = reader["text"].ToString();
                            string start = reader["start"].ToString();
                            string end = reader["end"].ToString();

                            Date d = new Date(text, start, end);
                            dates.Add(d);

                        }
                    }
                }
            }

            Span span = new Span();
            Dictionary<Date, List<DateTime>> selectedDates = span.GetDisplayDatesToDateList(dates);
            return selectedDates;
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

                return l;
            }
        }

        public DataSet GetDataSetDates(DateTime selectedDate)
        {

            string day = selectedDate.Day.ToString("D2");
            string month = selectedDate.Month.ToString("D2");
            string year = selectedDate.Year.ToString("D4");

            string selectedDay = day + "." + month + "." + year + " 22:47";
           
            string connectionString = "Data Source=cal.db;Version=3;";

            DataSet dataSet = new DataSet();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                //string sql = $"SELECT * FROM dates WHERE '{selectedDay}' BETWEEN start AND end";
                string sql = $"SELECT * FROM dates WHERE CAST('{selectedDay}' AS DATE) BETWEEN CAST(start AS DATE) AND CAST(end AS DATE)";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@selectedDay", selectedDay);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Erstellen eines DataTable, um die Daten zu speichern
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader); // Laden Sie die Daten direkt aus dem Reader in das DataTable

                        dataSet.Tables.Add(dataTable);
                    }
                }
            }



            return dataSet;
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
            }

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
            }
        }

        public string GetLocale(string locale)
        {
            throw new NotImplementedException();
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
            }
        }
    }
}