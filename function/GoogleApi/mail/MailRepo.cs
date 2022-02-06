using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using GoogleApi.domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.IO;
using MimeKit.IO.Filters;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;

namespace GoogleApi.mail
{
    public class MailRepo : IMailRepo
    {
        static string[] scopes = { "https://www.googleapis.com/auth/gmail.modify", "https://www.googleapis.com/auth/gmail.labels" };
        static string ApplicationName = "receipt-func";
        private readonly IConfiguration _config;
        private GoogleCloudApiConfig _googleCloudApiConfig = new GoogleCloudApiConfig();
        private readonly ILogger _log;

        public MailRepo(IConfiguration config, ILogger<MailRepo> log)
        {
            _config = config;
            _config.Bind("receipt-func-secrets:google_cloud", _googleCloudApiConfig);
            _log = log;
        }

        private GmailService GetServiceForUser(string userId)
        {
            string credJson = JsonConvert.SerializeObject(_googleCloudApiConfig);
            GoogleCredential credential = GoogleCredential.FromJson(credJson)
                         .CreateScoped(scopes).CreateWithUser(userId);
            // Create Gmail API service.
            GmailService service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public void GetMessagesStream(string userId, string labelId)
        {
            var gmailSvc = GetServiceForUser(userId);
            var listRequest = gmailSvc.Users.Messages.List(userId);
            listRequest.LabelIds = new Repeatable<string>(new string[] { labelId });
            listRequest.MaxResults = 20;

        }

        public async Task<List<EmailMessage>> GetEmailMessages(string userId, string labelId)
        {
            var gmailSvc = GetServiceForUser(userId);
            var listRequest = gmailSvc.Users.Messages.List(userId);
            listRequest.LabelIds = new Repeatable<string>(new string[] { labelId });
            listRequest.MaxResults = 20;
            ListMessagesResponse listResponse = await listRequest.ExecuteAsync();
            List<Message> messages = new List<Message>();
            List<EmailMessage> emailMessages = new List<EmailMessage>();
            messages.AddRange(listResponse.Messages);
            while (!string.IsNullOrEmpty(listResponse.NextPageToken))
            {
                listRequest.PageToken = listResponse.NextPageToken;
                listResponse = await listRequest.ExecuteAsync();
                if (listResponse.Messages.Any())
                {
                    messages.AddRange(listResponse.Messages);
                }
            }
            foreach (var item in messages)
            {
                var detailRequest = gmailSvc.Users.Messages.Get(userId, item.Id);
                //get raw
                detailRequest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
                var msg = await detailRequest.ExecuteAsync();
                string converted = msg.Raw.Replace('-', '+');
                converted = converted.Replace('_', '/');
                byte[] decodedByte = Convert.FromBase64String(converted);                
                using (var stream = new MemoryStream(decodedByte))
                {
                    // Convert to MimeKit from GMail
                    // Load a MimeMessage from a stream
                    var mkitMsg = await MimeMessage.LoadAsync(stream);                    
                    EmailMessage emailMessage = new EmailMessage
                    {
                        Raw = decodedByte                        
                    };
                    BuildMessage(mkitMsg, emailMessage);
                    string fileName = $"{emailMessage.From}_{emailMessage.Created.ToString("yyyy-MM-dd")}_{mkitMsg.MessageId}.eml";                    
                    emailMessage.FileName = fileName;                    
                    emailMessages.Add(emailMessage);
                }                
            }
            return emailMessages;
        }

        private void BuildMessage(MimeMessage mkitMsg, EmailMessage emailMessage)
        {
            //MimeMessage mimeMessage = await MimeMessage.LoadAsync()
            if (mkitMsg == null) return;
            emailMessage.Created = mkitMsg.Date.Date;
            emailMessage.Subject = mkitMsg.Subject;
            emailMessage.From = GetFrom(mkitMsg.From[0].ToString()).Host;
        }

        private MailAddress GetFrom(string field)
        {
            if (String.IsNullOrWhiteSpace(field))
                return null;

            MimeKit.InternetAddressList addresses;
            if (MimeKit.InternetAddressList.TryParse(field, out addresses))
            {
                return addresses.Select(s =>
                {
                    if (s is MimeKit.MailboxAddress)
                    {
                        var i = s as MimeKit.MailboxAddress;
                        return new MailAddress(i.Address, i.Name);
                    }
                    else if (s is MimeKit.GroupAddress)
                    {
                        var i = s as MimeKit.GroupAddress;
                        return null;
                    }
                    else
                    {
                        throw new NotImplementedException("Could not find SigParser code handler for address type " + s.GetType().FullName);
                    }
                }).FirstOrDefault(a => a != null);
            }
            else
            {
                return null;
            }
        }
    }
}
