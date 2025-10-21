
namespace Task.Domain.Models
{
    public class MyTask
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; }

        public User? User { get; set; }
    }
}
