namespace Yggdrasil.Models
{
    public class Homework
    {
        public int Id { get; set; }
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public bool Finished { get; set; }
    }
}
