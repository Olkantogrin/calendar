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

        TextBox textBox;
        string txt;

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
            txt = contactDao.GetEntryForId(id.ToString());

            textBox.Text = txt;

            Controls.Add(textBox);

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
            if (!txt.Equals(newText) && updateCheckBox.Checked == true) {
                ContactDao contactDao = new ContactDao();
                contactDao.UpdateContactForId(id.ToString(), newText);
                contactForm.InitializeContactControls();
            }
        }
    }
}