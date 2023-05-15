namespace ClubbyBook.Web.Pages
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Web.Utilities;

    public class SimplePage : System.Web.UI.Page
    {
        #region Fields

        private static string[] aspNetFormElements = new string[] {
          "__EVENTTARGET",
          "__EVENTARGUMENT",
          "__VIEWSTATE",
          "__EVENTVALIDATION",
          "__VIEWSTATEENCRYPTED",
        };

        #endregion Fields

        #region Properties

        public string ReturnUrl
        {
            get
            {
                string url = Request["ReturnUrl"] as string;
                if (!string.IsNullOrEmpty(url))
                    return HttpUtility.UrlDecode(url);
                return string.Empty;
            }
        }

        public virtual string PageTitle
        {
            get
            {
                return null;
            }
        }

        #endregion Properties

        #region Overrides Members

        protected override void OnLoad(System.EventArgs e)
        {
            EnsureJavaScriptEnabling();
            EnsureBrowserSupporting();

            if (!string.IsNullOrEmpty(PageTitle))
                Title = PageTitle;

            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

                base.Render(htmlWriter);

                string html = stringWriter.ToString();
                int formStart = html.IndexOf("<form");
                int endForm = -1;
                if (formStart >= 0)
                    endForm = html.IndexOf(">", formStart);

                if (endForm >= 0)
                {
                    StringBuilder viewStateBuilder = new StringBuilder();

                    foreach (string element in aspNetFormElements)
                    {
                        int startPoint = html.IndexOf("<input type=\"hidden\" name=\"" + element + "\"");
                        if (startPoint >= 0 && startPoint > endForm)
                        {
                            int endPoint = html.IndexOf("/>", startPoint);
                            if (endPoint >= 0)
                            {
                                endPoint += 2;
                                string viewStateInput = html.Substring(startPoint, endPoint - startPoint);
                                html = html.Remove(startPoint, endPoint - startPoint);
                                viewStateBuilder.Append(viewStateInput).Append(Environment.NewLine);
                            }
                        }
                    }

                    if (viewStateBuilder.Length > 0)
                    {
                        viewStateBuilder.Insert(0, Environment.NewLine);
                        html = html.Insert(endForm + 1, viewStateBuilder.ToString());
                    }
                }

                writer.Write(html);
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("An error has occurred while rendering page: '{0}'.", this.PageTitle), ex);
                RedirectHelper.Redirect(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.UnexpectedErrorHasOccurred));
            }
        }

        protected override void OnError(EventArgs e)
        {
            base.OnError(e);

            Logger.Write(string.Format("An error has occurred in the page: '{0}'.", this.PageTitle), base.Server.GetLastError());

            RedirectHelper.Redirect(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.UnexpectedErrorHasOccurred));
        }

        #endregion Overrides Members

        #region Public Members

        public void GoToReturnUrl()
        {
            GoToReturnUrl(RedirectDirection.Home);
        }

        public void GoToReturnUrl(RedirectDirection navigateToIfFailed, params object[] parameters)
        {
            if (string.IsNullOrEmpty(ReturnUrl) || !RedirectHelper.RedirectDirectly(ReturnUrl))
            {
                RedirectHelper.Redirect(navigateToIfFailed, parameters);
            }
        }

        #endregion Public Members

        #region Private Members

        private void EnsureJavaScriptEnabling()
        {
            string errorPageUrl = RedirectHelper.ResolveUrl(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.DisabledJavaScript));

            StringBuilder sbJavaScript = new StringBuilder();
            sbJavaScript.AppendLine("<noscript>");
            sbJavaScript.AppendLine(string.Format("<meta http-equiv=\"REFRESH\" content=\"0;URL={0}\">", errorPageUrl));
            sbJavaScript.AppendLine("</noscript>");
            LiteralControl metaObject = new LiteralControl(sbJavaScript.ToString());
            Page.Header.Controls.Add(metaObject);
        }

        private void EnsureBrowserSupporting()
        {
            if (Request.Browser.Browser == "IE" && Request.Browser.MajorVersion <= 9)
                RedirectHelper.Redirect(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.UnsupportedBrowser));
        }

        #endregion Private Members
    }
}