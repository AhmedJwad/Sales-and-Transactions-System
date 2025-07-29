using Sale.Share.Responses;

namespace Sale.Api.Helpers
{
    public interface IMailHelper
    {
        ActionResponse<string> SendEmail(string toName, string toEmail , string subject , string body);
    }
}
