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
    public partial class AppointmentForm : Form
    {
        public string SelectedDate { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public DateTime StartDate
        {
            get { return dateTimePickerStart.Value.Date; }
            set { dateTimePickerStart.Value = value; }
        }
        public DateTime EndDate
        {
            get { return dateTimePickerEnd.Value.Date; }
            set { dateTimePickerEnd.Value = value; }
        }
        public string TextBoxDate
        {
            get { return textBoxDate.Text; }
            set { textBoxDate.Text = value; }
        }

        public bool AddToBoldedDates { get { return checkBoxAddToBoldedDates.Checked; } }
        private CheckBox checkBoxAddToBoldedDates;

        private TextBox textBoxDate;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;

        private bool isCorrectEntries = false;

        DateTime selectedDateForUpDate;

        public AppointmentForm(DateTime selectedDate) 
        {

            selectedDateForUpDate = selectedDate;

            InitializeComponent();
            InitializeAppointmentControls();
            InitializeGridView(selectedDate);

            this.FormClosing += AppointmentForm_FormClosing;

            // Erstellen des Kontrollkästchens
            checkBoxAddToBoldedDates = new CheckBox
            {
                Text = "verwerfen",
                Location = new Point(10, 10)
            };
        Controls.Add(checkBoxAddToBoldedDates);

        }

        private void InitializeGridView(DateTime selectedDate)
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.Width = 360;
            dataGridView.Location = new Point(10, 120);
            dataGridView.ScrollBars = ScrollBars.Vertical;

            dataGridView.CellClick+=DataGridView_CellClick;

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

            DateDao dateDao = new DateDao();

            dataGridView.AllowUserToAddRows = false;
            dataGridView.ReadOnly = true;

            dataGridView.DataSource = dateDao.GetDataSetDates(selectedDate);
            dataGridView.DataMember = "dates";
            
            Controls.Add(dataGridView); 
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                // Zugriff auf die aktuelle Reihe
                DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];

                
                var id = row.Cells["id"].Value;
                //var text = row.Cells["text"].Value;
                //var start = row.Cells["start"].Value;
                //var end = row.Cells["end"].Value;
                
                DateDao dateDao = new DateDao();
                    dateDao.DeleteEntryById(id);
                
                checkBoxAddToBoldedDates.Checked = true;
                Close();  
                }
        }

        private void InitializeAppointmentControls()
        {
            textBoxDate = new TextBox
            {
                Location = new Point(10, 40),
                Width = 200
            };
            Controls.Add(textBoxDate);

            // DateTimePicker für die Anfangsuhrzeit
            dateTimePickerStart = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                CustomFormat = "HH:mm",
                Location = new Point(10, 70),
                Width = 100
            };
            Controls.Add(dateTimePickerStart);

            // DateTimePicker für die Enduhrzeit
            dateTimePickerEnd = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                Location = new Point(120, 70),
                Width = 100
            };
            Controls.Add(dateTimePickerEnd);

            this.FormClosed += AppointmentForm_FormClosed;

        }

        private void AppointmentForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            SetAppointmentDetails(textBoxDate.Text, dateTimePickerStart.Value.TimeOfDay, dateTimePickerEnd.Value.TimeOfDay);
        }

        
        public void SetAppointmentDetails(string selectedDate, TimeSpan startTime, TimeSpan endTime)
        {
            SelectedDate = selectedDate;
            StartTime = startTime;
            EndTime = endTime;
        }

        private void AppointmentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Entry entry = Entry.Instance;
            isCorrectEntries = entry.isCorrect(dateTimePickerStart, dateTimePickerEnd);

            if (!checkBoxAddToBoldedDates.Checked)
            {
                if (isCorrectEntries)
                {
                    SetAppointmentDetails(textBoxDate.Text, dateTimePickerStart.Value.TimeOfDay, dateTimePickerEnd.Value.TimeOfDay);
                }
                else
                {
                    MessageBox.Show("Der Starttermin muss vor dem Endtermin sein.");
                }
            }
            else {
                SetAppointmentDetails(textBoxDate.Text, dateTimePickerStart.Value.TimeOfDay, dateTimePickerEnd.Value.TimeOfDay);
            }
          
        }

    }
}
