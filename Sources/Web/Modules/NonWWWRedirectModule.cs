namespace ClubbyBook.Web.Modules
{
    using System;
    using System.Web;

    public class NonWWWRedirectModule : IHttpModule
    {
        private void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                if (context.Response.StatusCode != 301)
                {
                    if (context.Request.Url.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Status = "301 Moved Permanently";
                        context.Response.AddHeader("Location", context.Request.Url.AbsoluteUri.Replace("/www.", "/"));
                        return;
                    }
                }
            }
        }

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Context_BeginRequest);
        }

        #endregion IHttpModule Members
    }
}