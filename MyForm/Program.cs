using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace MyForm
{
    public class Form1 : Form
    {
        ResourceManager resourceManager;

        private MonthCalendar monthCalendar;
        private Font boldDateFont;
        private DateTime selectedDate;
        private DateTime previousDate;

        private AppointmentForm appointmentForm;
        private ContactsForm contactForm;

        private ComboBox dropdownList;
        private string locale = "de-DE";

        private string readFilePath;

        private Thread schedulerThread;

        private DateDao dateDao;

        public Form1()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;

            // MonthCalendar initialization
            monthCalendar = new MonthCalendar
            {
                CalendarDimensions = new Size(1, 1),
                Location = new Point(10, 10)
            };
            DrawCalendarForFreshMonth(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);
            prepareMonthCalendar();
            Controls.Add(monthCalendar);

            // Dropdown list initialization
            dropdownList = new ComboBox
            {
                Location = new Point(10, 200),
                Width = 150
            };
            dropdownList.Items.AddRange(new string[] { "English", "Deutsch", "русский" });
            dropdownList.SelectedIndexChanged += DropdownList_SelectedIndexChanged;
            Controls.Add(dropdownList);

            // Localization
            dateDao = new DateDao();
            resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            string loc = dateDao.GetLocale();
            CultureInfo ci = new CultureInfo(loc);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            // Buttons
            var readerButtonIcs = CreateButton("read ics", new Point(170, 200), new Size(100, 25));
            readerButtonIcs.Click += ReadButton_ClickICS;
            Controls.Add(readerButtonIcs);

            var contactsButton = CreateButton("contacts", new Point(170, 174), new Size(100, 25));
            contactsButton.Click += ContactButton_Click;
            Controls.Add(contactsButton);
        }

        private Button CreateButton(string textResourceId, Point location, Size size)
        {
            var button = new Button
            {
                Text = resourceManager.GetString(textResourceId),
                Location = location,
                Size = size
            };
            return button;
        }

        private void ContactButton_Click(object sender, EventArgs e)
        {
            if (contactForm == null || contactForm.IsDisposed)
            {
                contactForm = new ContactsForm(dateDao?.GetLocale()); // Use null-conditional operator
                if (appointmentForm != null) { appointmentForm.Close(); }
                contactForm.Show();
            }
            else
            {
                contactForm.Focus();
            }
        }

        private void ReadButton_ClickICS(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            openFileDialog.Title = "Datei auswählen";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    readFilePath = openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(readFilePath)) { 

            reader.ReaderClient datesReader = new reader.ReaderClient();
            int datesRead = datesReader.ReadeDatesFile(readFilePath, "ICS");

            if (datesRead > -1) {

                    
                    Controls.Remove(monthCalendar);

                    monthCalendar = new MonthCalendar
                    {
                        CalendarDimensions = new Size(1, 1),
                        Location = new Point(10, 10)
                    };

                    prepareMonthCalendar();

                    Controls.Add(monthCalendar);

                    DrawCalendarForFreshMonth(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);


                    
                }
            } 
        }

        private void ReStartScheduler() {
            schedulerThread.Abort();
            StartScheduler();
        }


        private void StartScheduler()
        {
            bool isSchedulerRunning = true;
            schedulerThread = new Thread(() =>
            {
                while (isSchedulerRunning)
                {
                    DateTime now = DateTime.Now;
                    Dictionary<Date, List<DateTime>> selectedDatesByDay = dateDao.GetSelectedTextDatesForMonthAndYear(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);

                    foreach (KeyValuePair<Date, List<DateTime>> entry in selectedDatesByDay)
                    {
                        foreach (DateTime dateTime in entry.Value)
                        {
                            bool alert = Math.Abs((dateTime - now).TotalMinutes) <= 10;
                            if (alert)
                            {
                                MessageBox.Show($"{entry.Key.Text} " + resourceManager.GetString("starts within 10 minutes"));
                            }
                        }
                    }

                    Thread.Sleep(20000); // Wait 20 seconds before the next check
                }
            });
            schedulerThread.IsBackground = true;
            schedulerThread.Start();
        }

        private void DropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            dateDao = new DateDao();

            if (dropdownList.SelectedIndex == 0)
           {
                locale = "en-GB";
                dateDao.UpdateLocale(locale);
            }

            else if (dropdownList.SelectedIndex == 1)
            {
                locale = "de-DE";
                dateDao.UpdateLocale(locale);
            }

            else if (dropdownList.SelectedIndex == 2)
            {
                locale = "ru-RU";
                dateDao.UpdateLocale(locale);
            }

            Application.Restart();
            Environment.Exit(0);
        }

        private void prepareMonthCalendar()
        {
            previousDate = monthCalendar.SelectionStart;

            // Benutzerdefinierte Schriftart für ausgewählte Daten
            boldDateFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

            // Ereignishandler für das DateSelected-Ereignis
            monthCalendar.DateSelected += MonthCalendar_DateSelected;

            monthCalendar.DateChanged += MonthCalendar_DateChanged;
        }

        private void MonthCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            // Überprüfe, ob sich der Monat geändert hat
            if (previousDate.Month != monthCalendar.SelectionStart.Month)
            {
                // Der Monat hat sich geändert, führe die gewünschte Aktion aus
                // ...
                DrawCalendarForFreshMonth(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);
            }
            // Aktualisiere previousDate mit dem neuen Datum
            previousDate = monthCalendar.SelectionStart;

            ReStartScheduler();
        }


        private void DrawCalendarForFreshMonth(int month, int year)
        {
            
            dateDao = new DateDao();

            List<DateTime> selectedDates = dateDao.GetSelectedDatesForMonthAndYear(month, year);
             
            foreach(DateTime selectedDate in selectedDates) {

                monthCalendar.AddBoldedDate(selectedDate);
            }
            monthCalendar.UpdateBoldedDates();
            // Erzwingen einer Neumalerei des Kalenders
            monthCalendar.Invalidate();

        }


        private void MonthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        { 
            if (appointmentForm == null || appointmentForm.IsDisposed)
            {
                dateDao = new DateDao();

                string loc = dateDao.GetLocale();

                appointmentForm = new AppointmentForm(monthCalendar.SelectionStart, loc);
                appointmentForm.FormClosed += AppointmentForm_FormClosed;
                if (contactForm!=null) { contactForm.Close(); } 
                appointmentForm.Show();

            }
            else {
                // Wenn eine Instanz bereits geöffnet ist, fokussiere sie
                appointmentForm.Focus();
            }
        }

        

        private void AppointmentForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            CultureInfo ci = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            // Entfernen der Referenz auf die geschlossene Instanz von AppointmentForm
            appointmentForm = null;

            // Aktualisieren des ausgewählten Datums, falls erforderlich
            selectedDate = monthCalendar.SelectionStart;

            
            if (((AppointmentForm)sender).AddToBoldedDates) {
                
                Controls.Remove(monthCalendar);

                monthCalendar = new MonthCalendar
                {
                    CalendarDimensions = new Size(1, 1),
                    Location = new Point(10, 10)
                };

                prepareMonthCalendar();

                Controls.Add(monthCalendar);

                DrawCalendarForFreshMonth(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);

                //Application.Restart();
                //Environment.Exit(0);

            }



            if (!((AppointmentForm)sender).AddToBoldedDates)
            {
                AppointmentForm form = sender as AppointmentForm;
                if (form != null)
                {
                                        
                    string text = form.TextBoxDate;

                    string startDay = form.StartDate.ToString();
                    
                    string endDay = form.EndDate.ToString();

                    TimeSpan start = form.StartTime;
                    string startHour = start.ToString().Substring(0, 5);

                    TimeSpan end = form.EndTime;

                    string endHour = end.ToString().Substring(0, 5);

                    bool isTextEmpty = string.IsNullOrEmpty(text);

                    if (isTextEmpty) {
                        text = resourceManager.GetString("my event");
                    }
                    
                    string[] textStartEnd = new string[3];
                    textStartEnd[0] = text;
                    textStartEnd[1] = startDay.Split(' ')[0] + " " + startHour;
                    textStartEnd[2] = endDay.Split(' ')[0] + " " + endHour;

                    int selectedRepetitionIndex = form.SelectedRepetitionIndex;
                    string repetitionType = "n"; // Standardwert

                    if (selectedRepetitionIndex == 1)
                    {
                        repetitionType = "m"; // Monatliche Wiederholung nach Kalenderzahl
                    }
                    else if (selectedRepetitionIndex == 2)
                    {
                        repetitionType = "y"; // Jährliche Wiederholung
                    }
                    else if (selectedRepetitionIndex == 3)
                    {
                        repetitionType = "d"; // Monatliche Wiederholung nach Wochentag
                    }

                    Date d = new Date(textStartEnd[0], textStartEnd[1], textStartEnd[2], repetitionType);

                    List<Date> datelist = new List<Date>();
                    datelist.Add(d);

                    Span span = new Span();

                    List<DateTime> selectedDates = span.GetSelectedDateTimesByDateList(datelist);

                    foreach (DateTime selectedDate in selectedDates)
                    {
                        monthCalendar.AddBoldedDate(selectedDate);
                    }
                    monthCalendar.UpdateBoldedDates();
                    
                    dateDao = new DateDao();
                    dateDao.SaveAppointment(d);
                
                }
            }

                     // Erzwingen einer Neumalerei des Kalenders
                      monthCalendar.Invalidate();

            ReStartScheduler();

        }


    }


    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
             
        }
    }
}
