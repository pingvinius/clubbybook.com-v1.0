namespace ClubbyBook.Web.Services
{
    using System;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class RatingService : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod]
        public int GetCommonAuthorRating(int id)
        {
            Author author = ControllerFactory.AuthorsController.Load(id);
            return author != null ? (int)RatesController.GetAuthorRating(author) : 0;
        }

        [WebMethod]
        [ScriptMethod]
        public int GetUserAuthorRating(int id)
        {
            CheckServiceUsing();

            Author author = ControllerFactory.AuthorsController.Load(id);
            return author != null ? (int)RatesController.GetAuthorRating(author, UserManagement.CurrentUser) : 0;
        }

        [WebMethod]
        [ScriptMethod]
        public void SetUserAuthorRating(int id, int newValue)
        {
            CheckServiceUsing();

            Author author = ControllerFactory.AuthorsController.Load(id);
            if (author != null)
                RatesController.SetAuthorRating(author, UserManagement.CurrentUser, newValue);
        }

        [WebMethod]
        [ScriptMethod]
        public int GetCommonBookRating(int id)
        {
            Book book = ControllerFactory.BooksController.Load(id);
            return book != null ? (int)RatesController.GetBookRating(book) : 0;
        }

        [WebMethod]
        [ScriptMethod]
        public int GetUserBookRating(int id)
        {
            CheckServiceUsing();

            Book book = ControllerFactory.BooksController.Load(id);
            return book != null ? (int)RatesController.GetBookRating(book, UserManagement.CurrentUser) : 0;
        }

        [WebMethod]
        [ScriptMethod]
        public void SetUserBookRating(int id, int newValue)
        {
            CheckServiceUsing();

            Book book = ControllerFactory.BooksController.Load(id);
            if (book != null)
                RatesController.SetBookRating(book, UserManagement.CurrentUser, newValue);
        }

        private void CheckServiceUsing()
        {
            if (!UserManagement.IsAuthenticated)
                throw new UnauthorizedAccessException();
        }
    }
}