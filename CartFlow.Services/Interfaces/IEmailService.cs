using System;
using System.Collections.Generic;
using System.Text;

namespace CartFlow.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetLink);

    }
}
