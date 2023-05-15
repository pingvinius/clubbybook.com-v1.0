namespace ClubbyBook.Web.Utilities
{
    using System;
    using System.Web;
    using ClubbyBook.Business;

    internal sealed class WebContextHolder : IContextHolder
    {
        private const string currentSessionKey = "context_session_key";

        #region IContextHolder Members

        public bool IsAvailable()
        {
            return HttpContext.Current != null;
        }

        public bool Contains()
        {
            return HttpContext.Current.Items.Contains(WebContextHolder.currentSessionKey);
        }

        public object Get()
        {
            if (HttpContext.Current.Items.Contains(WebContextHolder.currentSessionKey))
            {
                return HttpContext.Current.Items[WebContextHolder.currentSessionKey];
            }

            return null;
        }

        public void Set(object context)
        {
            if (HttpContext.Current.Items.Contains(WebContextHolder.currentSessionKey))
            {
                throw new InvalidOperationException("The context already exists in the context holder.");
            }

            HttpContext.Current.Items.Add(WebContextHolder.currentSessionKey, context);
        }

        public void Remove()
        {
            if (HttpContext.Current.Items.Contains(WebContextHolder.currentSessionKey))
            {
                HttpContext.Current.Items.Remove(WebContextHolder.currentSessionKey);
            }
        }

        #endregion IContextHolder Members
    }
}