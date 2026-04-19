namespace Enterprise.Framework.Domain.Common.Exceptions;

public abstract class DomainException : Exception {

protected DomainException(string message) : base(message) {
    
}
}



