namespace GoogleApi.mail
{
    public class EmailMessage
    {
        public string From { get;set; }
        public string To { get;set; }
        public string Subject { get;set; }
        public DateTime Created { get;set; }
        public byte[] Raw { get; set; }
        public string FileName { get; set; }   
    }
}
