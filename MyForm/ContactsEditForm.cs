using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace MyForm
{
    public class ContactsEditForm : Form
    {
        private object id;
        private string loc;
        private CheckBox updateCheckBox;
        private ContactsForm contactForm; 

        TextBox textBox; TextBox streetAndNumberBox; TextBox postalcodeAndCityBox; TextBox telBox; TextBox mailBox;
        string txt;
        string streetandnumber;
        string postalcodeandcity;
        string tel;
        string mail;

        public ContactsEditForm(object id, string loc, ContactsForm contactForm)
        {
            this.id = id;
            this.loc = loc;
            this.contactForm = contactForm;

            InitializeContactControls(id, contactForm);

            this.FormClosed += new FormClosedEventHandler(MyForm_FormClosed);

        }

        private void InitializeContactControls(object id, ContactsForm contactForm)
        {
            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            CultureInfo ci = new CultureInfo(loc);
            Thread.CurrentThread.CurrentCulture = ci;


            textBox = new TextBox();

            textBox.Location = new Point(10, 10); 
            textBox.Size = new Size(200, 20); 

            ContactDao contactDao = new ContactDao();
            txt = contactDao.GetEntryForId(id.ToString())[0];

            streetandnumber = contactDao.GetEntryForId(id.ToString())[1];

            postalcodeandcity = contactDao.GetEntryForId(id.ToString())[2];

            tel = contactDao.GetEntryForId(id.ToString())[3];

            mail = contactDao.GetEntryForId(id.ToString())[4];

            textBox.Text = txt;

            Controls.Add(textBox);


            streetAndNumberBox = new TextBox(); postalcodeAndCityBox = new TextBox(); telBox = new TextBox(); mailBox = new TextBox();

            streetAndNumberBox.Location = new Point(10, 70);
            streetAndNumberBox.Size = new Size(200, 20);
            streetAndNumberBox.Text = streetandnumber;

            postalcodeAndCityBox.Location = new Point(10, 100);
            postalcodeAndCityBox.Size = new Size(200, 20);
            postalcodeAndCityBox.Text = postalcodeandcity;

            telBox.Location = new Point(10, 130);
            telBox.Size = new Size(200, 20);
            telBox.Text = tel;

            mailBox.Location = new Point(10, 160);
            mailBox.Size = new Size(200, 20);
            mailBox.Text = mail;
            
            Controls.Add(streetAndNumberBox); Controls.Add(postalcodeAndCityBox); Controls.Add(telBox); Controls.Add(mailBox);

            updateCheckBox = new CheckBox
            {
                Location = new Point(10, 40), 
                Size = new Size(200, 20),
                Text = resourceManager.GetString("update")
            };

            Controls.Add(updateCheckBox);
        }

        private void MyForm_FormClosed(object sender, FormClosedEventArgs e)
        {   string newText = textBox.Text;

            string streetAndNumber = streetAndNumberBox.Text;
            string postalcodeAndCity = postalcodeAndCityBox.Text;
            string tel = telBox.Text;
            string mail = mailBox.Text;

            if ((!txt.Equals(newText) && updateCheckBox.Checked == true)
            || (!streetAndNumber.Equals(newText) && updateCheckBox.Checked == true)
            || (!txt.Equals(postalcodeAndCity) && updateCheckBox.Checked == true)
            || (!txt.Equals(tel) && updateCheckBox.Checked == true)
            || (!txt.Equals(mail) && updateCheckBox.Checked == true))
            {
                ContactDao contactDao = new ContactDao();
                contactDao.UpdateContactForId(id.ToString(), newText, streetAndNumber, postalcodeAndCity, tel, mail);
                contactForm.InitializeContactControls();
            }
        }
    }
}