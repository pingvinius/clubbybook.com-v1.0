namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class Authors : EntityListPage<Author, AuthorsController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Обменять, продать, купить книги авторов | Каталог авторов", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccessManagement.CanAddAuthor)
                contentHeader.Actions.Add(new HeaderAction("Добавить", RedirectHelper.ResolveUrl(RedirectDirection.EditAuthor)));
        }
    }
}