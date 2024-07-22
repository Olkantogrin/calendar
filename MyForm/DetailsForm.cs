using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            if (dataGridViewC != null) {

                Controls.Remove(dataGridViewC);
            }

            

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

            dataGridViewC.CellClick += DataGridViewC_CellClick;
            dataGridViewC.RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridViewC_RowsAdded);

            dataGridViewC.DataBindingComplete += (sender, e) =>
            {
                HideUnwantedColumns();
                
            };

            LoadDataIntoDataGridView();

            Controls.Add(dataGridViewC);
        }

         

        private void HideUnwantedColumns()
        {
            string[] unwantedColumns = { "streetandnumber", "postalcode", "tel", "mail", "postalcodeandcity", "SortKey" };
            foreach (string columnName in unwantedColumns)
            {
                if (dataGridViewC.Columns[columnName] != null)
                {
                    dataGridViewC.Columns[columnName].Visible = false;
                }
            }
        }

        private void LoadDataIntoDataGridView()
        {
            ContactDao contactDao = new ContactDao();

            DataSet contactsDataSet = contactDao.GetContacts();
            if (contactsDataSet.Tables.Count > 0)
            {
                dataGridViewC.DataSource = contactsDataSet.Tables[0];
            }
        }


        
        private void DataGridViewC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ContactDao contactDao = new ContactDao();
            

            if (e.ColumnIndex == dataGridViewC.Columns["xColumn"].Index)
            {


                var cellValue = dataGridViewC.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null && cellValue.ToString() == "+")
                {
                    var idValue = dataGridViewC.Rows[e.RowIndex].Cells["id"].Value;
                    if (idValue != null)
                    {
                        contactDao.ToggleCouple(this.id.ToString(), idValue.ToString());
                    }


                    CreateDataGridViewContacts();

                
                }
            }
        }


        private void DataGridViewC_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

            ContactDao contactDao = new ContactDao();

            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {


                dataGridViewC.Rows[i].Cells["xColumn"].Value = "+";

                string cellValue = dataGridViewC.Rows[i].Cells["id"].Value.ToString();

                bool isLink = contactDao.GetLinkedContact(this.id.ToString(), cellValue);

                if (dataGridViewC.Rows[i].Cells["id"].Value != null && isLink)
                {
                    dataGridViewC.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                    //dataGridViewC.Rows[i].DefaultCellStyle.BackColor = Color.Green;

                }
            }

        }


        private void InitializeAppointmentControls()
        {

            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(DetailsForm).Assembly);

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

                            DateLocaleConverter converter = DateLocaleConverter.Instance;
                            
                            row.Cells["start"].Value = converter.ConvertDateAccordingToLocale(dateDao.GetLocale(), dateTimePickerStart.Value.ToString());
                            row.Cells["end"].Value = converter.ConvertDateAccordingToLocale(dateDao.GetLocale(), dateTimePickerEnd.Value.ToString());
                            break;
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


            DataGridView dataGridView = appointmentForm.dataGridView;

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
