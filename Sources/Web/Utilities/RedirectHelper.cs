namespace ClubbyBook.Web.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using ClubbyBook.Common.Logging;

    public enum RedirectDirection
    {
        Home,
        Error,
        Login,
        Logout,
        Registration,
        About,
        Contacts,
        SiteMap,
        BookList,
        ViewBook,
        EditBook,
        AuthorList,
        ViewAuthor,
        EditAuthor,
        UserList,
        ViewProfile,
        EditProfile,
        EditAccount,
        NewsList,
        ViewNews,
        EditNews,
        Notifications,
        ResolveDuplicateBook,
        ResetPassword,
        UserAgreement,
        EditorTools,
        PrePostValidation,
    }

    public static class RedirectHelper
    {
        private static Dictionary<RedirectDirection, string> pathDict = new Dictionary<RedirectDirection, string>() {
      {RedirectDirection.Home, "~/"},
      {RedirectDirection.Error, "~/error"},
      {RedirectDirection.Login, "~/login"},
      {RedirectDirection.Logout, "~/logout"},
      {RedirectDirection.Registration, "~/registration"},
      {RedirectDirection.About, "~/about"},
      {RedirectDirection.Contacts, "~/contacts"},
      {RedirectDirection.SiteMap, "~/sitemap"},
      {RedirectDirection.BookList, "~/books"},
      {RedirectDirection.ViewBook, "~/viewbook"},
      {RedirectDirection.EditBook, "~/editbook"},
      {RedirectDirection.AuthorList, "~/authors"},
      {RedirectDirection.ViewAuthor, "~/viewauthor"},
      {RedirectDirection.EditAuthor, "~/editauthor"},
      {RedirectDirection.UserList, "~/users"},
      {RedirectDirection.ViewProfile, "~/viewprofile"},
      {RedirectDirection.EditProfile, "~/editprofile"},
      {RedirectDirection.EditAccount, "~/editaccount"},
      {RedirectDirection.NewsList, "~/news"},
      {RedirectDirection.ViewNews, "~/viewnews"},
      {RedirectDirection.EditNews, "~/editnews"},
      {RedirectDirection.Notifications, "~/notifications"},
      {RedirectDirection.ResolveDuplicateBook, "~/resolveduplicatebook"},
      {RedirectDirection.ResetPassword, "~/resetpassword"},
      {RedirectDirection.UserAgreement, "~/useragreement"},
      {RedirectDirection.EditorTools, "~/editortools"},
      {RedirectDirection.PrePostValidation, "~/prepostvalidation"},
    };

        public static string ResolveUrl(RedirectDirection direction, params object[] values)
        {
            return ResolveUrl(direction, false, values);
        }

        public static string ResolveUrl(RedirectDirection direction, bool includeReturnUrl, params object[] values)
        {
            if (pathDict.ContainsKey(direction))
            {
                StringBuilder sbUrl = new StringBuilder(pathDict[direction]);
                if (sbUrl.Length == 0)
                    throw new InvalidOperationException("The url should exist for known direction.");

                if (values != null)
                {
                    foreach (var val in values)
                    {
                        if (val != null)
                        {
                            if (sbUrl[sbUrl.Length - 1] != '/')
                                sbUrl.Append('/');

                            sbUrl.Append(val.ToString().ToLower());
                        }
                    }
                }

                string urlToRedirect = sbUrl.ToString();

                try
                {
                    if (includeReturnUrl && HttpContext.Current.Request != null)
                    {
                        if (!urlToRedirect.EndsWith("/"))
                            urlToRedirect += "/";

                        string returnUrl = HttpContext.Current.Request.RawUrl;
                        if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params.Get("ReturnUrl")))
                            returnUrl = HttpUtility.UrlDecode(HttpContext.Current.Request.Params.Get("ReturnUrl"));
                        urlToRedirect += string.Format("?ReturnUrl={0}", HttpUtility.UrlEncode(returnUrl.ToLower()));
                    }

                    return VirtualPathUtility.ToAbsolute(urlToRedirect);
                }
                catch (Exception ex)
                {
                    Logger.Write(string.Format("An unexpected error has been detected. (Url: {0})", urlToRedirect));
                    Logger.Write(ex);
                }
            }
            else
                Logger.Write(string.Format("There is no appropriate rule to resolve url. (Direction: {0})", direction));

            return string.Empty;
        }

        public static bool Redirect(RedirectDirection direction)
        {
            return Redirect(direction, false, new object[0]);
        }

        public static bool Redirect(RedirectDirection direction, params object[] values)
        {
            return Redirect(direction, false, values);
        }

        public static bool Redirect(RedirectDirection direction, bool includeReturnUrl, params object[] values)
        {
            return RedirectDirectly(ResolveUrl(direction, includeReturnUrl, values));
        }

        public static bool RedirectDirectly(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (HttpContext.Current.Response != null)
            {
                HttpContext.Current.Response.Redirect(url, true);
                return true;
            }

            return false;
        }
    }
}