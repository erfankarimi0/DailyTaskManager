using DailyTaskManager.Enums;

public class GetAllTaskResultDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public TaskPriority Priority { get; set; }
}