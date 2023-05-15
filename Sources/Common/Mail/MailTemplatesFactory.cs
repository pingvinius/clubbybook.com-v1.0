namespace ClubbyBook.Common.Mail
{
    using System;
    using System.IO;

    public static class MailTemplatesFactory
    {
        private static string mailTemplatesPath;

        public static void Initialize(string templatesPath)
        {
            if (string.IsNullOrEmpty(templatesPath))
                throw new ArgumentNullException("templatesPath");

            mailTemplatesPath = templatesPath;
        }

        public static MailTemplate CreateRegistrationTemplate(string userEmail, string userPassword)
        {
            return new RegistrationMailTemplate(ValidatePath(Settings.RegistrationTemplateFileName),
              userEmail, userPassword);
        }

        public static MailTemplate CreateConversationTemplate(string userName, string senderUserName, string message)
        {
            return new ConversationNotificationMailTemplate(ValidatePath(Settings.ConversationNotificationTemplateFileName),
              userName, senderUserName, message);
        }

        public static MailTemplate CreateFeedbackTemplate(string userName, string userEmail, string message)
        {
            return new FeedbackNotificationMailTemplate(ValidatePath(Settings.FeedbackNotificationTemplateFileName),
              userName, userEmail, message);
        }

        public static MailTemplate CreateAddBookNotificationTemplate(string userName, string userEmail, string bookInfo)
        {
            return new AddBookSystemNotificationMailTemplate(ValidatePath(Settings.AddBookSystemNotificationTemplateFileName),
              userName, userEmail, bookInfo);
        }

        public static MailTemplate CreatePasswordWasChangedTemplate(string userName, string userPassword)
        {
            return new PasswordWasChangedMailTemplate(ValidatePath(Settings.PasswordWasChangedTemplateFileName),
              userName, userPassword);
        }

        public static MailTemplate CreateResetPasswordTemplate(string userName, string userPassword)
        {
            return new ResetPasswordMailTemplate(ValidatePath(Settings.ResetPasswordTemplateFileName),
              userName, userPassword);
        }

        private static string ValidatePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (string.IsNullOrEmpty(mailTemplatesPath))
                throw new InvalidOperationException("The MailTemplatesFactory should be initialized.");

            return Path.Combine(mailTemplatesPath, fileName);
        }
    }
}