namespace Enterprise.Framework.Application.Locations.Commands;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;



public record CreateLocationCommand : IRequest<long> {

public string Name  { get; }init;
 } = string.Empty;

    public string Code  { get; }init;
 } = string.Empty;

    public LocationType Type  { get; }init;
 

 long? ParentId { get; }init;
 

 string? RegistryNumbers { get; }init;
 }



 class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, long> {

private readonly IApplicationDbContext _context;

    public CreateLocationCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<long> Handle(CreateLocationCommand request, CancellationToken cancellationToken) {
    
var location = new Location
        
Name = request.Name,
            Code = request.Code,
            Type = request.Type,
            ParentId = request.ParentId,
            RegistryNumbers = request.RegistryNumbers
        }
;

        _context.GetDbSet<Location>().Add(location);

        await _context.SaveChangesAsync(cancellationToken);

        return location.Id;

    }

}
}



