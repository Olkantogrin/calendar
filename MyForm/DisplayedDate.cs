namespace MyForm
{
    public class DisplayedDate
    {
        private string text;
        private string time;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        // Getter und Setter für start
        public string Time
        {
            get { return time; }
            set { time = value; }
        }


        public DisplayedDate(string text, string time)
        {
            this.text = text;
            this.time = time;
        }
    }
}