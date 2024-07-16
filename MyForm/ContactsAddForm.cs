using System;
using System.Drawing;
using System.Globalization;
using System.Net.Mail;
using System.Resources;
using System.Text.RegularExpressions;
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
            textStreetAndNumber.Location = new Point(10, 90);
            textStreetAndNumber.Width = 250;
            this.Controls.Add(textStreetAndNumber);

            labelPostalCodeAndCity = new Label();
            labelPostalCodeAndCity.Text = resourceManager.GetString("Town and postal code");
            labelPostalCodeAndCity.Location = new Point(10, 120);
            labelPostalCodeAndCity.Width = 250;
            this.Controls.Add(labelPostalCodeAndCity);

            textPostalCodeAndCity = new TextBox();
            textPostalCodeAndCity.Location = new Point(10, 146);
            textPostalCodeAndCity.Width = 250;
            this.Controls.Add(textPostalCodeAndCity);

            textMail = new TextBox();
            textMail.Text = "mail@mail.com"; //resourceManager.GetString("phone");
            textMail.Location = new Point(10, 172);
            textMail.Width = 250;
            this.Controls.Add(textMail);


            textTel = new TextBox();
            textTel.Text = "11123456789"; //resourceManager.GetString("mail");
            textTel.Location = new Point(10, 196);
            textTel.Width = 250;
            this.Controls.Add(textTel);

            Button btnSubmit = new Button();
            btnSubmit.Text = resourceManager.GetString("Submit");  
            btnSubmit.Location = new Point(10, 220); 
            btnSubmit.Click += BtnSubmit_Click; 
            this.Controls.Add(btnSubmit); 

        }

    
        private void BtnSubmit_Click(object sender, EventArgs e)
        {

            //ALTER TABLE contacts ADD streetandnumber TEXT, postalcodeandcity TEXT, mail TEXT, tel TEXT; und dann

            string contactText = textContact.Text;
            string contactStreetAndNumber = textStreetAndNumber.Text;
            string contactPostalCodeAndCity = textPostalCodeAndCity.Text;        
            string contactTel = textTel.Text;
            string contactMail = textMail.Text;

            bool isClose = true;
            bool isValidTel = IsValidTel(contactTel);
            bool isValidMail = IsValidMail(contactMail);

          


            if (!isValidTel) {
                MessageBox.Show("Not a valid phone number.");
                isClose = false;
                textTel.Text = "";

            }

            if (!isValidMail)
            {
                MessageBox.Show("Not a valid e-mail.");
                isClose = false;
                textMail.Text = "";
            }

            if(isClose) {

                if (contactMail.Equals("mail@mail.com")) { contactMail = "";  }

                if (contactTel.Equals("11123456789")) { contactTel = ""; }



            Contact contact = new Contact(contactText, contactStreetAndNumber,
            contactPostalCodeAndCity,
            contactTel,
            contactMail);
             
            ContactDao contactDao = new ContactDao();
            contactDao.SaveContact(contact);
            
            Close();

            }

            this.contactForm.InitializeContactControls();
        }

        private bool IsValidMail(string email) //TODO: Implementieren.
        {
                return true;
        }

        private bool IsValidTel(string tel) //TODO: Implementieren.
        {
            return true;
        }
    }
}
