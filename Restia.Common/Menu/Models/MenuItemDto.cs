namespace Restia.Common.Menu.Models
{
    public class MenuItemDto
    {
        public string NameKey { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public List<MenuItemDto> Children { get; set; } = [];
    }
}
