namespace ClubbyBook.BackgroundActions.Mailing
{
    using System;
    using System.Collections.Generic;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Mail;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;

    public sealed class SendAddBookNotificationMailAction : MailingAction
    {
        private string userEmail;
        private string userName;
        private string bookInfo;

        public SendAddBookNotificationMailAction(User user, string bookInfo)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            this.userName = UIUtilities.ValidateStringValue(UIUtilities.GetUserFullName(user));
            this.userEmail = user.EMail;
            this.bookInfo = bookInfo;
        }

        protected override IEnumerable<SendItem> CreateSendItems()
        {
            List<SendItem> sendItems = new List<SendItem>();
            foreach (var editorUser in ControllerFactory.UsersController.GetUsersByRoles(UserManagement.EditorRoleName, UserManagement.AdminRoleName))
            {
                var template = MailTemplatesFactory.CreateAddBookNotificationTemplate(this.userName, this.userEmail, this.bookInfo);
                sendItems.Add(new SendItem()
                {
                    To = editorUser.EMail,
                    Subject = template.Subject,
                    Body = template.Body
                });
            }
            return sendItems;
        }
    }
}