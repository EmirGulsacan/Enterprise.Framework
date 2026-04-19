namespace Enterprise.Framework.Application.Common.Interfaces;

public interface IEmailService {
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
}

public interface ISmsService {
    Task SendSmsAsync(string phoneNumber, string message);
}
