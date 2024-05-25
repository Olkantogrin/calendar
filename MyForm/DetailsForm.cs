﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
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

        public DetailsForm(object id, AppointmentForm appointmentForm)
        {
            this.appointmentForm = appointmentForm;
            this.id = id;
            InitializeComponent();
            InitializeAppointmentControls();
           
        }

        private void InitializeAppointmentControls()
        {


            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(DetailsForm).Assembly);

            //CreateDataGridViewWithReadOnly(id, true);
            //Controls.Add(dataGridView);

            DateDao dateDao = new DateDao();
            Date date = dateDao.GetDateForId(id);

            if (date != null) { 

            string start = date.Start; //24.05.2024 13:03
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


            //Controls.Add(dataGridView);

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*
        private void CreateDataGridViewWithReadOnly(object id, bool isReadOnly)
        {
            dataGridView = new DataGridView();
            dataGridView.Width = 360;
            dataGridView.Location = new Point(10, 20);
            dataGridView.ScrollBars = ScrollBars.Vertical;
            dataGridView.RowHeadersVisible = false;

            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "id";
            column.Name = "id";
            dataGridView.Columns.Add(column);

            dataGridView.Columns["id"].Visible = false;

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "text";
            column.Name = "text";
            dataGridView.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "start";
            column.Name = "start";
            dataGridView.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "end";
            column.Name = "end";
            dataGridView.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "xColumn";
            column.HeaderText = "";
            dataGridView.Columns.Add(column);
            
            dataGridView.AllowUserToAddRows = false;
            dataGridView.ReadOnly = isReadOnly;

            DateDao dateDao = new DateDao();
            //dataGridView.DataSource = dateDao.GetDataSetDatesForId(id);
            dataGridView.DataMember = "dates";

        }
        */

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            

            bool textChanged = textBoxDate.Text != originalText;
            bool startDateChanged = dateTimePickerStart.Value != originalStartDate;
            bool endDateChanged = dateTimePickerEnd.Value != originalEndDate;

            if (textChanged || startDateChanged || endDateChanged) {

                DateDao dateDao = new DateDao();

                dateDao.UpdateDateWithId(id, textBoxDate.Text, dateTimePickerStart.Value, dateTimePickerEnd.Value);

                DataGridView dataGridView = appointmentForm.dataGridView;

                // Durchlaufen der Zeilen in der DataGridView, um die entsprechende Zeile zu finden
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["id"].Value != null && row.Cells["id"].Value.Equals(id))
                    {
                        //TODO Ändern der Hintergrundfarbe der gefundenen Zeile durch Ädern der Zeile ersetzen.
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break; // Keine weitere Suche notwendig, sobald die Zeile gefunden wurde
                    }
                }
                this.Close();

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
