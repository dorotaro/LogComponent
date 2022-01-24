namespace Domain.Models
{ 
    public class LogLine
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public LogLine(string text)
        {
            Text = text;
        }
    }
}