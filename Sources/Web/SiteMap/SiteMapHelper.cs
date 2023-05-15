namespace ClubbyBook.Web.SiteMap
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Web.Utilities;

    public static class SiteMapHelper
    {
        public static string LastModifiedDateKey = "LastModifiedDateKey";
        public static string PriorityKey = "PriorityKey";
        public static string ChangeFrequentlyKey = "ChangeFrequentlyKey";

        private static Dictionary<char, string> entityEscaping = new Dictionary<char, string>() {
          {'&', "&amp;"},
          {'\'', "&apos;"},
          {'"', "&quot;"},
          {'>', "&gt;"},
          {'<', "&lt;"}
        };

        public static string ValidateUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            StringBuilder sb = new StringBuilder("http://clubbybook.com");

            foreach (char ch in url)
            {
                if (entityEscaping.ContainsKey(ch))
                    sb.Append(entityEscaping[ch]);
                else
                    sb.Append(ch);
            }

            return sb.ToString().ToLower();
        }

        public static string GetPriority(RedirectDirection pageType)
        {
            double priority = 0.0;

            switch (pageType)
            {
                case RedirectDirection.ViewBook:
                    priority = 0.8;
                    break;

                case RedirectDirection.ViewAuthor:
                    priority = 0.7;
                    break;

                case RedirectDirection.ViewNews:
                    priority = 0.6;
                    break;

                case RedirectDirection.SiteMap:
                    priority = 0.5;
                    break;

                case RedirectDirection.About:
                case RedirectDirection.Contacts:
                    priority = 0.4;
                    break;

                case RedirectDirection.Home:
                case RedirectDirection.BookList:
                case RedirectDirection.AuthorList:
                case RedirectDirection.NewsList:
                    priority = 0.2;
                    break;
                default:
                    break;
            }

            return priority.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetChangeFrequently(ChangeFrequently changeFrequently)
        {
            return AttributeHelper.GetEnumValueDescription(changeFrequently);
        }

        public static string GetLastModifiedDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}