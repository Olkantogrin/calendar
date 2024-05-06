using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyForm
{
    public class Entry
    {
        private static Entry _instance;
        private static readonly object _lock = new object();


        private Entry()
        {
        }

        public static Entry Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Entry();
                    }
                    return _instance;
                }
            }
        }

        public bool isCorrect(DateTimePicker dateTimePickerStart, DateTimePicker dateTimePickerEnd)
        {

            if (dateTimePickerStart.Value == null || dateTimePickerEnd.Value == null) {
                return false;
            }

            if (dateTimePickerStart.Value <= dateTimePickerEnd.Value)
            {
             return true;
            }

            return false;
        }
    }
}
