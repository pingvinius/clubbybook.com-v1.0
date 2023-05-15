namespace ClubbyBook.BackgroundActions.Mailing
{
    using System;
    using System.Collections.Generic;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Mail;

    public sealed class SendRegistrationMailAction : MailingAction
    {
        private string registeredUserEmail;
        private string password;

        public SendRegistrationMailAction(User registeredUser, string password)
        {
            if (registeredUser == null)
            {
                throw new ArgumentNullException("registeredUser");
            }

            this.registeredUserEmail = registeredUser.EMail;
            this.password = password;
        }

        protected override IEnumerable<SendItem> CreateSendItems()
        {
            var template = MailTemplatesFactory.CreateRegistrationTemplate(this.registeredUserEmail, this.password);
            return new List<SendItem>()
            {
                new SendItem()
                {
                    To = registeredUserEmail,
                    Subject = template.Subject,
                    Body = template.Body
                }
            };
        }
    }
}