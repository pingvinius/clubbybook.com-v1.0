namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class Books : EntityListPage<Book, BooksController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Обменять, продать, купить книги | Каталог книг", UIUtilities.SiteBrandName);
            }
        }

        public int SearchGenreId
        {
            get
            {
                int id = -1;
                if (int.TryParse(Request.Params["genreId"], out id))
                    return id;
                return -1;
            }
        }

        public int SearchAuthorId
        {
            get
            {
                int id = -1;
                if (int.TryParse(Request.Params["authorId"], out id))
                    return id;
                return -1;
            }
        }

        public int SearchUserId
        {
            get
            {
                int id = -1;
                if (int.TryParse(Request.Params["userId"], out id))
                    return id;
                return -1;
            }
        }

        public int SearchCityId
        {
            get
            {
                int id = -1;
                if (int.TryParse(Request.Params["cityId"], out id))
                    return id;
                return -1;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccessManagement.CanAddBook)
                contentHeader.Actions.Add(new HeaderAction("Добавить", RedirectHelper.ResolveUrl(RedirectDirection.EditBook)));
            contentHeader.Actions.Add(new HeaderAction("Расширенный поиск", new ClientOnClickCode("expandAdvancedSearchBlock()")));
            contentHeader.Actions.Add(new HeaderAction("Сбросить фильтрацию", RedirectHelper.ResolveUrl(RedirectDirection.BookList)));
        }
    }
}