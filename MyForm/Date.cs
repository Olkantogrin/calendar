namespace MyForm
{


   
    public class Date
    {
        private string text;
        private string start;
        private string end;
        private string repeat;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Start
        {
            get { return start; }
            set { start = value; }
        }

        public string End
        {
            get { return end; }
            set { end = value; }
        }

        public string Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }


        public Date(string text, string start, string end, string repeat)
        {
            this.text = text;
            this.start = start;
            this.end = end;
            this.repeat = repeat;
        }

        
        public override string ToString()
        {
            return $"Text: {Text}, Start: {Start}, End: {End}, Repeat: {Repeat}";
        }
        
    }
}