using System;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;



namespace MyForm
{
    public partial class ContactsAddForm : Form
    {
        private string loc = "de-DE";

        TextBox textContact;

        ContactsForm contactForm;

        public ContactsAddForm(ContactsForm contactForm, string locale)
        {
            this.loc = locale;

            this.contactForm = contactForm;

            InitializeContactAddControls();
        }

        private void InitializeContactAddControls()
        {
            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(ContactsAddForm).Assembly);
            CultureInfo ci = new CultureInfo(this.loc);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            

            textContact = new TextBox();
            textContact.Text = "";
            textContact.Location = new Point(10, 10); 
            this.Controls.Add(textContact); 

            Button btnSubmit = new Button();
            btnSubmit.Text = resourceManager.GetString("Submit");  
            btnSubmit.Location = new Point(10, 40); 
            btnSubmit.Click += BtnSubmit_Click; 
            this.Controls.Add(btnSubmit); 

        }

    
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            ContactDao contactDao = new ContactDao();
            contactDao.SaveContact(textContact.Text);

            this.contactForm.InitializeContactControls();

            Close();

        }

   

    }
}
