namespace MyWorkItem.ViewModels.WorkItems
{
    public class WorkItemListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
    }
}
