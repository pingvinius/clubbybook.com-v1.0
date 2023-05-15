namespace ClubbyBook.Common.Mail
{
    internal sealed class FeedbackNotificationMailTemplate : MailTemplate
    {
        public FeedbackNotificationMailTemplate(string filePath, string userName, string userEmail, string message)
            : base(filePath)
        {
            if (string.IsNullOrEmpty(userName))
                userName = "Анонимус";

            if (string.IsNullOrEmpty(userEmail))
                userEmail = "почта не указана";

            Parameters.Add("@UserName", userName);
            Parameters.Add("@UserEmail", userEmail);
            Parameters.Add("@Message", message);
        }
    }
}