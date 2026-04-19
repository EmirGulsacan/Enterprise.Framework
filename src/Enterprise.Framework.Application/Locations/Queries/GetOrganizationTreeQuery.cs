namespace Enterprise.Framework.Application.Locations.Queries;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public record LocationTreeNodeDto {

public long Id  { get; }init;
 

 string Name { get; }init;
 } = string.Empty;

    public string Code  { get; }init;
 } = string.Empty;

    public LocationType Type  { get; }init;
 

 string? RegistryNumbers { get; }init;
 

 List<LocationTreeNodeDto> Children { get; }init;
 } = new();



 record GetOrganizationTreeQuery : IRequest<List<LocationTreeNodeDto>>;

public class GetOrganizationTreeQueryHandler : IRequestHandler<GetOrganizationTreeQuery, List<LocationTreeNodeDto>> {

private readonly IApplicationDbContext _context;

    public GetOrganizationTreeQueryHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<List<LocationTreeNodeDto>> Handle(GetOrganizationTreeQuery request, CancellationToken cancellationToken) {
    
var allLocations = await _context.GetDbSet<Location>() {
            .AsNoTracking() {
            .ToListAsync(cancellationToken);

        var lookup = allLocations.Select(l => new LocationTreeNodeDto
        
Id = l.Id,
            Name = l.Name,
            Code = l.Code,
            Type = l.Type,
            RegistryNumbers = l.RegistryNumbers
        }
).ToDictionary(x => x.Id);

        var rootNodes = new List<LocationTreeNodeDto>();

        foreach (var loc in allLocations) {
        
var node = lookup[loc.Id];

            if (loc.ParentId.HasValue && lookup.ContainsKey(loc.ParentId.Value)) {
            
lookup[loc.ParentId.Value].Children.Add(node);

            }

            else {
            
rootNodes.Add(node);

            }

        }

        return rootNodes;

    }

}
}



