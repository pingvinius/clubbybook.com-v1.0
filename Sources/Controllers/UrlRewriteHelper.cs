namespace ClubbyBook.Controllers
{
    using System;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Utilities;

    public static class UrlRewriteHelper
    {
        private delegate bool IsAlreadyExistDelegate(string value);

        public static void ApplyUrlRewrite(Book book)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            string alias = string.Empty;

            if (book.Confirmed)
            {
                alias = book.Title.Latinize().ToLower();
                alias = PreventDublicate(alias, new IsAlreadyExistDelegate(delegate(string value)
                {
                    return ContextManager.Current.Books.Count(entity => entity.Id != book.Id &&
                      value.Equals(entity.UrlRewrite, StringComparison.OrdinalIgnoreCase)) > 0;
                }));
            }
            else
            {
                alias = book.Title.GetHashCode().ToString("x").Latinize().ToLower();
                alias = PreventDublicate(alias, new IsAlreadyExistDelegate(delegate(string value)
                {
                    return ContextManager.Current.Books.Count(entity => entity.Id != book.Id &&
                      value.Equals(entity.UrlRewrite, StringComparison.OrdinalIgnoreCase)) > 0;
                }));
            }

            if (book.UrlRewrite != alias)
                book.UrlRewrite = alias;
        }

        public static void ApplyUrlRewrite(Author author)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            string alias = author.FullName.Latinize().ToLower();
            alias = PreventDublicate(alias, new IsAlreadyExistDelegate(delegate(string value)
            {
                return ContextManager.Current.Authors.Count(entity => entity.Id != author.Id &&
                  value.Equals(entity.UrlRewrite, StringComparison.OrdinalIgnoreCase)) > 0;
            }));

            if (author.UrlRewrite != alias)
                author.UrlRewrite = alias;
        }

        public static void ApplyUrlRewrite(Profile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            string profileName = profile.User.EMail;
            profileName = profileName.Substring(0, profileName.IndexOf('@'));
            profileName = profileName.Replace('.', '-');
            profileName = profileName.Replace('_', '-');

            string alias = profileName.Latinize().ToLower();
            alias = PreventDublicate(alias, new IsAlreadyExistDelegate(delegate(string value)
            {
                return ContextManager.Current.Profiles.Count(entity => entity.Id != profile.Id &&
                  value.Equals(entity.UrlRewrite, StringComparison.OrdinalIgnoreCase)) > 0;
            }));

            if (profile.UrlRewrite != alias)
                profile.UrlRewrite = alias;
        }

        public static void ApplyUrlRewrite(News news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            string alias = news.Title.Latinize().ToLower();
            alias = PreventDublicate(alias, new IsAlreadyExistDelegate(delegate(string value)
            {
                return ContextManager.Current.News.Count(entity => entity.Id != news.Id &&
                  value.Equals(entity.UrlRewrite, StringComparison.OrdinalIgnoreCase)) > 0;
            }));

            if (news.UrlRewrite != alias)
                news.UrlRewrite = alias;
        }

        private static string PreventDublicate(string original, IsAlreadyExistDelegate isAlreadyExist)
        {
            if (string.IsNullOrEmpty(original))
                throw new ArgumentNullException("original");

            if (isAlreadyExist == null)
                throw new ArgumentNullException("isAlreadyExist");

            int index = 1;
            string value = original;

            while (isAlreadyExist(value))
                value = string.Format("{0}-{1}", original, index++);

            return value;
        }
    }
}