namespace ClubbyBook.BackgroundActions.Mailing
{
    using System;
    using System.Collections.Generic;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Mail;
    using ClubbyBook.UI;

    public sealed class SendConversationNotificationMailAction : MailingAction
    {
        private string fromUserName;
        private string toUserName;
        private string toUserEmail;
        private string message;

        public SendConversationNotificationMailAction(User fromUser, User toUser, string message)
        {
            if (fromUser == null)
            {
                throw new ArgumentNullException("fromUser");
            }

            if (toUser == null)
            {
                throw new ArgumentNullException("toUser");
            }

            this.fromUserName = UIUtilities.ValidateStringValue(UIUtilities.GetUserFullName(fromUser));
            this.toUserName = UIUtilities.ValidateStringValue(UIUtilities.GetUserFullName(toUser));
            this.toUserEmail = toUser.EMail;
            this.message = message;
        }

        protected override IEnumerable<SendItem> CreateSendItems()
        {
            var template = MailTemplatesFactory.CreateConversationTemplate(this.toUserName, this.fromUserName, this.message);
            return new List<SendItem>()
            {
                new SendItem()
                {
                    To = this.toUserEmail,
                    Subject = template.Subject,
                    Body = template.Body
                }
            };
        }
    }
}