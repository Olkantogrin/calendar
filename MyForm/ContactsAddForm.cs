using System;
using System.Drawing;
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
            textContact.Location = new Point(10, 10); 
            this.Controls.Add(textContact); 

            Button btnSubmit = new Button();
            btnSubmit.Text = "Submit"; 
            btnSubmit.Location = new Point(10, 40); 
            btnSubmit.Click += BtnSubmit_Click; 
            this.Controls.Add(btnSubmit); 

        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            ContactDao contactDao = new ContactDao();

            contactDao.SaveContact(textContact.Text);
        }
    }
}
