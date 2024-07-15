using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForm
{
    class Contact
    {

        private string text;
        private string contactStreetAndNumber;
        private string contactPostalCodeAndCity;
        private string contactTel;
        private string contactMail;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string ContactStreetAndNumber
        {
            get { return contactStreetAndNumber; }
            set { contactStreetAndNumber = value; }
        }

        public string ContactPostalCodeAndCity
        {
            get { return contactPostalCodeAndCity; }
            set { contactPostalCodeAndCity = value; }
        }

        public string ContactTel
        {
            get { return contactTel; }
            set { contactTel = value; }
        }

        public string ContactMail
        {
            get { return contactMail; }
            set { contactMail = value; }
        }


        public Contact(string text, string contactStreetAndNumber, string contactPostalCodeAndCity, string contactTel, string contactMail)
        {
            this.text = text;
            this.contactStreetAndNumber = contactStreetAndNumber;
            this.contactPostalCodeAndCity = contactPostalCodeAndCity;
            this.contactTel = contactTel;
            this.contactMail = contactMail;

    }


        public override string ToString()
        {
            return $"Text: {Text}, ContactStreetAndNumber: {ContactStreetAndNumber}, ContactPostalCodeAndCity: {ContactPostalCodeAndCity}, ContactTel: {ContactMail}, ContactTel: {ContactMail}";
        }

    }

}

