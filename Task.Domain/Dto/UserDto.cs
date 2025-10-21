namespace Task.Domain.Dto
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public class LoginUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
