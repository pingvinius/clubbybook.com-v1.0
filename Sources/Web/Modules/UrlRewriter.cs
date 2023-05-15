namespace ClubbyBook.Web.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.Common;

    public class UrlRewriter : IHttpModule
    {
        private static string ErrorUrlFormatString = "~/Common/Error.aspx?code={0}";
        private static string ErrorUrl = string.Format(ErrorUrlFormatString, (int)ErrorCodeType.InvalidUrlRewrite);
        private static string MaintenanceUrl = "~/Common/Maintenance.aspx";
        private static Dictionary<string, IRewriteUnit> rules = new Dictionary<string, IRewriteUnit>();

        static UrlRewriter()
        {
            #region General Rules

            rules.Add("about", new SimpleRewriteUnit("~/Common/About.aspx"));
            rules.Add("sitemap", new SimpleRewriteUnit("~/Common/SiteMap.aspx"));
            rules.Add("contacts", new SimpleRewriteUnit("~/Common/Contacts.aspx"));
            rules.Add("useragreement", new SimpleRewriteUnit("~/Common/UserAgreement.aspx"));
            rules.Add("registration", new SimpleRewriteUnit("~/Common/Registration.aspx"));
            rules.Add("login", new SimpleRewriteUnit("~/Common/Login.aspx"));
            rules.Add("resetpassword", new SimpleRewriteUnit("~/Common/ResetPassword.aspx"));
            rules.Add("logout", new SimpleRewriteUnit("~/Common/Logout.aspx"));
            rules.Add("notifications", new SimpleRewriteUnit("~/Account/Notifications.aspx"));
            rules.Add("editortools", new SimpleRewriteUnit("~/Editor/EditorTools.aspx"));
            rules.Add("prepostvalidation", new SimpleRewriteUnit("~/Editor/PrePostValidation.aspx"));
            rules.Add("resolveduplicatebook", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                Book book = ControllerFactory.BooksController.FindByUrlRewrite(parameters[0]);
                if (book != null)
                    return string.Format("~/Editor/ResolveDuplicateBook.aspx?id={0}", book.Id);
                return string.Empty;
            }));
            rules.Add("error", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                var errorCodeValue = AttributeHelper.FindEnumValueByUrlRewrite<ErrorCodeType>(parameters[0]);
                if (errorCodeValue != null)
                    return string.Format(ErrorUrlFormatString, (int)errorCodeValue);
                return string.Empty;
            }));

            #endregion General Rules

            #region Book Entity Rules

            rules.Add("viewbook", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                Book book = ControllerFactory.BooksController.FindByUrlRewrite(parameters[0]);
                if (book != null)
                    return string.Format("~/Common/ViewBook.aspx?id={0}", book.Id);
                return string.Empty;
            }));
            rules.Add("editbook", new CustomRewriteUnit(count => count == 0 || count == 1, delegate(string[] parameters)
            {
                string newPath = "~/Editor/EditBook.aspx";

                if (parameters.Length == 1)
                {
                    Book book = ControllerFactory.BooksController.FindByUrlRewrite(parameters[0]);
                    if (book != null)
                        newPath += string.Format("?id={0}", book.Id);
                }

                return newPath;
            }));
            rules.Add("books", new CustomRewriteUnit(count => count >= 0, delegate(string[] parameters)
            {
                List<string> searchParameters = new List<string>();

                if (parameters.Length > 0)
                {
                    Genre genre = ControllerFactory.GenresController.FindByUrlRewrite(parameters[0]);
                    if (genre != null)
                        searchParameters.Add(string.Format("genreId={0}", genre.Id));

                    Profile profile = ControllerFactory.ProfilesController.FindByUrlRewrite(parameters[0]);
                    if (profile != null)
                        searchParameters.Add(string.Format("userId={0}", profile.UserId));

                    Author author = ControllerFactory.AuthorsController.FindByUrlRewrite(parameters[0]);
                    if (author != null)
                        searchParameters.Add(string.Format("authorId={0}", author.Id));
                }

                string newPath = "~/Common/Books.aspx";
                if (searchParameters.Count > 0)
                    newPath += "?" + string.Join("&", searchParameters);

                return newPath;
            }));

            #endregion Book Entity Rules

            #region Author Entity Rules

            rules.Add("viewauthor", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                Author author = ControllerFactory.AuthorsController.FindByUrlRewrite(parameters[0]);
                if (author != null)
                    return string.Format("~/Common/ViewAuthor.aspx?id={0}", author.Id);
                return string.Empty;
            }));
            rules.Add("editauthor", new CustomRewriteUnit(count => count == 0 || count == 1, delegate(string[] parameters)
            {
                string newPath = "~/Editor/EditAuthor.aspx";

                if (parameters.Length == 1)
                {
                    Author author = ControllerFactory.AuthorsController.FindByUrlRewrite(parameters[0]);
                    if (author != null)
                        newPath += string.Format("?id={0}", author.Id);
                }

                return newPath;
            }));
            rules.Add("authors", new SimpleRewriteUnit("~/Common/Authors.aspx"));

            #endregion Author Entity Rules

            #region Profile Entity Rules

            rules.Add("viewprofile", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                Profile profile = ControllerFactory.ProfilesController.FindByUrlRewrite(parameters[0]);
                if (profile != null)
                    return string.Format("~/Common/ViewProfile.aspx?id={0}", profile.Id);

                return string.Empty;
            }));
            rules.Add("editprofile", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                Profile profile = ControllerFactory.ProfilesController.FindByUrlRewrite(parameters[0]);
                if (profile != null)
                    return string.Format("~/Account/EditProfile.aspx?id={0}", profile.Id);

                return string.Empty;
            }));
            rules.Add("editaccount", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                Profile profile = ControllerFactory.ProfilesController.FindByUrlRewrite(parameters[0]);
                if (profile != null)
                    return string.Format("~/Account/EditAccount.aspx?id={0}", profile.UserId);

                return string.Empty;
            }));
            rules.Add("users", new CustomRewriteUnit(count => count == 0 || count == 2 || count == 3, delegate(string[] parameters)
            {
                List<string> searchParameters = new List<string>();

                if (parameters.Length >= 2)
                {
                    int currentIndex = 0;

                    if (parameters.Length == 3)
                    {
                        City city = ControllerFactory.CitiesController.FindByUrlRewrite(parameters[currentIndex++]);
                        if (city != null)
                            searchParameters.Add(string.Format("cityId={0}", city.Id));
                    }

                    Book book = ControllerFactory.BooksController.FindByUrlRewrite(parameters[currentIndex++]);
                    if (book != null)
                        searchParameters.Add(string.Format("bookId={0}", book.Id));

                    var offer = AttributeHelper.FindEnumValueByUrlRewrite<UserBookOfferType>(parameters[currentIndex]);
                    if (offer != null)
                        searchParameters.Add(string.Format("offer={0}", (int)((UserBookOfferType)offer)));
                    else
                    {
                        var bookType = AttributeHelper.FindEnumValueByUrlRewrite<UserBookType>(parameters[currentIndex]);
                        if (bookType != null)
                            searchParameters.Add(string.Format("bookType={0}", (int)((UserBookType)bookType)));
                    }
                    currentIndex++;
                }

                string newPath = "~/Common/Users.aspx";
                if (searchParameters.Count > 0)
                    newPath += "?" + string.Join("&", searchParameters);

                return newPath;
            }));

            #endregion Profile Entity Rules

            #region News Entity Rules

            rules.Add("viewnews", new CustomRewriteUnit(count => count == 1, delegate(string[] parameters)
            {
                News news = ControllerFactory.NewsController.FindByUrlRewrite(parameters[0]);
                if (news != null)
                    return string.Format("~/Common/ViewNews.aspx?id={0}", news.Id);
                return string.Empty;
            }));
            rules.Add("editnews", new CustomRewriteUnit(count => count == 0 || count == 1, delegate(string[] parameters)
            {
                string newPath = "~/Editor/EditNews.aspx";

                if (parameters.Length == 1)
                {
                    News news = ControllerFactory.NewsController.FindByUrlRewrite(parameters[0]);
                    if (news != null)
                        newPath += string.Format("?id={0}", news.Id);
                }

                return newPath;
            }));
            rules.Add("news", new SimpleRewriteUnit("~/Common/News.aspx"));

            #endregion News Entity Rules
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                try
                {
                    string path = context.Request.Url.AbsolutePath.ToLower();

                    if (path.Contains("scripts/") || path.Contains("styles/") || path.Contains("images/"))
                    {
                        return;
                    }
                    else if (Settings.IsUnderMaintenance)
                    {
                        context.RewritePath(MaintenanceUrl, false);
                        return;
                    }

                    // Parse requested url
                    string[] parts = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length; i++)
                        parts[i] = context.Server.UrlDecode(parts[i]).ToLower();

                    // Redirect to default page
                    if (parts.Length == 0 || parts[0].Equals("default.aspx", StringComparison.OrdinalIgnoreCase))
                    {
                        context.RewritePath("~/Common/Default.aspx", false);
                        return;
                    }

                    string leadingPart = parts[0];
                    if (rules.ContainsKey(leadingPart))
                    {
                        string[] parameterParts = new string[parts.Length - 1];
                        Array.Copy(parts, 1, parameterParts, 0, parts.Length - 1);

                        string redirectToUrl = rules[leadingPart].Rewrite(parameterParts);
                        if (string.IsNullOrEmpty(redirectToUrl))
                            redirectToUrl = ErrorUrl;

                        context.RewritePath(redirectToUrl, false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("An error has occurred while parsing URL rewriting.", ex);

                    context.RewritePath(ErrorUrl, false);
                }
            }
        }

        #region IHttpModule Members

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        public void Dispose()
        {
        }

        #endregion IHttpModule Members

        private interface IRewriteUnit
        {
            string Rewrite(string[] parts);
        }

        private class SimpleRewriteUnit : IRewriteUnit
        {
            private string url;

            public SimpleRewriteUnit(string url)
            {
                this.url = url;
            }

            #region IRewriteUnit Members

            public string Rewrite(string[] parameters)
            {
                return parameters.Length == 0 ? url : string.Empty;
            }

            #endregion IRewriteUnit Members
        }

        private class CustomRewriteUnit : IRewriteUnit
        {
            private readonly Func<int, bool> validate;
            private readonly Func<string[], string> handler;

            public CustomRewriteUnit(Func<string[], string> handler) : this((c) => true, handler) { }

            public CustomRewriteUnit(Func<int, bool> validate, Func<string[], string> handler)
            {
                if (validate == null)
                    throw new ArgumentNullException("validate");

                if (handler == null)
                    throw new ArgumentNullException("convertHandler");

                this.validate = validate;
                this.handler = handler;
            }

            #region IRewriteUnit Members

            public string Rewrite(string[] parameters)
            {
                if (!validate(parameters.Length))
                    return string.Empty;

                return handler(parameters);
            }

            #endregion IRewriteUnit Members
        }
    }
}