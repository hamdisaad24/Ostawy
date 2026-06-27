namespace Ostawy.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receptor, string Sub, string body);
    }
}
