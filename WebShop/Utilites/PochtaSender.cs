using Microsoft.AspNetCore.Identity.UI.Services;
using WebShop.Areas.Identity.Pages.Account.Manage;

namespace WebShop.Utilites;

public class PochtaSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        throw new NotImplementedException();
    }
}