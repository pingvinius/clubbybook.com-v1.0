namespace ClubbyBook.Common.Mail
{
    using System;

    internal sealed class ConversationNotificationMailTemplate : MailTemplate
    {
        public ConversationNotificationMailTemplate(string filePath, string userName, string senderUserName, string message)
            : base(filePath)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrEmpty(senderUserName))
                throw new ArgumentNullException("senderUserName");

            Parameters.Add("@UserName", userName);
            Parameters.Add("@SenderUserName", senderUserName);
            Parameters.Add("@Message", message);
        }
    }
}