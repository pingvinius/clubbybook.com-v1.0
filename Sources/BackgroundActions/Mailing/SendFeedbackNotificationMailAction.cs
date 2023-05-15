namespace ClubbyBook.BackgroundActions.Mailing
{
    using System.Collections.Generic;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Mail;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;

    public sealed class SendFeedbackNotificationMailAction : MailingAction
    {
        private string fromUserName;
        private string fromUserEmail;
        private string message;

        public SendFeedbackNotificationMailAction(User fromUser, string message)
        {
            if (fromUser != null)
            {
                this.fromUserName = UIUtilities.ValidateStringValue(UIUtilities.GetUserFullName(fromUser));
                this.fromUserEmail = fromUser.EMail;
            }

            this.message = message;
        }

        protected override IEnumerable<SendItem> CreateSendItems()
        {
            List<SendItem> sendItems = new List<SendItem>();
            foreach (var adminUser in ControllerFactory.UsersController.GetUsersByRoles(UserManagement.AdminRoleName))
            {
                var template = MailTemplatesFactory.CreateFeedbackTemplate(this.fromUserName, this.fromUserEmail, this.message);
                sendItems.Add(new SendItem()
                {
                    To = adminUser.EMail,
                    Subject = template.Subject,
                    Body = template.Body
                });
            }
            return sendItems;
        }
    }
}