using System.Windows.Forms;

namespace MyForm
{
    internal class ContactForm : Form
    {
        private string locale;

        public ContactForm(string locale)
        {
            this.locale = locale;
        }
    }
}