namespace MyWorkItem.ViewModels.WorkItems
{
    public class WorkItemDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public string Sort { get; set; } = "desc";
    }
}
