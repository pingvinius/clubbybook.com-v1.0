namespace ClubbyBook.BackgroundActions.Mailing
{
    using System;
    using System.Collections.Generic;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Mail;
    using ClubbyBook.UI;

    public sealed class SendPasswordWasChangedMailAction : MailingAction
    {
        private string userEmail;
        private string userName;
        private string newPassword;

        public SendPasswordWasChangedMailAction(User user, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            this.userName = UIUtilities.ValidateStringValue(UIUtilities.GetUserFullName(user));
            this.userEmail = user.EMail;
            this.newPassword = newPassword;
        }

        protected override IEnumerable<SendItem> CreateSendItems()
        {
            var template = MailTemplatesFactory.CreatePasswordWasChangedTemplate(this.userName, this.newPassword);
            return new List<SendItem>()
            {
                new SendItem()
                {
                    To = this.userEmail,
                    Subject = template.Subject,
                    Body = template.Body
                }
            };
        }
    }
}