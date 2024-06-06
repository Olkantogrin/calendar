using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

                  

namespace MyForm //TODO: Ich muss programmieren, dass ich verhindere, dass
                 //die AppointmentForm und die ContactForm sich gleichzietig öffnen lassen und die DatePicker nachbessern.
{    
    public partial class ContactAddForm : Form
    {
        public ContactAddForm(ContactForm contactForm)
        {
            InitializeComponent();
        }
    }
}
