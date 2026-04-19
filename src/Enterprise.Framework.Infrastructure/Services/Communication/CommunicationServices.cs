namespace Enterprise.Framework.Infrastructure.Services.Communication;

using Enterprise.Framework.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;



public class SmtpEmailService : IEmailService {

private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(ILogger<SmtpEmailService> logger) {
    
_logger = logger;

    

 Task SendEmailAsync(string to, string subject, string body, bool isHtml = true) {
    
_logger.LogInformation("Email dispatched to: 
To}
, Subject: 
Subject}
", to, subject);

        return Task.CompletedTask;

    }



 class MockSmsService : ISmsService {

private readonly ILogger<MockSmsService> _logger;

    public MockSmsService(ILogger<MockSmsService> logger) {
    
_logger = logger;

    

 Task SendSmsAsync(string phoneNumber, string message) {
    
_logger.LogInformation("SMS dispatched to: 
PhoneNumber}
, Message: 
Message}
", phoneNumber, message);

        return Task.CompletedTask;
}



