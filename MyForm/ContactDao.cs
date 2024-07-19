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

        public List<string> GetContactIDsForIDinLinkedCouples(object id)
        {
            string dateID = id.ToString();

            List<string> contactIDs = new List<string>();

            string connectionString = "Data Source=cal.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM couples WHERE id_date = '{dateID}' AND iscouple = '1'";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contactIDs.Add(reader["id_contact"].ToString());

                        }



                    }
                }
                connection.Close();
            }


            return contactIDs;

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

        public void Couple(string dateID, string contactID)
        {
            string connectionString = "Data Source=cal.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Prüfen, ob ein Eintrag mit dateID oder contactID existiert
                string checkQuery = "SELECT COUNT(*) FROM couples WHERE id_date = @dateID OR id_contact = @contactID";
                SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@dateID", dateID);
                checkCommand.Parameters.AddWithValue("@contactID", contactID);

                int count = int.Parse(checkCommand.ExecuteScalar().ToString());

                if (count == 0)
                {
                    // Wenn kein Eintrag existiert, INSERT durchführen
                    string insertQuery = "INSERT INTO couples (id_date, id_contact, iscouple) VALUES (@dateID, @contactID, '1')";
                    SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@dateID", dateID);
                    insertCommand.Parameters.AddWithValue("@contactID", contactID);
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    // Wenn ein Eintrag existiert, UPDATE durchführen
                    string updateQuery = "UPDATE couples SET iscouple = CASE WHEN iscouple = '1' THEN '0' ELSE '1' END WHERE id_date = @dateID OR id_contact = @contactID";
                    SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@dateID", dateID);
                    updateCommand.Parameters.AddWithValue("@contactID", contactID);
                    updateCommand.ExecuteNonQuery();
                }
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