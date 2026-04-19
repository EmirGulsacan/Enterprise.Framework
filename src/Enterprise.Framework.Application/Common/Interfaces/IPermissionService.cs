namespace Enterprise.Framework.Application.Common.Interfaces;

public interface IPermissionService {

Task<HashSet<string>> GetPermissionsAsync(string identityId, CancellationToken cancellationToken = default);
}



