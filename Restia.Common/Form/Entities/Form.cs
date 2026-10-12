using Restia.Common.Shared.Entities;

namespace Restia.Common.Form.Entities
{
    public class Form : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Url { get; set; }

        public string? Icon { get; set; }

        public int? ParentId { get; set; }

        public Form? Parent { get; set; }

        public ICollection<Form> Children { get; set; } = [];

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}