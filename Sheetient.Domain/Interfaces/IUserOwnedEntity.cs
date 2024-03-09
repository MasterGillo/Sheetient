namespace Sheetient.Domain.Interfaces
{
    public interface IUserOwnedEntity : IEntity
    {
        int UserId { get; set; }
    }
}
