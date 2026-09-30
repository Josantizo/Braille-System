using BrailleSuite.Api.Features.Documents.Dtos;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrailleSuite.Api.Features.Documents.Documents.GetDocumentDetail;

public record GetDocumentDetailQuery(Guid Id) : IRequest<DocumentDetailResponse>;

public class GetDocumentDetailHandler(AppDbContext db, ICurrentUserContext currentUser)
    : IRequestHandler<GetDocumentDetailQuery, DocumentDetailResponse>
{
    public async Task<DocumentDetailResponse> Handle(GetDocumentDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var document = await db.Documents
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.UserId == userId, cancellationToken)
            ?? throw new NotFoundException($"Document '{request.Id}' was not found.");

        return DocumentMapper.ToDetailResponse(document);
    }
}
