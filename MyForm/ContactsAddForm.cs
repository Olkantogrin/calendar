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

        Label labelTextContact; Label labelStreetAndNumber; Label labelPostalCodeAndCity;
        TextBox textContact; TextBox textStreetAndNumber; TextBox textPostalCodeAndCity;
        TextBox textTel; TextBox textMail;

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
            
            labelTextContact = new Label();
            labelTextContact.Text = resourceManager.GetString("contact");
            labelTextContact.Location = new Point(10, 10);
            this.Controls.Add(labelTextContact);
            
            textContact = new TextBox();
            textContact.Location = new Point(10, 36);
            textContact.Width = 250;
            this.Controls.Add(textContact);

            labelStreetAndNumber = new Label();
            labelStreetAndNumber.Text = resourceManager.GetString("Street and house number");
            labelStreetAndNumber.Location = new Point(10, 64);
            labelStreetAndNumber.Width = 250;
            this.Controls.Add(labelStreetAndNumber);

            textStreetAndNumber = new TextBox();
            textStreetAndNumber.Text = "Noch kein DB-Anschluss.";
            textStreetAndNumber.Location = new Point(10, 90);
            textStreetAndNumber.Width = 250;
            this.Controls.Add(textStreetAndNumber);

            labelPostalCodeAndCity = new Label();
            labelPostalCodeAndCity.Text = resourceManager.GetString("Town and postal code");
            labelPostalCodeAndCity.Location = new Point(10, 120);
            labelPostalCodeAndCity.Width = 250;
            this.Controls.Add(labelPostalCodeAndCity);

            textPostalCodeAndCity = new TextBox();
            textPostalCodeAndCity.Text = "Noch kein DB-Anschluss.";
            textPostalCodeAndCity.Location = new Point(10, 146);
            textPostalCodeAndCity.Width = 250;
            this.Controls.Add(textPostalCodeAndCity);


            ///////////////////////////

            textTel = new TextBox();
            textTel.Text = "Noch kein DB-Anschluss."; //resourceManager.GetString("phone");
            textTel.Location = new Point(10, 172);
            textTel.Width = 250;
            this.Controls.Add(textTel);


            textMail = new TextBox();
            textMail.Text = "Noch kein DB-Anschluss."; //resourceManager.GetString("mail");
            textMail.Location = new Point(10, 196);
            textMail.Width = 250;
            this.Controls.Add(textMail);

            ///////////////////////////

            Button btnSubmit = new Button();
            btnSubmit.Text = resourceManager.GetString("Submit");  
            btnSubmit.Location = new Point(10, 220); 
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
