namespace MyWorkItem.ViewModels.WorkItems
{
    public class WorkItemIndexViewModel
    {
        public string Sort { get; set; } = "desc";
        public List<WorkItemListItemViewModel> Items { get; set; } = [];
    }
}
