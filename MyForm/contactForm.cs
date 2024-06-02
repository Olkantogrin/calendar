using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace MyForm //TODO: Feiertage markieren.
{
    public class ContactForm : Form
    {
        private Button addButton;
        private DataGridView dataGridView;

        private string loc;
        ResourceManager resourceManager;

        public ContactForm(string locale)
        {
            this.loc = locale;

            resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            CultureInfo ci = new CultureInfo(this.loc);
            Thread.CurrentThread.CurrentCulture = ci;

            InitializeComponent();
            InitializeContactControls();

        }

        private void InitializeComponent()
        {
            this.Height = 500;
            this.Width = 500;
        }

        private void InitializeContactControls()
        {

            Controls.Remove(dataGridView);

            addButton = new Button();
            addButton.Text = "+";
            addButton.Location = new System.Drawing.Point(10, 10);
            addButton.Size = new System.Drawing.Size(75, 50);
            addButton.Click += DataGridView_AddClick;
            Controls.Add(addButton);

            CreateDataGridView();

        }

        private void CreateDataGridView()
        {
            dataGridView = new DataGridView();
            dataGridView.Location = new System.Drawing.Point(10, 70);
            dataGridView.Size = new System.Drawing.Size(400, 300);

            dataGridView.ScrollBars = ScrollBars.Vertical;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.CellClick += DataGridView_CellClick;

            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "id";
            column.Name = "id";
            dataGridView.Columns.Add(column);

            dataGridView.Columns["id"].Visible = false;

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "name";
            column.Name = "name";
            dataGridView.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "xColumn";
            column.HeaderText = "";
            dataGridView.Columns.Add(column);

            dataGridView.RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridView_RowsAdded);

            ContactDao contactDao = new ContactDao();

            dataGridView.DataSource = contactDao.GetContacts();
            dataGridView.DataMember = "contacts";

            Controls.Add(dataGridView);
        }

        private void DataGridView_AddClick(object sender, EventArgs e)
        {
            ContactAddForm contactsAddForm = new ContactAddForm(this);
            contactsAddForm.ShowDialog(); // Optional: Falls das Hauptfenster blockiert wird, bis die DetailsForm geschlossen wird, verwendet man ShowDialog(), sonst Show().

        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dataGridView.Columns["xColumn"].Index)
                {

                    // Zugriff auf die aktuelle Reihe
                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];

                    var id = row.Cells["id"].Value;

                    ContactDao contactDao = new ContactDao();
                    contactDao.DeleteEntryById(id);

                    InitializeContactControls();
                }
                else
                {

                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
                    var id = row.Cells["id"].Value;

                    ContactsEditForm contactsEditForm = new ContactsEditForm(id, this);
                    contactsEditForm.ShowDialog(); // Optional: Falls das Hauptfenster blockiert wird, bis die DetailsForm geschlossen wird, verwendet man ShowDialog(), sonst Show().


                }
            }

        }

        private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                // Setzen des Wertes "x" für jede neue Zeile in der "xColumn" Spalte
                dataGridView.Rows[i].Cells["xColumn"].Value = "x";
            }
        }
    }
}