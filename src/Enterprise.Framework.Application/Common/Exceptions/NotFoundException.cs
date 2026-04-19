namespace Enterprise.Framework.Application.Common.Exceptions;

public class NotFoundException : Exception {

public NotFoundException() : base() {
    
NotFoundException(string message) : base(message) {
    
NotFoundException(string message, Exception innerException) : base(message, innerException) {
    
NotFoundException(string name, object key) : base($"Entity \"
name}
\" (
key}
) was not found.") {
    





}
}
}



