﻿using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

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
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader); 

                        dataSet.Tables.Add(dataTable);
                    }
                }

                connection.Close();
            }

                return dataSet;
        }

        public string GetEntryForId(string id)
        {
            string name = "";

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM contacts WHERE id = '{id}'";
                
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name = reader["name"].ToString();

                        }



                    }
                }
                connection.Close();
            }

            return name;
        }

        
            public void SaveContact(Contact contact)
            {
                string connectionString = "Data Source=cal.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open(); 

                    string sql = "INSERT INTO contacts (name, streetandnumber, postalcodeandcity, mail, tel) VALUES (@text, @streetAndNumber, @postalCodeAndCity, @tel, @mail)";   
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                    command.Parameters.AddWithValue("@text", contact.Text);
                    command.Parameters.AddWithValue("@streetAndNumber", contact.ContactStreetAndNumber);
                    command.Parameters.AddWithValue("@postalCodeAndCity", contact.ContactPostalCodeAndCity);
                    command.Parameters.AddWithValue("@tel", contact.ContactTel);
                    command.Parameters.AddWithValue("@mail", contact.ContactMail);



                    command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            
        }

        public void UpdateContactForId(string id, string newText)
        {
            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "UPDATE contacts SET name = @name WHERE id = @idd";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", newText);
                    command.Parameters.AddWithValue("@idd", id);
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