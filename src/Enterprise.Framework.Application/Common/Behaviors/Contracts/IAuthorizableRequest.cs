namespace Enterprise.Framework.Application.Common.Behaviors.Contracts;

public interface IAuthorizableRequest {

IReadOnlyList<string> RequiredPermissions { get;
}



