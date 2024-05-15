using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyForm.reader
{
    class ReaderClient
    {
        public int ReadeDatesFile(string path, string dateReaderType)
        {
            int ret = -1;

            if ("ICS".Equals(dateReaderType) && path.EndsWith("ics")) {
               ret = ClientCode(new SpecificReaderICS(), path);
            }

            return ret;
        }

        private int ClientCode(Creator creator, string path)
        {
            DateDao dateDao = new DateDao();

            Date date = creator.SpecificReadDateOperation(path);

            if (date == null) { return -1; }

            dateDao.SaveAppointment(date);

            return 0;
        }
    }
}
