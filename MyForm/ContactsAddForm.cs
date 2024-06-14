using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace MyForm 
{
    public partial class ContactsAddForm : Form
    {
        TextBox textContact;

        public ContactsAddForm(ContactsForm contactForm)
        {
            InitializeContactAddControls();
        }

        private void InitializeContactAddControls()
        {
            textContact = new TextBox();
            textContact.Text = "";
            textContact.Location = new Point(10, 10); // Setze die Position des Textfelds
            this.Controls.Add(textContact); // Füge das Textfeld zur Form hinzu

            Button btnSubmit = new Button();
            btnSubmit.Text = "Submit"; // Setze den Text des Buttons
            btnSubmit.Location = new Point(10, 40); // Setze die Position des Buttons
            btnSubmit.Click += BtnSubmit_Click; // Füge einen Event-Handler für den Klick auf den Button hinzu
            this.Controls.Add(btnSubmit); // Füge den Button zur Form hinzu

        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            ContactDao contactDao = new ContactDao();

            contactDao.SaveContact(textContact.Text);
        }
    }
}
