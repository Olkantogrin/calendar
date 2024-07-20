using System;
using System.Collections.Generic;
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

                string sql = "SELECT * FROM contacts ORDER BY name";

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

        public string[] GetEntryForId(string id)
        {
            string[] contact = new string[5];

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM contacts WHERE id = '{id}' ORDER BY name";
                
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contact[0] = reader["name"].ToString();
                            contact[1] = reader["streetandnumber"].ToString();
                            contact[2] = reader["postalcodeandcity"].ToString();
                            contact[3] = reader["tel"].ToString();
                            contact[4] = reader["mail"].ToString();

                        }



                    }
                }
                connection.Close();
            }

            return contact;
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

        public bool GetLinkedContact(string dateID, string contactID)
        {

            bool boolValue = false;
            string connectionString = "Data Source=cal.db;Version=3;";
            string iscouple = "0";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {


                connection.Open();

                string sql = $"SELECT isCouple FROM couples WHERE id_date = '{dateID}' AND id_contact = '{contactID}' AND iscouple = '1'";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            iscouple = reader["iscouple"].ToString();

                        }



                    }
                }
                connection.Close();
            }


            if (iscouple.Equals("1"))
            {
                boolValue = true;
            }
            else if (iscouple.Equals("0"))
            {
                boolValue = false;
            }

            return boolValue;

        }

        public void UpdateContactForId(string id, string newText, string streetAndNumber, string postalcodeAndCity, string tel, string mail)
        {
            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = "UPDATE contacts SET name = @name, streetandnumber = @san, postalcodeandcity = @poc, tel = @tel, mail = @mail WHERE id = @idd";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", newText);
                    command.Parameters.AddWithValue("@san", streetAndNumber);
                    command.Parameters.AddWithValue("@poc", postalcodeAndCity);
                    command.Parameters.AddWithValue("@tel", tel);
                    command.Parameters.AddWithValue("@mail", mail);
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