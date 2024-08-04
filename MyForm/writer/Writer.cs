using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyForm.writer
{
    public class Writer
    {



        internal void WriteAllContacts(List<Contact> contactsToPrint)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string outputPath = saveFileDialog.FileName;

                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        foreach (Contact contact in contactsToPrint)
                        {
                            // Schreibe die relevanten Informationen in die Datei
                            writer.Write($"Name: {contact.Text}");
                            writer.Write(" ");
                            writer.Write($"Adresse: {contact.ContactStreetAndNumber}");
                            writer.Write(" ");
                            writer.Write($"Telefon: {contact.ContactTel}");
                            writer.Write(" ");
                            writer.Write($"E-Mail: {contact.ContactMail}");
                            writer.WriteLine(); // Leerzeile zwischen den Kontakten
                        }
                    }
                }
            }
        }
    }
}
