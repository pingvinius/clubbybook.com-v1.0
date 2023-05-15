namespace ClubbyBook.Common
{
    using System.IO;
    using System.Web.Configuration;

    public static class Settings
    {
        public static string ConnectionString
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            }
        }

        public static string SimpleConnectionString
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["SimpleMySqlConnection"].ConnectionString;
            }
        }

        public static bool IsUnderMaintenance
        {
            get
            {
                bool r = false;
                return bool.TryParse(WebConfigurationManager.AppSettings["Maintenance"], out r) && r;
            }
        }

        public static string LoggerFilePath
        {
            get
            {
                return "~/Logs/web.log";
            }
        }

        public static string SqlLoggerFilePath
        {
            get
            {
                return "~/Logs/sql.log";
            }
        }

        public static string SiteMapFilePath
        {
            get
            {
                return "~/sitemap.xml";
            }
        }

        #region Global images paths

        public static string ImagesTempPath
        {
            get
            {
                return "~/Temp/Images/";
            }
        }

        public static string ImagesProfilesPath
        {
            get
            {
                return "~/Images/Profiles/";
            }
        }

        public static string ImagesBooksPath
        {
            get
            {
                return "~/Images/Books/";
            }
        }

        public static string ImagesAuthorsPath
        {
            get
            {
                return "~/Images/Authors/";
            }
        }

        #endregion Global images paths

        #region File name templates

        public static string ProfileAvatarFileName
        {
            get
            {
                return "Avatar{0}.png";
            }
        }

        public static string BookCoverFileName
        {
            get
            {
                return "Cover{0}.png";
            }
        }

        public static string AuthorPhotoFileName
        {
            get
            {
                return "Photo{0}.png";
            }
        }

        #endregion File name templates

        #region Default entity images paths

        public static string EmptyBookCoverPath
        {
            get
            {
                return Path.Combine(ImagesBooksPath, "EmptyCover.png");
            }
        }

        public static string EmptyProfileAvatarPath
        {
            get
            {
                return Path.Combine(ImagesProfilesPath, "EmptyAvatar.png");
            }
        }

        public static string AnonymousProfileAvatarPath
        {
            get
            {
                return Path.Combine(ImagesProfilesPath, "AnonymousAvatar.png");
            }
        }

        public static string EmptyAuthorPhotoPath
        {
            get
            {
                return Path.Combine(ImagesAuthorsPath, "EmptyPhoto.png");
            }
        }

        #endregion Default entity images paths

        #region Mail Templates

        public static string MailTemplatesPath
        {
            get
            {
                return "~/MailTemplates/";
            }
        }

        public static string RegistrationTemplateFileName
        {
            get
            {
                return "Registration.bcmt";
            }
        }

        public static string ConversationNotificationTemplateFileName
        {
            get
            {
                return "ConversationNotification.bcmt";
            }
        }

        public static string FeedbackNotificationTemplateFileName
        {
            get
            {
                return "FeedbackNotification.bcmt";
            }
        }

        public static string AddBookSystemNotificationTemplateFileName
        {
            get
            {
                return "AddBookSystemNotification.bcmt";
            }
        }

        public static string PasswordWasChangedTemplateFileName
        {
            get
            {
                return "PasswordWasChanged.bcmt";
            }
        }

        public static string ResetPasswordTemplateFileName
        {
            get
            {
                return "ResetPassword.bcmt";
            }
        }

        #endregion Mail Templates
    }
}