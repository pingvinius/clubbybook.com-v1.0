namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class Default : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Обменять, продать, купить книги - социальная сеть любителей книг", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            topNewsContentHeader.Actions.Add(new HeaderAction("Смотреть все", RedirectHelper.ResolveUrl(RedirectDirection.NewsList)));
        }
    }
}