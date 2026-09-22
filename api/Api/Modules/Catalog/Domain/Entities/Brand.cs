namespace Api.Modules.Catalog.Domain.Entities
{
    public sealed class Brand
    {
        public uint Id { get; private set; }
        public Guid ExternalId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public bool Active { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Brand()
        {
        }

        public static Brand Restore(
            uint id,
            Guid externalId,
            string name,
            string slug,
            bool active,
            DateTime createdAt,
            DateTime updatedAt)
        {
            return new Brand
            {
                Id = id,
                ExternalId = externalId,
                Name = name,
                Slug = slug,
                Active = active,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }
    }
}