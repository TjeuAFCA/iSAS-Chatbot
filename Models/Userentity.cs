namespace chatbot_iSAS.Models
{
    public class Userentity
    {
        public string sessionId { get; set; }
        public string name { get; set; }
        public bool extend { get; set; }
        public Entry[] entries { get; set; }
    }

    public class Entry
    {
        public string value { get; set; }
        public string[] synonyms { get; set; }
    }

}