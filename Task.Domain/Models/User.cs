namespace Task.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Role { get; set; }
        public string? RefreshToekn { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<MyTask>? Tasks { get; set; }
    }
}
