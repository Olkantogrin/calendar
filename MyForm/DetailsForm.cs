using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO: Hier kann man noch eine Update-Logik implementieren über eine beschreibbare DataGridView.

namespace MyForm
{
    public partial class DetailsForm : Form
    {
        private AppointmentForm appointmentForm;
        private object id;
        private Button closeButton;

        public DetailsForm(object id, AppointmentForm appointmentForm)
        {
            InitializeComponent();
            InitializeAppointmentControls();
            this.appointmentForm = appointmentForm;
            this.id = id;

           
        }

        private void InitializeAppointmentControls()
        {
            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(DetailsForm).Assembly);
          
            closeButton = new Button();
            closeButton.Text = resourceManager.GetString("delete");
            closeButton.Location = new System.Drawing.Point(10, 370);
            closeButton.Size = new System.Drawing.Size(100, 50);
            closeButton.Click += CloseButton_Click;
            Controls.Add(closeButton);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {

            // Zugriff auf die DataGridView aus der Hauptform (angenommen, sie heißt "appointmentForm")
            DataGridView dataGridView = appointmentForm.dataGridView;

            // Durchlaufen der Zeilen in der DataGridView, um die entsprechende Zeile zu finden
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["id"].Value != null && row.Cells["id"].Value.Equals(id))
                {
                    DateDao dateDao = new DateDao();
                    dateDao.DeleteEntryById(id);

                    // Löschen der gefundenen Zeile
                    dataGridView.Rows.Remove(row);
                    break; // Keine weitere Suche notwendig, sobald die Zeile gefunden und gelöscht wurde
                }
            }
            this.Close();
        }
    }
}
