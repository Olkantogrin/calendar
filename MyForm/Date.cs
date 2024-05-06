namespace MyForm
{
    public class Date
    {
        private string text;
        private string start;
        private string end;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        // Getter und Setter für start
        public string Start
        {
            get { return start; }
            set { start = value; }
        }

        // Getter und Setter für end
        public string End
        {
            get { return end; }
            set { end = value; }
        }

        public Date(string text, string start, string end)
        {
            this.text = text;
            this.start = start;
            this.end = end;
        }
    }
}