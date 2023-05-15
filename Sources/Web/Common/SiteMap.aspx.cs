namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Common;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.SiteMap;
    using ClubbyBook.Web.Utilities;

    public partial class SiteMap : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Карта сайта социальной сети любителей книг", UIUtilities.SiteBrandName);
            }
        }

        #region Page Events

        protected void btnUpdateSiteMap_Click(object sender, EventArgs e)
        {
            SiteMapGenerator.SaveSiteMapToXml(Server.MapPath(Settings.SiteMapFilePath));

            RedirectHelper.Redirect(RedirectDirection.SiteMap);
        }

        protected void btnUpdateBooksLastModifiedDate_Click(object sender, EventArgs e)
        {
            ControllerFactory.BooksController.UpdateBooksLastModifiedDate();

            SiteMapGenerator.SaveSiteMapToXml(Server.MapPath(Settings.SiteMapFilePath));

            RedirectHelper.Redirect(RedirectDirection.SiteMap);
        }

        protected void btnUpdateAuthorsLastModifiedDate_Click(object sender, EventArgs e)
        {
            ControllerFactory.AuthorsController.UpdateAuthorsLastModifiedDate();

            SiteMapGenerator.SaveSiteMapToXml(Server.MapPath(Settings.SiteMapFilePath));

            RedirectHelper.Redirect(RedirectDirection.SiteMap);
        }

        #endregion Page Events
    }
}