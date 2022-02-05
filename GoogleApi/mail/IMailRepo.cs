using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.mail
{
    public interface IMailRepo
    {
        Task<List<EmailMessage>> GetEmailMessages(string userId, string labelId);
    }
}
