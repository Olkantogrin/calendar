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
    public partial class DetailsForm : Form
    {
        private object id;

        public DetailsForm(object id)
        {
            InitializeComponent();
            this.id = id;
            LoadDetails();
        }

        private void LoadDetails()
        {
            // Laden Sie hier die Details basierend auf der übergebenen id
            // Beispiel:
            // labelId.Text = id.ToString();
            MessageBox.Show($"Die Details für die id {id} werden hier geladen.");
        }
    }
}
