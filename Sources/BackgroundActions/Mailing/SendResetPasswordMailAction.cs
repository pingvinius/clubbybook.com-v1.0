namespace ClubbyBook.BackgroundActions.Mailing
{
    using System;
    using System.Collections.Generic;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Mail;
    using ClubbyBook.UI;

    public sealed class SendResetPasswordMailAction : MailingAction
    {
        private string userEmail;
        private string userName;
        private string resetPassword;

        public SendResetPasswordMailAction(User user, string resetPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            this.userName = UIUtilities.ValidateStringValue(UIUtilities.GetUserFullName(user));
            this.userEmail = user.EMail;
            this.resetPassword = resetPassword;
        }

        protected override IEnumerable<SendItem> CreateSendItems()
        {
            var template = MailTemplatesFactory.CreateResetPasswordTemplate(userName, resetPassword);
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