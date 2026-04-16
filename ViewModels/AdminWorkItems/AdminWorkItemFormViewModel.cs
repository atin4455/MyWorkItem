using System.ComponentModel.DataAnnotations;

namespace MyWorkItem.ViewModels.AdminWorkItems
{
    public class AdminWorkItemFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "標題為必填。")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
