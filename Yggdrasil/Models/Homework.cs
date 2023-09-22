namespace Yggdrasil.Models
{
    public class Homework
    {
        public int Id { get; set; }
        public string Subject { get;set; }
        public string Description { get;set; }
        public DateTime Deadline { get;set; }
        public bool Finished { get;set; }
    }
}
