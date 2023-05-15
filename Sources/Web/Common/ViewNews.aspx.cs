namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class ViewNews : EntityViewPage<ClubbyBook.Business.News, NewsController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Новость {1}", UIUtilities.SiteBrandName, Entity.Title);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            // Adjust header title
            contentHeader.Title = Entity.Title;

            if (AccessManagement.CanEditNews)
                contentHeader.Actions.Add(new HeaderAction("Редактировать", RedirectHelper.ResolveUrl(RedirectDirection.EditNews, Entity.UrlRewrite)));
        }
    }
}