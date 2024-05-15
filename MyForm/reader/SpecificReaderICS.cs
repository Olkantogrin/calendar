namespace MyForm.reader
{ 
        public class SpecificReaderICS : Creator
        {
            public override IProdukt FactoryMethod()
            {
                return new SpecificReaderProductICS();
            }
        }
}
