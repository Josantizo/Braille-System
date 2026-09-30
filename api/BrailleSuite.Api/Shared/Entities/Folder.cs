using BrailleSuite.Api.Shared.Common;

namespace BrailleSuite.Api.Shared.Entities;

public class Folder : ITimestamped
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public required string Name { get; set; }
    public Guid? ParentFolderId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public User? User { get; set; }
    public Folder? ParentFolder { get; set; }
    public ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
