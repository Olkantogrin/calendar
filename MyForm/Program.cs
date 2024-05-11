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
        private MonthCalendar monthCalendar;
        private Font boldDateFont;
        private DateTime selectedDate;
        private AppointmentForm appointmentForm;
        private DateTime previousDate;
        
        private ComboBox dropdownList;
        private string locale = "de-DE";

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

            /*
            DateDao dateDao = new DateDao();
            List<DateTime> selectedDates = dateDao.GetSelectedDatesForMonthAndYear(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);
            List<DisplayedDate> datelist = new List<DisplayedDate>();
            foreach (DateTime selectedDate in selectedDates)
            {
                datelist.Add(new DisplayedDate("Zu implementieren!", selectedDate.ToString()));
            }

            scheduler = new Scheduler(datelist);
            
            StartScheduler();
            */

            StartScheduler();

            // Initialisierung der Dropdown-Liste
            dropdownList = new ComboBox
            {
                Location = new Point(10, 200), // Ändere die Position entsprechend deiner Anforderungen
                Width = 150 // Ändere die Breite entsprechend deiner Anforderungen
            };

            // Füge die Werte zur Dropdown-Liste hinzu
            dropdownList.Items.AddRange(new string[] { "English" });

            // Hinzufügen der Event-Handler für die Dropdown-Liste
            dropdownList.SelectedIndexChanged += DropdownList_SelectedIndexChanged;

            // Hinzufügen des MonthCalendar zur Form
            Controls.Add(monthCalendar);

            // Hinzufügen der Dropdown-Liste zur Form
            Controls.Add(dropdownList);

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
                List<DateTime> selectedDates = dateDao.GetSelectedDatesForMonthAndYear(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);
                List<DisplayedDate> datelist = new List<DisplayedDate>();
                foreach (DateTime selectedDate in selectedDates)
                {
                    datelist.Add(new DisplayedDate("Zu implementieren!", selectedDate.ToString()));
                }

                while (isSchedulerRunning)
                {
                    foreach (var dt in datelist)
                    {
                        // Überprüfe, ob das Ereignis in weniger als 10 Minuten beginnt
                        DateTime dateTime = DateTime.ParseExact(dt.Time, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                        DateTime tenMinutesAhead = DateTime.Now.AddMinutes(10);
                        DateTime zeroMinutesAhead = DateTime.Now;

                        bool alert = (dateTime >= zeroMinutesAhead && dateTime <= tenMinutesAhead);

                        //TODO: Ic will wissen, wie der Termin heißt. Das dann da unten reinschrieben.
                        if (alert)
                        {
                            MessageBox.Show($"Achtung! {dt.Text} beginnt in 10 Minuten.");
                        }
                    }

                    //Thread.Sleep(10000);

                    Thread.Sleep(60000); // Warte eine Minute, bevor die nächste Überprüfung erfolgt
                }
            });
            schedulerThread.IsBackground = true;
            schedulerThread.Start();
        }

        private void DropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropdownList.SelectedIndex == 0)
           {
                locale = "en-GB";
            }
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

            //TODO: Testen! 
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
                     
                    Date d = new Date(textStartEnd[0], textStartEnd[1], textStartEnd[2]);
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

            //TODO: Testen!
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
