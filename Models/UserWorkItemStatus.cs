namespace MyWorkItem.Models
{
    public class UserWorkItemStatus
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int WorkItemId { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }

        public WorkItem? WorkItem { get; set; }
    }
}
