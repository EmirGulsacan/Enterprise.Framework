namespace Enterprise.Framework.Application.Features.Identity;

using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed class UserDto : IMapFrom<AppUser>
{
    public long Id { get; set; }
    public string IdentityId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<long> RoleIds { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public bool IsSystemAdmin { get; set; }
    public bool IsAdmin { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AppUser, UserDto>()
            .ForMember(d => d.RoleIds, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.RoleId).ToList()))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name).ToList()));
    }
}

public sealed class RoleDto : IMapFrom<AppRole>
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<long> PermissionIds { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AppRole, RoleDto>()
            .ForMember(d => d.PermissionIds, opt => opt.MapFrom(s => s.RolePermissions.Select(rp => rp.PermissionId).ToList()));
    }
}

public sealed class PermissionDto : IMapFrom<AppPermission>
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ModuleName { get; set; } = string.Empty;
}
