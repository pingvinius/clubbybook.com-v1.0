namespace ClubbyBook.Web
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;
    using ClubbyBook.Business;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.Mail;
    using ClubbyBook.Web.Utilities;

    public class Global : System.Web.HttpApplication
    {
        public static System.Timers.Timer MyKillTimer = new System.Timers.Timer();

        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.Write("ClubbyBook started an initialization.");

            try
            {
                // Initialize logs
                bool allowTrace = false;
#if DEBUG
                allowTrace = true;
                Logger.AttachLog(LogType.Sql, Server.MapPath(Settings.SqlLoggerFilePath), allowTrace);
#endif
                Logger.AttachLog(LogType.General, Server.MapPath(Settings.LoggerFilePath), allowTrace);

                // Initialize contexts
                ContextManager.Initialize(new WebContextHolder());

                // Initialize mail templates
                MailTemplatesFactory.Initialize(Server.MapPath(Settings.MailTemplatesPath));

                // Times to kill sleep connections
                try
                {
                    MyKillTimer.Interval = 120000; // check sleeping connections every 120 seconds
                    MyKillTimer.Elapsed += new System.Timers.ElapsedEventHandler(MyKillTimer_Event);
                    MyKillTimer.AutoReset = true;
                    MyKillTimer.Enabled = true;
                }
                catch
                {
                }

                Logger.Write("ClubbyBook has been started.");
            }
            catch (Exception ex)
            {
                Logger.Write("While starting ClubbyBook application unknown exception has occurred.", ex);
                throw;
            }
        }

        private void MyKillTimer_Event(object source, System.Timers.ElapsedEventArgs e)
        {
            ContextManager.KillSleepingConnections(30);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            ContextManager.Dispose();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity.GetType() == typeof(FormsIdentity))
                    {
                        FormsIdentity fi = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket fat = fi.Ticket;

                        string[] roles = fat.UserData.Split(';');
                        HttpContext.Current.User = new GenericPrincipal(fi, roles);
                    }
                }
            }
        }

        private void Page_Error(object sender, System.EventArgs e)
        {
            Exception exception = base.Server.GetLastError();
            Logger.Write("An error has occurred on current page.", exception);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (base.Context.AllErrors.Length == 1)
            {
                Logger.Write("An error has occurred while performing current operation.");
            }
            else if (base.Context.AllErrors.Length > 1)
            {
                Logger.Write(string.Format("The {0} errors have occurred while performing current operation.", Context.AllErrors.Length));
            }

            foreach (var exception in base.Context.AllErrors)
            {
                Logger.Write(exception);
            }

            base.Server.ClearError();
        }

        protected void Global_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Logger.Write("A global error has occurred.", exception);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.Write("ClubbyBook started stopping...");

            ContextManager.Dispose();

            Logger.Write("ClubbyBook has been stopped.");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }
    }
}