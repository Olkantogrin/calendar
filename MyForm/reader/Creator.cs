using System;
using System.IO;
using System.Windows.Forms;

namespace MyForm.reader
{
    public abstract class Creator
    {
        public abstract IProdukt FactoryMethod();

        public Date SpecificReadDateOperation(string path)
        {
            var produkt = FactoryMethod();

            string fileContent = "";

            try
            {
                fileContent = File.ReadAllText(path);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Date result = produkt.Operation(fileContent);

            return result;
        }
    }
}