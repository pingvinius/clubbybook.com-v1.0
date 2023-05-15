namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class News : EntityListPage<ClubbyBook.Business.News, NewsController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Книжные новости и события | Новости книжного мира", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccessManagement.CanAddNews)
                contentHeader.Actions.Add(new HeaderAction("Добавить", RedirectHelper.ResolveUrl(RedirectDirection.EditNews)));
        }
    }
}