using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace MyForm
{
    public class Form1 : Form
    {
        private MonthCalendar monthCalendar;
        private Font boldDateFont;
        private DateTime selectedDate;
        private AppointmentForm appointmentForm;
        private DateTime previousDate;
        
        private ComboBox dropdownList;
        private string locale = "de-DE";

        private string readFilePath;

        private Thread schedulerThread;

        public Form1()
        {


            
            // Initialisierung des MonthCalendar
            monthCalendar = new MonthCalendar
            {
                CalendarDimensions = new Size(1, 1),
                Location = new Point(10, 10)
            };

            DrawCalendarForFreshMonth(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);

            prepareMonthCalendar();

            // Hinzufügen des MonthCalendar zur Form
            Controls.Add(monthCalendar);


            StartScheduler();

            // Initialisierung der Dropdown-Liste
            dropdownList = new ComboBox
            {
                Location = new Point(10, 200), // Ändere die Position entsprechend deiner Anforderungen
                Width = 150 // Ändere die Breite entsprechend deiner Anforderungen
            };

            // Füge die Werte zur Dropdown-Liste hinzu
            dropdownList.Items.AddRange(new string[] { "English", "Deutsch", "русский" });

            // Hinzufügen der Event-Handler für die Dropdown-Liste
            dropdownList.SelectedIndexChanged += DropdownList_SelectedIndexChanged;

            // Hinzufügen des MonthCalendar zur Form
            Controls.Add(monthCalendar);

            // Hinzufügen der Dropdown-Liste zur Form
            Controls.Add(dropdownList);

        
            DateDao dateDao = new DateDao();

            ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
            string loc = dateDao.GetLocale();
            CultureInfo ci = new CultureInfo(loc);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            
            Button readerButtonICS = new Button
            {
                Text = resourceManager.GetString("read file"),
                Location = new System.Drawing.Point(170, 200),
                Size = new System.Drawing.Size(100, 25)
            };

            // Füge den Button zur Form hinzu
            this.Controls.Add(readerButtonICS);

            // Registriere das Click-Ereignis
            readerButtonICS.Click += ReadButton_ClickICS;
            
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
                DateDao dateDao = new DateDao();

                ResourceManager resourceManager = new ResourceManager("MyForm.Resources.ResXFile", typeof(AppointmentForm).Assembly);
                string loc = dateDao.GetLocale();
                CultureInfo ci = new CultureInfo(loc);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                

                Dictionary<Date, List<DateTime>> selectedDates = dateDao.GetSelectedTextDatesForMonthAndYear(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);

                while (isSchedulerRunning)
                {
                    foreach (KeyValuePair<Date, List<DateTime>> entry in selectedDates)
                    {
                        foreach (DateTime dateTime in entry.Value)
                        {

                            DateTime tenMinutesAhead = DateTime.Now.AddMinutes(10);
                            DateTime zeroMinutesAhead = DateTime.Now;

                            bool alert = (dateTime >= zeroMinutesAhead && dateTime <= tenMinutesAhead);

                            if (alert)
                            {  
                                MessageBox.Show($"{entry.Key.Text} " + resourceManager.GetString("starts within 10 minutes"));
                            }
                        }
                    }
                    
                    Thread.Sleep(20000);
                    //Thread.Sleep(60000); // Warte eine Minute, bevor die nächste Überprüfung erfolgt
                }
                
            });
            schedulerThread.IsBackground = true;
            schedulerThread.Start();
        }

        private void DropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateDao dateDao = new DateDao();

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
            DateDao dateDao = new DateDao();

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
                // Öffnen des AppointmentForm-Formulars
                appointmentForm = new AppointmentForm(monthCalendar.SelectionStart, locale);
                appointmentForm.FormClosed += AppointmentForm_FormClosed;
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

                    bool isTextEmpty = String.IsNullOrEmpty(text);

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
                        repetitionType = "m"; // Monatliche Wiederholung
                    }
                    else if (selectedRepetitionIndex == 2)
                    {
                        repetitionType = "y"; // Jährliche Wiederholung
                    }
                    else if (selectedRepetitionIndex == 3)
                    {
                        repetitionType = "w"; // wöchentliche Wiederholung
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


                    DateDao dateDao = new DateDao();
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
