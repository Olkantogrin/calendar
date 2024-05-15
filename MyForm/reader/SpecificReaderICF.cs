namespace MyForm.reader
{ 
        public class SpecificReaderICF : Creator
        {
            public override IProdukt FactoryMethod()
            {
                return new SpecificReaderProductICS();
            }
        }
}
