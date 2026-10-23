namespace Restia.Web.Models
{
    public class TableActionsViewModel
    {
        public bool CanModify { get; set; }
        public string? EditUrl { get; set; }
        public string DeleteAction { get; set; } = "Delete";
        public string? DeleteId { get; set; }
    }
}
