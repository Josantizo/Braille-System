namespace BrailleSuite.Api.Shared.Entities;

public class User
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Folder> Folders { get; set; } = new List<Folder>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
