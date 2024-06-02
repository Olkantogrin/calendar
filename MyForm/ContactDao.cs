using System;
using System.Data;
using System.Data.SQLite;

namespace MyForm
{
    internal class ContactDao
    {
        public ContactDao()
        {
        }

        public DataSet GetContacts()
        {
            DataSet dataSet = new DataSet();

            string connectionString = "Data Source=cal.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM contacts";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                 
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Erstellen eines DataTable, um die Daten zu speichern
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader); 

                        dataSet.Tables.Add(dataTable);
                    }
                }

                connection.Close();
            }

                return dataSet;
        }

        public void DeleteEntryById(object idVar)
        {
            string id = idVar.ToString();

            string connectionString = "Data Source=cal.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"DELETE FROM contacts WHERE id = @id";
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