using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace MyForm
{
    public class ContactsForm : Form
    {
        private Button addButton;
        private Button exportButton;
        private DataGridView dataGridView;

        private string loc;
        ResourceManager resourceManager;

        public ContactsForm(string locale)
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

        public void InitializeContactControls()
        {

            Controls.Remove(dataGridView);

            addButton = new Button();
            addButton.Text = "+";
            addButton.Location = new System.Drawing.Point(10, 10);
            addButton.Size = new System.Drawing.Size(75, 50);
            addButton.Click += DataGridView_AddClick;
            Controls.Add(addButton);

            exportButton = new Button();
            exportButton.Text = "->";
            exportButton.Location = new System.Drawing.Point(95, 10);
            exportButton.Size = new System.Drawing.Size(75, 50);
            exportButton.Click += DataGridView_ExportClick;
            Controls.Add(exportButton);

            CreateDataGridView();

        }

        private void DataGridView_ExportClick(object sender, EventArgs e)
        {  

            List<Contact> contactsToPrint = new List<Contact>();

            ContactDao contactDao = new ContactDao();
            System.Data.DataSet ds = contactDao.GetContacts();

            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    contactsToPrint.Add(new Contact(row["name"].ToString(), row["streetandnumber"].ToString(), row["postalcodeandcity"].ToString(), row["tel"].ToString(), row["mail"].ToString()));
                }
            }


            writer.Writer wrtr = new writer.Writer();

            wrtr.WriteAllContacts(contactsToPrint);

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

            dataGridView.DataBindingComplete += (sender, e) => HideUnwantedColumns();

            Controls.Add(dataGridView);
        }

        private void HideUnwantedColumns()
        {
            string[] unwantedColumns = { "streetandnumber", "postalcode", "tel", "mail", "postalcodeandcity" };
            foreach (string columnName in unwantedColumns)
            {
                if (dataGridView.Columns[columnName] != null)
                {
                    dataGridView.Columns[columnName].Visible = false;
                }
            }
        }

        private void DataGridView_AddClick(object sender, EventArgs e)
        {
            ContactsAddForm contactsAddForm = new ContactsAddForm(this, this.loc);
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
                    ContactsEditForm contactsEditForm = new ContactsEditForm(id, loc, this);
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