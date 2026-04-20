namespace Enterprise.Framework.Application.Features.Identity;

using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed class UserDto : IMapFrom<AppUser>
{
    public long Id { get; set; }
    public string IdentityId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<long> RoleIds { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AppUser, UserDto>()
            .ForMember(d => d.RoleIds, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.RoleId).ToList()));
    }
}

public sealed record RoleDto(long Id, string Name, string? Description) : IMapFrom<AppRole>;

public sealed class PermissionDto : IMapFrom<AppPermission>
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ModuleName { get; set; } = string.Empty;
}
