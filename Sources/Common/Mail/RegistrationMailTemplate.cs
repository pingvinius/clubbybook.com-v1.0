namespace ClubbyBook.Common.Mail
{
    using System;

    internal sealed class RegistrationMailTemplate : MailTemplate
    {
        public RegistrationMailTemplate(string filePath, string userEmail, string userPassword)
            : base(filePath)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentNullException("userEmail");

            if (string.IsNullOrEmpty(userPassword))
                throw new ArgumentNullException("userPassword");

            Parameters.Add("@UserEmail", userEmail);
            Parameters.Add("@UserPassword", userPassword);
        }
    }
}