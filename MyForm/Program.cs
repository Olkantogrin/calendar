using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        public Form1()
        {
            // Initialisierung des MonthCalendar
            monthCalendar = new MonthCalendar
            {
                CalendarDimensions = new Size(1, 1),
                Location = new Point(10, 10)
            };

            DrawCalendarForFreshMonth(monthCalendar.SelectionStart.Month, monthCalendar.SelectionStart.Year);

            previousDate = monthCalendar.SelectionStart;

            // Benutzerdefinierte Schriftart für ausgewählte Daten
            boldDateFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

            // Ereignishandler für das DateSelected-Ereignis
            monthCalendar.DateSelected += MonthCalendar_DateSelected;

            monthCalendar.DateChanged += MonthCalendar_DateChanged;

            // Hinzufügen des MonthCalendar zur Form
            Controls.Add(monthCalendar);
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
                appointmentForm = new AppointmentForm(monthCalendar.SelectionStart);
                appointmentForm.FormClosed += AppointmentForm_FormClosed;
                appointmentForm.Show();

                /*
                // Aktualisieren des ausgewählten Datums
                selectedDate = monthCalendar.SelectionStart;
                monthCalendar.AddBoldedDate(selectedDate);
                monthCalendar.UpdateBoldedDates();

                // Erzwingen einer Neumalerei des Kalenders
                monthCalendar.Invalidate();
                */
            }
            else {
                // Wenn eine Instanz bereits geöffnet ist, fokussiere sie
                appointmentForm.Focus();
            }
        }

        private void AppointmentForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            // Entfernen der Referenz auf die geschlossene Instanz von AppointmentForm
            appointmentForm = null;

            // Aktualisieren des ausgewählten Datums, falls erforderlich
            selectedDate = monthCalendar.SelectionStart;
 
            //TODO: Wenn das hier unten false ist, dann müssen für dengegebenen Monat und das Jahr, den der Kalender gerade hat, alle Termine ausgelesen werden, dann den Kalender nue bauen.
            MessageBox.Show((!((AppointmentForm)sender).AddToBoldedDates).ToString());
            
            if (!((AppointmentForm)sender).AddToBoldedDates)
            {
                AppointmentForm form = sender as AppointmentForm;
                if (form != null)
                {
                    
                    // Zugriff auf die Werte
                    //string date = form.SelectedDate;


                    
                    string text = form.TextBoxDate;

                    string startDay = form.StartDate.ToString();
                    
                    string endDay = form.EndDate.ToString();

                    //TODO: Wenn startDay oder endDay nicht der aktuelle Tag sind: MessageBox.Show("Wenn startDay oder endDay nicht der aktuelle Tag sind: Bitte die Kalender benutzen.");

                    TimeSpan start = form.StartTime;
                    string startHour = start.ToString().Substring(0, 5);

                    TimeSpan end = form.EndTime;

                    string endHour = end.ToString().Substring(0, 5);

                    // Setzen der Werte in der AppointmentForm
                    //form.SetAppointmentDetails(date, start, end);

                    bool isTextEmpty = String.IsNullOrEmpty(text);

                    if (isTextEmpty) {
                        text = "mein Ereignis";
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
