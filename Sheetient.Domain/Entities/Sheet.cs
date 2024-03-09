using Sheetient.Domain.Entities.Identity;
using Sheetient.Domain.Interfaces;

namespace Sheetient.Domain.Entities
{
    public class Sheet : IUserOwnedEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<Page> Pages { get; set; } = [];
    }
}
