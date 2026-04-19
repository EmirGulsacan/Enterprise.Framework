namespace Enterprise.Framework.Application.Common.Behaviors.Contracts;

public interface IIdempotentCommand<TResponse>
{
    string IdempotencyKey { get; }
}
