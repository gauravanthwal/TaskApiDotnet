namespace Task.Domain.Dto
{
    public class CreateTaskDto
    {
        public string Task { get; set; }
    }


    public class UpdateTaskDto
    {
        public string Task { get; set; }
        public bool IsCompleted { get; set; }
    }
}
