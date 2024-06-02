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
        public int SelectedRepetitionIndex { get; private set; }
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
        private ComboBox comboBox;
        private CheckBox wholeDay;
        private TextBox textBoxDate;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;
        private Button closeButton;
        private Button saveButton;

        public DataGridView dataGridView;
        private DataGridView dataGridViewC;

        DateTime selectedDateForUpDate;

        private bool isCorrectEntries = false;
        private bool closeButtonClicked = false;

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

            
            checkBoxAddToBoldedDates = new CheckBox
            {
                Text = resourceManager.GetString("discard"),
                Location = new Point(10, 10),
                Visible = false
            };
            Controls.Add(checkBoxAddToBoldedDates);

        }


        private void InitializeGridView(DateTime selectedDate)
        {
            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            CultureInfo ci = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            CreateDataGridViewWithReadOnly(selectedDate, false);

            Controls.Add(dataGridView);

            CreateDataGridViewContacts();

            closeButton = new Button();
            closeButton.Text = resourceManager.GetString("close");
            closeButton.Location = new System.Drawing.Point(120, 370);
            closeButton.Size = new System.Drawing.Size(100, 50);
            closeButton.Click += new EventHandler(SaveButton_Click);
            Controls.Add(closeButton);

            saveButton = new Button();
            saveButton.Text = resourceManager.GetString("save");
            saveButton.Location = new System.Drawing.Point(10, 370);
            saveButton.Size = new System.Drawing.Size(100, 50);
            saveButton.Click += new EventHandler(CloseButton_Click);
            Controls.Add(saveButton);

        }

        private void CreateDataGridViewContacts()
        {
            dataGridViewC = new DataGridView();
            dataGridViewC.Location = new System.Drawing.Point(380, 70);
            dataGridViewC.Size = new System.Drawing.Size(400, 300);

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

            ContactDao contactDao = new ContactDao();

            dataGridViewC.DataSource = contactDao.GetContacts();
            dataGridViewC.DataMember = "contacts";

            Controls.Add(dataGridViewC);
        }

        private void DataGridViewC_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
           
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    // Setzen des Wertes "x" für jede neue Zeile in der "xColumn" Spalte
                    dataGridViewC.Rows[i].Cells["xColumn"].Value = "+";
                }
            
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            closeButtonClicked = true;
            this.Close();
        }

        private void CreateDataGridViewWithReadOnly(DateTime selectedDate, bool isReadOnly)
        {
            dataGridView = new DataGridView();
            dataGridView.Width = 360;
            dataGridView.Location = new Point(10, 120);
            dataGridView.ScrollBars = ScrollBars.Vertical;
            dataGridView.RowHeadersVisible = false;

            dataGridView.CellClick += DataGridView_CellClick;

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

            // Ereignis für das Befüllen der Zellen registrieren
            dataGridView.RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridView_RowsAdded);

            DateDao dateDao = new DateDao();

            dataGridView.AllowUserToAddRows = false;
            dataGridView.ReadOnly = isReadOnly;

            dataGridView.DataSource = dateDao.GetDataSetDatesForSelectedDate(selectedDate);
            dataGridView.DataMember = "dates";
        }

        private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                // Setzen des Wertes "x" für jede neue Zeile in der "xColumn" Spalte
                dataGridView.Rows[i].Cells["xColumn"].Value = "x";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Close();
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
                    //var text = row.Cells["text"].Value;
                    //var start = row.Cells["start"].Value;
                    //var end = row.Cells["end"].Value;

                    DateDao dateDao = new DateDao();
                    dateDao.DeleteEntryById(id);

                    checkBoxAddToBoldedDates.Checked = true;
                    Close();
                }
                else
                {

                    // Zugriff auf die aktuelle Reihe
                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
                    var id = row.Cells["id"].Value;

                    // Neue Form erstellen und die id übergeben
                    DetailsForm detailsForm = new DetailsForm(id, this);
                    detailsForm.ShowDialog(); // Optional: Falls das Hauptfenster blockiert wird, bis die DetailsForm geschlossen wird, verwendet man ShowDialog(), sonst Show().


                }
            }
        }

        private void InitializeAppointmentControls()
        {

            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            CultureInfo ci = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            comboBox = new ComboBox();
            comboBox.Location = new Point(10, 10); // X, Y Koordinaten auf dem Formular
            comboBox.Size = new Size(125, 20); // Breite, Höhe

            wholeDay = new CheckBox();
            wholeDay.Location = new Point(200, 10);
            wholeDay.CheckedChanged += new EventHandler(WholeDay_CheckedChanged); 
            this.Controls.Add(wholeDay);

            Label wholeDayLabel = new Label();
            wholeDayLabel.Text = resourceManager.GetString("whole day");
            wholeDayLabel.Location = new Point(140, 12);
            this.Controls.Add(wholeDayLabel);

            comboBox.Items.Add(resourceManager.GetString("no repetition") + " n");
            comboBox.Items.Add(resourceManager.GetString("monthly repetition") + " m");
            comboBox.Items.Add(resourceManager.GetString("yearly repetition") + " y");
            comboBox.Items.Add(resourceManager.GetString("weekly repetition") + " w");

            // Optional: Standardauswahl setzen
            comboBox.SelectedIndex = 0; // Wählt die erste Option "wöchentlich" als Standard

            // Fügen Sie die Combobox zum Formular hinzu
            this.Controls.Add(comboBox);

            textBoxDate = new TextBox
            {
                Location = new Point(10, 40),
                Width = 200
            };
            Controls.Add(textBoxDate);

            // DateTimePicker für die Anfangsuhrzeit
            /*
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
            */

            dateTimePickerStart = new DateTimePicker();
            dateTimePickerStart.Format = DateTimePickerFormat.Custom;
            dateTimePickerStart.CustomFormat = "dd.MM.yyyy HH:mm";
            dateTimePickerStart.Location = new Point(10, 70);

            dateTimePickerEnd = new DateTimePicker();
            dateTimePickerEnd.Format = DateTimePickerFormat.Custom;
            dateTimePickerEnd.CustomFormat = "dd.MM.yyyy HH:mm";
            dateTimePickerEnd.Location = new Point(10, 95);

            Controls.Add(dateTimePickerStart);
            Controls.Add(dateTimePickerEnd);

            dateTimePickerStart.ValueChanged += DatePicker_ValueChanged;
            dateTimePickerEnd.ValueChanged += DatePicker_ValueChanged;

            this.FormClosed += AppointmentForm_FormClosed;

            UpdateDatePickerWholeDay();
        }

        private void WholeDay_CheckedChanged(object sender, EventArgs e) 
        {
            UpdateDatePickerWholeDay();
        }


        private void UpdateDatePickerWholeDay() 
        {
            bool isCheckedWholeDay = wholeDay.Checked;
            if (isCheckedWholeDay) {

                DateTime currentDate = dateTimePickerStart.Value;
                int newHour = 00; 
                int newMinute = 00;
                DateTime newDateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, newHour, newMinute, 0);
                dateTimePickerStart.Value = newDateTime;

                currentDate = dateTimePickerStart.Value;
                newHour = 23;
                newMinute = 59;
                newDateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, newHour, newMinute, 0);
                dateTimePickerEnd.Value = newDateTime;

            }
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
            if (!closeButtonClicked)
            {
                checkBoxAddToBoldedDates.Checked = true;
            }

            if (!datePickerSelected && !checkBoxAddToBoldedDates.Checked)
            {
                MessageBox.Show("Bitte wählen Sie Termine für Anfangs- und Endzeit aus.");
                e.Cancel = true; // Verhindert das Schließen des Formulars
                checkBoxAddToBoldedDates.Checked = true;
            }
            else
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
                        MessageBox.Show("The start date must be before the end date.");
                        e.Cancel = true;
                    }
                }
                else
                {
                    SetAppointmentDetails(textBoxDate.Text, dateTimePickerStart.Value.TimeOfDay, dateTimePickerEnd.Value.TimeOfDay);
                }
                SelectedRepetitionIndex = comboBox.SelectedIndex;
            }
        }

    }
}
