namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class Logout : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Выйти из социальной сети любителей книг", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UserManagement.Logout();

            GoToReturnUrl(RedirectDirection.Home);
        }
    }
}