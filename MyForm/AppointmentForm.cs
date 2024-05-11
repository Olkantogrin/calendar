using System;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;
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

        private bool datePickerSelected = false;

        private CheckBox checkBoxAddToBoldedDates;
        private TextBox textBoxDate;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;

        private bool isCorrectEntries = false;

        DateTime selectedDateForUpDate;


        string locale = "de-DE";

        public AppointmentForm(DateTime selectedDate, string loc)
        {

            locale = loc;

            selectedDateForUpDate = selectedDate;

            InitializeComponent();
            InitializeAppointmentControls();
            InitializeGridView(selectedDate);

            this.FormClosing += AppointmentForm_FormClosing;

            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            CultureInfo ci = new CultureInfo(locale); 
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            // Erstellen des Kontrollkästchens
            checkBoxAddToBoldedDates = new CheckBox
            {
                Text = resourceManager.GetString("discard"),
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

            dateTimePickerStart.ValueChanged += DatePicker_ValueChanged;
            dateTimePickerEnd.ValueChanged += DatePicker_ValueChanged;

            this.FormClosed += AppointmentForm_FormClosed;


        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            // Überprüfen, ob beide Datepicker ausgewählt wurden
            if (dateTimePickerStart.Value != null && dateTimePickerEnd.Value != null)
            {
                datePickerSelected = true;
            }
            else
            {
                datePickerSelected = false;
            }

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

            if (!datePickerSelected && !checkBoxAddToBoldedDates.Checked)
            {
                MessageBox.Show("Bitte wählen Sie Termine für Anfangs- und Endzeit aus.");
                e.Cancel = true; // Verhindert das Schließen des Formulars
            }
            else { 

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
                        e.Cancel = true;
                    }
            }
            else {
                SetAppointmentDetails(textBoxDate.Text, dateTimePickerStart.Value.TimeOfDay, dateTimePickerEnd.Value.TimeOfDay);
            }
            }
        }

    }
}
