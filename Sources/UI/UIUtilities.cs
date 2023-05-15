namespace ClubbyBook.UI
{
    using System;
    using System.Text;
    using System.Web;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Controllers;

    public static class UIUtilities
    {
        public const string NotSpecifiedString = "Не указано";
        public const string NotSpecifiedMaleProfileFullNameString = "Незнакомец";
        public const string NotSpecifiedFemaleProfileFullNameString = "Незнакомка";
        public const string SelectString = "Выбрать";
        private const int BookDescriptionMaxLength = 170;
        private const int AuthorDescriptionMaxLength = 170;
        private const int NewsDescriptionMaxLength = 300;

        public static string SiteBrandName
        {
            get
            {
                return "ClubbyBook";
            }
        }

        public static string GetBookImageAltForSEO(string bookTitle)
        {
            return string.Format("Купить {0} | Продать {0} | Обменять {0}", bookTitle);
        }

        public static string GetAuthorImageAltForSEO(string authorFullName)
        {
            return string.Format("Купить книгу автора {0} | Продать книгу автора {0} | Обменять книгу автора {0}", authorFullName);
        }

        public static string GetProfileImageAltForSEO(string profileName)
        {
            return string.Format("Профайл пользователя {0}", profileName);
        }

        public static string ValidateStringValue(object val)
        {
            return string.IsNullOrEmpty(val as string) ? NotSpecifiedString : val as string;
        }

        public static string ValidateImagePath(object path, object defaultPath)
        {
            string defaultValue = string.IsNullOrEmpty(defaultPath as string) ? string.Empty : defaultPath as string;
            return string.IsNullOrEmpty(path as string) ? defaultValue : path as string;
        }

        public static string GetFullDateString(DateTime dt)
        {
            return dt.ToString("f");
        }

        public static string GetShortDateString(DateTime dt)
        {
            return dt.ToString("g");
        }

        public static string GetRestrictedBookDescription(object objBook)
        {
            if (objBook == null)
                throw new ArgumentNullException("objBook");

            if (!(objBook is Book))
                throw new InvalidCastException("Book");

            return GetRestrictedString((objBook as Book).Description, BookDescriptionMaxLength);
        }

        public static string GetRestrictedAuthorDescription(object objAuthor)
        {
            if (objAuthor == null)
                throw new ArgumentNullException("objAuthor");

            if (!(objAuthor is Author))
                throw new InvalidCastException("Author");

            return GetRestrictedString((objAuthor as Author).ShortDescription, AuthorDescriptionMaxLength);
        }

        public static string GetRestrictedNewsMessage(object objNews)
        {
            if (objNews == null)
                throw new ArgumentNullException("objNews");

            if (!(objNews is News))
                throw new InvalidCastException("News");

            return GetRestrictedString((objNews as News).Message, NewsDescriptionMaxLength);
        }

        public static string PrepareTextContent(string text)
        {
            if (text == null)
                text = string.Empty;

            text = HttpUtility.HtmlEncode(text);

            StringBuilder sb = new StringBuilder();

            sb.Append("<p>");

            bool lastCharIsNewLine = false;
            for (int i = 0; i < text.Length; i++)
            {
                var curr = text[i];

                if (curr == '\r')
                    continue;
                else if (curr == '\n')
                {
                    if (lastCharIsNewLine)
                        sb.Append("<br />");

                    sb.Append("</p>");

                    lastCharIsNewLine = true;
                }
                else
                {
                    if (lastCharIsNewLine)
                        sb.Append("<p>");
                    lastCharIsNewLine = false;
                    sb.Append(curr);
                }
            }

            sb.Append("</p>");

            return sb.ToString();
        }

        public static string GetAuthorLifeYearsString(GenderType gender, int? birthdayYear, int? deathYear)
        {
            const string maleBirthText = "родился в {0} г.";
            const string femaleBirthText = "родилась в {0} г.";

            const string maleDeathText = "умер в {0} г.";
            const string femaleDeathText = "умерла в {0} г.";

            if (birthdayYear.HasValue && deathYear.HasValue)
                return string.Format("{0} - {1} гг.", birthdayYear.Value, deathYear.Value);
            else if (birthdayYear.HasValue && !deathYear.HasValue)
                return string.Format(gender != GenderType.Female ? maleBirthText : femaleBirthText, birthdayYear.Value);
            else if (!birthdayYear.HasValue && deathYear.HasValue)
                return string.Format(gender != GenderType.Female ? maleDeathText : femaleDeathText, deathYear.Value);

            return NotSpecifiedString;
        }

        public static string GetProfileFullName(Profile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            if (!string.IsNullOrEmpty(profile.Name) && !string.IsNullOrEmpty(profile.Surname))
                return string.Format("{0} {1}", profile.Name, profile.Surname);
            else if (string.IsNullOrEmpty(profile.Name) && string.IsNullOrEmpty(profile.Surname) && string.IsNullOrEmpty(profile.Nickname))
                return profile.Gender == GenderType.Female ? NotSpecifiedFemaleProfileFullNameString : NotSpecifiedMaleProfileFullNameString;
            else if (string.IsNullOrEmpty(profile.Name) && string.IsNullOrEmpty(profile.Surname) && !string.IsNullOrEmpty(profile.Nickname))
                return profile.Nickname;
            else if (string.IsNullOrEmpty(profile.Name))
                return profile.Surname;
            else if (string.IsNullOrEmpty(profile.Surname))
                return profile.Name;

            return string.Empty;
        }

        public static string GetUserFullName(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return GetProfileFullName(ControllerFactory.ProfilesController.GetProfile(user));
        }

        private static string GetRestrictedString(string fullString, int restrictCount)
        {
            if (fullString == null)
            {
                return string.Empty;
            }

            if (restrictCount >= fullString.Length)
            {
                return fullString;
            }

            return string.Format("{0} ...", fullString.Substring(0, restrictCount));
        }
    }
}