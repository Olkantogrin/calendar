using System.Windows.Forms;

namespace MyForm
{
    public class ContactsEditForm : Form
    {
        private object id;
        private ContactForm contactForm;


        public ContactsEditForm(object id, ContactForm contactForm)
        {
            this.id = id;
            this.contactForm = contactForm;
        }
    }
}