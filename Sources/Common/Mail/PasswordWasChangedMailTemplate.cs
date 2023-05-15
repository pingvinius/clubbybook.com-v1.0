namespace ClubbyBook.Common.Mail
{
    using System;

    internal sealed class PasswordWasChangedMailTemplate : MailTemplate
    {
        public PasswordWasChangedMailTemplate(string filePath, string userName, string userPassword)
            : base(filePath)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            if (string.IsNullOrEmpty(userPassword))
                throw new ArgumentNullException("userPassword");

            Parameters.Add("@UserName", userName);
            Parameters.Add("@UserPassword", userPassword);
        }
    }
}