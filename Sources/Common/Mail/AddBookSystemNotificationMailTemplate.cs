namespace ClubbyBook.Common.Mail
{
    using System;

    internal sealed class AddBookSystemNotificationMailTemplate : MailTemplate
    {
        public AddBookSystemNotificationMailTemplate(string filePath, string userName, string userEmail, string bookInfo)
            : base(filePath)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentNullException("userEmail");

            if (string.IsNullOrEmpty(bookInfo))
                throw new ArgumentNullException("bookInfo");

            Parameters.Add("@UserName", userName);
            Parameters.Add("@UserEmail", userEmail);
            Parameters.Add("@BookInfo", bookInfo);
        }
    }
}