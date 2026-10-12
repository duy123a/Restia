namespace Restia.Common.Form.Models
{
    public class FormItemDto
    {
        public string NameKey { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public List<FormItemDto> Children { get; set; } = [];
    }
}
