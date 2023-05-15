namespace ClubbyBook.Web.Common
{
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using System;

    public partial class Maintenance : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Социальная сеть любителей книг временно недоступна.", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.StatusCode = 503;
            Response.StatusDescription = "HTTP/1.1 503 Service Temporarily Unavailable";
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Retry-After", "Sat, 11 May 2013 23:00:00 GMT"); 
            //Response.Flush();
        }
    }
}