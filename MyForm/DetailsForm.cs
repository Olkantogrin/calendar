using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace MyForm
{
    public partial class DetailsForm : Form
    {
        //private DataGridView dataGridView;
        private AppointmentForm appointmentForm;
        private object id;
        private TextBox textBoxDate;
        private Button closeButton;
        private Button updateButton;
        private Button saveButton;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;

        private string originalText;
        private DateTime originalStartDate;
        private DateTime originalEndDate;

        private DataGridView dataGridViewC;

        public DetailsForm(object id, AppointmentForm appointmentForm)
        {

            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.appointmentForm = appointmentForm;
            this.id = id;
            InitializeComponent();
            InitializeAppointmentControls();

            CreateDataGridViewContacts();

        }

        private void CreateDataGridViewContacts()
        {
            dataGridViewC = new DataGridView();
            dataGridViewC.Location = new System.Drawing.Point(380, 70);
            dataGridViewC.Size = new System.Drawing.Size(300, 300);

            dataGridViewC.ScrollBars = ScrollBars.Vertical;
            dataGridViewC.AllowUserToAddRows = false;
            dataGridViewC.ReadOnly = true;
            dataGridViewC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "id";
            column.Name = "id";
            dataGridViewC.Columns.Add(column);

            dataGridViewC.Columns["id"].Visible = false;

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "name";
            column.Name = "name";
            dataGridViewC.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "xColumn";
            column.HeaderText = "";
            dataGridViewC.Columns.Add(column);

            dataGridViewC.RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridViewC_RowsAdded);

            dataGridViewC.DataBindingComplete += (sender, e) => HideUnwantedColumnsAndMarkLinked();

            ContactDao contactDao = new ContactDao();

            dataGridViewC.DataSource = contactDao.GetContacts();
            dataGridViewC.DataMember = "contacts";

            dataGridViewC.CellClick += DataGridViewC_CellClick;

            Controls.Add(dataGridViewC);
        }

        private void HideUnwantedColumnsAndMarkLinked()
        {

            string[] unwantedColumns = { "streetandnumber", "postalcode", "tel", "mail", "postalcodeandcity" };
            foreach (string columnName in unwantedColumns)
            {
                if (dataGridViewC.Columns[columnName] != null)
                {
                    dataGridViewC.Columns[columnName].Visible = false;
                }
            }

            ContactDao contactDao = new ContactDao();
            List<string> coupleIDs = contactDao.GetContactIDsForIDinLinkedCouples(this.id); //TODO: Testen und weiter implementieren.

            MessageBox.Show("Testen und weiter implementieren.");

            foreach (DataGridViewRow row in dataGridViewC.Rows)
            {
                if (row.Cells["id"].Value != null && coupleIDs.Contains(row.Cells["id"].Value.ToString()))
                {
                row.DefaultCellStyle.BackColor = Color.Green;
                }
            }
        }

        private void DataGridViewC_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dataGridViewC.Columns["xColumn"].Index && e.RowIndex >= 0)
            {
                if (dataGridViewC.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "+")
                {
                    ContactDao contactDao = new ContactDao();

                    contactDao.Couple(this.id.ToString(), dataGridViewC.Rows[e.RowIndex].Cells["id"].Value.ToString());
                    

                }
            }
        }

        private void DataGridViewC_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {



            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                // Setzen des Wertes "x" für jede neue Zeile in der "xColumn" Spalte
                dataGridViewC.Rows[i].Cells["xColumn"].Value = "+";



            }

        }


        private void InitializeAppointmentControls()
        {

            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(DetailsForm).Assembly);
            
            //CreateDataGridViewWithReadOnly(id, true);
            //Controls.Add(dataGridView);

            DateDao dateDao = new DateDao();
            Date date = dateDao.GetDateForId(id);

            if (date != null) { 

            string start = date.Start;
            string end = date.End;

                textBoxDate = new TextBox
                {
                    Location = new Point(10, 40),
                    Width = 200
                };

                textBoxDate.Text = date.Text;

            dateTimePickerStart = new DateTimePicker();
            dateTimePickerStart.Format = DateTimePickerFormat.Custom;
            dateTimePickerStart.CustomFormat = "dd.MM.yyyy HH:mm";
            dateTimePickerStart.Location = new Point(10, 150);

            DateTime dateTime;
            if (DateTime.TryParseExact(start, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
             {
                dateTimePickerStart.Value = dateTime;

             }

            dateTimePickerEnd = new DateTimePicker();
            dateTimePickerEnd.Format = DateTimePickerFormat.Custom;
            dateTimePickerEnd.CustomFormat = "dd.MM.yyyy HH:mm";
            dateTimePickerEnd.Location = new Point(10, 180);

            if (DateTime.TryParseExact(end, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
             {
                    dateTimePickerEnd.Value = dateTime;

             }

            }

            originalText = textBoxDate.Text;
            originalStartDate = dateTimePickerStart.Value;
            originalEndDate = dateTimePickerEnd.Value;

            Controls.Add(textBoxDate);
            Controls.Add(dateTimePickerStart);
            Controls.Add(dateTimePickerEnd);

            closeButton = new Button();
            closeButton.Text = resourceManager.GetString("delete");
            closeButton.Location = new System.Drawing.Point(10, 370);
            closeButton.Size = new System.Drawing.Size(100, 50);
            closeButton.Click += CloseButton_Click;
            Controls.Add(closeButton);
            
            updateButton = new Button();
            updateButton.Text = resourceManager.GetString("update");
            updateButton.Location = new System.Drawing.Point(120, 370);
            updateButton.Size = new System.Drawing.Size(100, 50);
            updateButton.Click += UpdateButton_Click;
            Controls.Add(updateButton);

            saveButton = new Button();
            saveButton.Text = resourceManager.GetString("close");
            saveButton.Location = new System.Drawing.Point(230, 370);
            saveButton.Size = new System.Drawing.Size(100, 50);
            saveButton.Click += new EventHandler(SaveButton_Click);
            Controls.Add(saveButton);

            
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(DetailsForm).Assembly);
            
            bool textChanged = textBoxDate.Text != originalText;
            bool startDateChanged = dateTimePickerStart.Value != originalStartDate;
            bool endDateChanged = dateTimePickerEnd.Value != originalEndDate;

            if (textChanged || startDateChanged || endDateChanged) {

                Entry entry = Entry.Instance;
                bool isCorrectEntries = entry.isCorrect(dateTimePickerStart, dateTimePickerEnd);

                if (isCorrectEntries)
                {

                    DateDao dateDao = new DateDao();

                    dateDao.UpdateDateWithId(id, textBoxDate.Text, dateTimePickerStart.Value, dateTimePickerEnd.Value);

                    DataGridView dataGridView = appointmentForm.dataGridView;

                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if (row.Cells["id"].Value != null && row.Cells["id"].Value.Equals(id))
                        {
                            row.DefaultCellStyle.BackColor = Color.Red;
                            row.Cells["text"].Value = textBoxDate.Text;
                            row.Cells["start"].Value = dateTimePickerStart.Value;
                            row.Cells["end"].Value = dateTimePickerEnd.Value;
                            break; // Keine weitere Suche notwendig, sobald die Zeile gefunden wurde
                        }
                    }

                    this.Close();
                }
                else {

                    MessageBox.Show(resourceManager.GetString("The start date must be before the end date."));

                }


            } else {

                MessageBox.Show("No changes have been made.");
            }

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

                    dataGridView.Rows.Remove(row);
                    break;
                }
            }
            this.Close();
        }
    }
}
