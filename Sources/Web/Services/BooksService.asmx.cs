namespace ClubbyBook.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.BackgroundActions;
    using ClubbyBook.BackgroundActions.Mailing;
    using ClubbyBook.Business;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Utilities;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class BooksService : System.Web.Services.WebService
    {
        private const int defaultPageBooksCount = 20;

        #region Autocomplete

        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetAutoCompleteBooksAndAuthors(string prefixText)
        {
            List<SimpleListItem> resultList = new List<SimpleListItem>();

            foreach (BookAuthor ba in ControllerFactory.BooksController.GetAutoCompleteList(prefixText))
                resultList.Add(new SimpleListItem(string.Format("{0} ({1})", ba.Book.Title, ba.Author.FullName), ba.BookId));

            return resultList;
        }

        #endregion Autocomplete

        #region Get Lists Public Members

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetBooks(IList<SearchItem> searchParameters)
        {
            BooksSearchInfo searchInfo;

            if (ParseSearchParameters(searchParameters, out searchInfo))
                return GetBooksInternal(searchInfo);

            return GetBooksInternal(new BooksSearchInfo());
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<CommentItem> GetBookComments(IList<SearchItem> searchParameters)
        {
            CommentSearchInfo searchInfo;

            if (ParseSearchParameters(searchParameters, out searchInfo))
                return GetBookCommentsInternal(searchInfo);

            return new ListResponse<CommentItem>();
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetRecommendedBooks(int userId)
        {
            User user = ControllerFactory.UsersController.Load(userId);
            if (user == null)
                throw new ArgumentNullException("userId");

            if (!ControllerFactory.UsersController.IsUserInRole(user, UserManagement.AccountRoleName) &&
              !ControllerFactory.UsersController.IsUserInRole(user, UserManagement.EditorRoleName) &&
              !ControllerFactory.UsersController.IsUserInRole(user, UserManagement.AdminRoleName))
                throw new UnauthorizedAccessException();

            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetRecommendedBooks(user, defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetMostPopularBooks()
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetMostPopularBooks(defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetMostPopularBooksOnSale()
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetMostPopularBooksOnSale(defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetMostPopularBooksInExchange()
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetMostPopularBooksInExchange(defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetMostPopularBooksWillGiveRead()
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetMostPopularBooksWillGiveRead(defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetMostPopularBooksWillGrantGratis()
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetMostPopularBooksWillGrantGratis(defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetLatestAddedBooks()
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.items = new List<BookItem>();
            response.ids = new List<int>();
            response.moreItems = false;
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;

            foreach (Book book in ControllerFactory.BooksController.GetLatestAddedBooks(defaultPageBooksCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            return response;
        }

        #endregion Get Lists Public Members

        #region Get Utility Information Public Members

        [WebMethod]
        [ScriptMethod]
        public BookItem GetBookInfo(int bookId)
        {
            Book book = ControllerFactory.BooksController.Load(bookId);
            if (book == null)
                return null;

            return CreateBookItem(book);
        }

        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetGenres()
        {
            List<SimpleListItem> genres = new List<SimpleListItem>();
            foreach (var genreTreeItem in ControllerFactory.GenresController.GetGenresTreeViewList())
                genres.Add(new SimpleListItem(genreTreeItem.Description, genreTreeItem.Value));

            return genres;
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<BookItem> GetBooksInfo(IList<int> booksIds)
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;
            response.moreItems = false;
            response.ids = new List<int>();
            response.items = new List<BookItem>();

            foreach (int bookId in booksIds)
            {
                BookItem bi = GetBookInfo(bookId);
                if (bi != null)
                {
                    response.items.Add(bi);
                    response.ids.Add(bookId);
                }
            }

            return response;
        }

        #endregion Get Utility Information Public Members

        #region Action Public Members

        [WebMethod]
        [ScriptMethod]
        public int AddUserUnprovenBook(int userId, string title, string authors, string comment, bool markAsRead)
        {
            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            // TODO: Trim and validate strings

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null && !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(authors))
            {
                Book addedBook = ControllerFactory.BooksController.AddUserUnprovenBook(user, title, authors, comment, markAsRead);
                if (addedBook == null)
                    throw new InvalidOperationException("The unexpected error was thrown while adding the book.");

                string message = string.Format("Пользователь добавил книгу \"{0}\" ({1})", addedBook.Title, addedBook.Description);
                ControllerFactory.NotificationsController.AddNewBookNotification(user, addedBook, message);

                BackgroundActionManager.Instance.ExecuteAction(new SendAddBookNotificationMailAction(user, string.Format("{0} ({1})", addedBook.Title, addedBook.Description)));

                return addedBook.Id;
            }

            return -1;
        }

        [WebMethod]
        [ScriptMethod]
        public AddBookResultType AddBookToLibrary(int userId, int bookId)
        {
            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
            {
                Book book = ControllerFactory.BooksController.Load(bookId);
                if (book != null)
                {
                    if (!ControllerFactory.BooksController.ContainsBookInUserLibrary(user, book))
                    {
                        ControllerFactory.BooksController.AddBookToUserLibrary(user, book);
                        return AddBookResultType.OK;
                    }
                    else
                        return AddBookResultType.AlreadyExists;
                }
            }

            return AddBookResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveBookFromLibrary(int userId, int bookId)
        {
            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
            {
                Book book = ControllerFactory.BooksController.Load(bookId);
                if (book != null)
                {
                    ControllerFactory.BooksController.RemoveBookFromUserLibrary(user, book);
                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType ChangeUserBookStatus(int userId, int bookId, int status)
        {
            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
            {
                Book book = ControllerFactory.BooksController.Load(bookId);
                if (book != null)
                {
                    ControllerFactory.BooksController.ChangeUserBookStatus(user, book, (UserBookStatusType)status);
                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType ChangeUserBookOffer(int userId, int bookId, int offer)
        {
            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
            {
                Book book = ControllerFactory.BooksController.Load(bookId);
                if (book != null)
                {
                    UserBookOfferType action = UserBookOfferType.None;

                    if ((offer & (int)UserBookOfferType.Sell) == (int)UserBookOfferType.Sell)
                        action |= UserBookOfferType.Sell;
                    if ((offer & (int)UserBookOfferType.Buy) == (int)UserBookOfferType.Buy)
                        action |= UserBookOfferType.Buy;
                    if ((offer & (int)UserBookOfferType.Barter) == (int)UserBookOfferType.Barter)
                        action |= UserBookOfferType.Barter;
                    if ((offer & (int)UserBookOfferType.WillGiveRead) == (int)UserBookOfferType.WillGiveRead)
                        action |= UserBookOfferType.WillGiveRead;
                    if ((offer & (int)UserBookOfferType.WillGrantGratis) == (int)UserBookOfferType.WillGrantGratis)
                        action |= UserBookOfferType.WillGrantGratis;

                    ControllerFactory.BooksController.ChangeUserBookOffer(user, book, action);

                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType ChangeUserBookType(int userId, int bookId, int type)
        {
            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
            {
                Book book = ControllerFactory.BooksController.Load(bookId);
                if (book != null)
                {
                    UserBookType bookType = UserBookType.None;

                    if ((type & (int)UserBookType.PaperBook) == (int)UserBookType.PaperBook)
                        bookType |= UserBookType.PaperBook;
                    if ((type & (int)UserBookType.EBook) == (int)UserBookType.EBook)
                        bookType |= UserBookType.EBook;
                    if ((type & (int)UserBookType.Audiobook) == (int)UserBookType.Audiobook)
                        bookType |= UserBookType.Audiobook;

                    ControllerFactory.BooksController.ChangeUserBookType(user, book, bookType);

                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType ChangeUserBookComment(int userId, int bookId, string comment)
        {
            // TODO: validate strings and trim it

            if (!UserManagement.IsAuthenticated || UserManagement.CurrentUser.Id != userId)
                throw new UnauthorizedAccessException();

            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
            {
                Book book = ControllerFactory.BooksController.Load(bookId);
                if (book != null)
                {
                    ControllerFactory.BooksController.ChangeUserBookComment(user, book, comment);
                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public bool ContainsBookInLibrary(int userId, int bookId)
        {
            User user = ControllerFactory.UsersController.Load(userId);
            if (user != null)
                return ContainsBookInUserLibrary(user, ControllerFactory.BooksController.Load(bookId));

            return false;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveBook(int bookId)
        {
            if (!AccessManagement.CanRemoveBook)
                throw new UnauthorizedAccessException();

            Book book = ControllerFactory.BooksController.Load(bookId);
            if (book != null)
            {
                ControllerFactory.BooksController.Delete(book);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public CommentItem AddBookComment(int bookId, string comment)
        {
            // TODO: validate strings and trim it

            if (!AccessManagement.CanAddBookComment)
                throw new UnauthorizedAccessException();

            Book book = ControllerFactory.BooksController.Load(bookId);
            if (book != null && !string.IsNullOrEmpty(comment))
                return CreateCommentItem(book, ControllerFactory.BooksController.AddBookComment(UserManagement.CurrentUser, book, comment));

            return null;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveBookComment(int bookId, int commentId)
        {
            Book book = ControllerFactory.BooksController.Load(bookId);
            if (book != null)
            {
                Comment comment = book.Comments.Where(c => c.Id == commentId).FirstOrDefault();
                if (comment != null)
                {
                    if (!AccessManagement.CanRemoveBookComment(comment))
                        throw new UnauthorizedAccessException();

                    ControllerFactory.BooksController.RemoveBookComment(book, comment);
                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        #endregion Action Public Members

        #region Private Methods

        private bool ParseSearchParameters(IList<SearchItem> searchParameters, out BooksSearchInfo info)
        {
            info = new BooksSearchInfo();

            try
            {
                foreach (var item in searchParameters)
                {
                    switch (item.paramName.ToLower())
                    {
                        case "existentid":
                            info.ExistentIds.Add((int)item.paramValue);
                            break;

                        case "searchtext":
                            info.SearchText = item.paramValue as string;
                            break;

                        case "booktype":
                            if ((UserBookType)item.paramValue != UserBookType.None)
                                info.BookType = (UserBookType)item.paramValue;
                            break;

                        case "offer":
                            if ((UserBookOfferType)item.paramValue != UserBookOfferType.None)
                                info.Offer = (UserBookOfferType)item.paramValue;
                            break;

                        case "status":
                            if ((UserBookStatusType)item.paramValue != UserBookStatusType.None)
                                info.Status = (UserBookStatusType)item.paramValue;
                            break;

                        case "userid":
                            info.BookProgress = BookProgressType.All;
                            info.UserId = (int)item.paramValue;
                            break;

                        case "cityid":
                            if (info.CitiesIds == null)
                                info.CitiesIds = new List<int>();
                            info.CitiesIds.Add((int)item.paramValue);
                            break;

                        case "authorid":
                            if (info.AuthorsIds == null)
                                info.AuthorsIds = new List<int>();
                            info.AuthorsIds.Add((int)item.paramValue);
                            break;

                        case "genreid":
                            info.GenreId = (int)item.paramValue;
                            break;

                        case "bookprogress":
                            info.BookProgress = (BookProgressType)item.paramValue;
                            break;

                        case "bookcollectionsornot":
                            info.BookCollectionsOrNot = (BookCollectionsOrNotType)item.paramValue;
                            break;

                        case "returnall":
                            if ((int)item.paramValue == 1)
                                info.ReturnCount = int.MaxValue;
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private bool ParseSearchParameters(IList<SearchItem> searchParameters, out CommentSearchInfo info)
        {
            info = new CommentSearchInfo();

            try
            {
                foreach (var item in searchParameters)
                {
                    switch (item.paramName.ToLower())
                    {
                        case "existentid":
                            info.ExistentIds.Add((int)item.paramValue);
                            break;

                        case "bookid":
                            info.BookId = (int)item.paramValue;
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private ListResponse<BookItem> GetBooksInternal(BooksSearchInfo searchInfo)
        {
            ListResponse<BookItem> response = new ListResponse<BookItem>();
            response.searchParametersString = PrepareSearchParametersString(searchInfo);
            response.items = new List<BookItem>();
            response.ids = new List<int>();

            int totalCount = 0;
            foreach (Book book in ControllerFactory.BooksController.Search(searchInfo, out totalCount))
            {
                response.items.Add(CreateBookItem(book));
                response.ids.Add(book.Id);
            }

            response.totalItemCountString = PrepareTotalItemCountString(totalCount);
            response.moreItems = searchInfo.ReturnCount != int.MaxValue ? response.items.Count == searchInfo.ReturnCount : false;

            return response;
        }

        private ListResponse<CommentItem> GetBookCommentsInternal(CommentSearchInfo searchInfo)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            if (!searchInfo.BookId.HasValue)
                throw new ArgumentNullException("searchInfo.BookId");

            ListResponse<CommentItem> response = new ListResponse<CommentItem>();
            response.items = new List<CommentItem>();
            response.ids = new List<int>();

            Book book = ControllerFactory.BooksController.Load(searchInfo.BookId.Value);
            if (book == null)
                throw new ArgumentException(string.Format("The book entity {0} doesn't exist!", searchInfo.BookId.Value));

            int totalCount = 0;
            foreach (Comment comment in ControllerFactory.BooksController.GetBookComments(searchInfo, out totalCount))
            {
                response.items.Add(CreateCommentItem(book, comment));
                response.ids.Add(comment.Id);
            }

            response.totalItemCountString = string.Empty; // TODO: if it is necessary
            response.moreItems = searchInfo.ReturnCount != int.MaxValue ? response.items.Count == searchInfo.ReturnCount : false;

            return response;
        }

        private string PrepareSearchParametersString(BooksSearchInfo searchInfo)
        {
            string csps = "Параметры поиска: ";

            List<string> parameters = new List<string>();

            if (searchInfo.CitiesIds != null)
            {
                List<string> citiesNames = new List<string>();
                foreach (var cityId in searchInfo.CitiesIds)
                {
                    City city = ControllerFactory.CitiesController.Load(cityId);
                    if (city != null)
                        citiesNames.Add(city.Name);
                }

                if (citiesNames.Count > 0)
                    parameters.Add(string.Format("город{0} - {1}", citiesNames.Count > 1 ? "а" : string.Empty,
                      string.Join(", ", citiesNames.ToArray())));
            }

            if (searchInfo.GenreId.HasValue)
            {
                Genre genre = ControllerFactory.GenresController.Load(searchInfo.GenreId.Value);
                if (genre != null)
                    parameters.Add(string.Format("жанр - {0}", genre.Name));
            }

            if (searchInfo.UserId.HasValue)
            {
                User user = ControllerFactory.UsersController.Load(searchInfo.UserId.Value);
                if (user != null)
                {
                    string fullName = UIUtilities.GetUserFullName(user);
                    if (!string.IsNullOrEmpty(fullName))
                        parameters.Add(string.Format("пользователь - {0}", fullName));
                }
            }

            if (searchInfo.AuthorsIds != null)
            {
                List<string> authorNames = new List<string>();
                foreach (var authorId in searchInfo.AuthorsIds)
                {
                    Author author = ControllerFactory.AuthorsController.Load(authorId);
                    if (author != null)
                        authorNames.Add(author.FullName);
                }

                if (authorNames.Count > 0)
                    parameters.Add(string.Format("автор{0} - {1}", authorNames.Count > 1 ? "ы" : string.Empty,
                      string.Join(", ", authorNames.ToArray())));
            }

            switch (searchInfo.BookCollectionsOrNot)
            {
                case BookCollectionsOrNotType.CollectionsOnly:
                    parameters.Add("только сборники");
                    break;

                case BookCollectionsOrNotType.ExcludeCollections:
                    parameters.Add("исключить сборники");
                    break;

                case BookCollectionsOrNotType.DoesnotMatter:
                default:
                    break;
            }

            List<string> offerWishes = new List<string>();
            List<string> bookTypeWishes = new List<string>();

            if (searchInfo.Offer.HasValue)
            {
                if (((int)searchInfo.Offer & (int)UserBookOfferType.Buy) == (int)UserBookOfferType.Buy)
                    offerWishes.Add("продать");
                if (((int)searchInfo.Offer & (int)UserBookOfferType.Sell) == (int)UserBookOfferType.Sell)
                    offerWishes.Add("купить");
                if (((int)searchInfo.Offer & (int)UserBookOfferType.Barter) == (int)UserBookOfferType.Barter)
                    offerWishes.Add("обменять");
                if (((int)searchInfo.Offer & (int)UserBookOfferType.WillGiveRead) == (int)UserBookOfferType.WillGiveRead)
                    offerWishes.Add("взять прочесть");
                if (((int)searchInfo.Offer & (int)UserBookOfferType.WillGrantGratis) == (int)UserBookOfferType.WillGrantGratis)
                    offerWishes.Add("получить подарок");
            }

            if (searchInfo.BookType.HasValue)
            {
                if (((int)searchInfo.BookType & (int)UserBookType.PaperBook) == (int)UserBookType.PaperBook)
                    bookTypeWishes.Add("печатную книгу");
                if (((int)searchInfo.BookType & (int)UserBookType.EBook) == (int)UserBookType.EBook)
                    bookTypeWishes.Add("электронную книгу");
                if (((int)searchInfo.BookType & (int)UserBookType.Audiobook) == (int)UserBookType.Audiobook)
                    bookTypeWishes.Add("аудиокнигу");
            }

            string bookTypeWishesString = string.Join(" или ", bookTypeWishes.ToArray());
            string offerWishesString = string.Join(" или ", offerWishes.ToArray());

            if (offerWishes.Count > 0 && bookTypeWishes.Count > 0)
                parameters.Add(string.Format("хочу - {0} {1}", offerWishesString, bookTypeWishesString));
            else if (offerWishes.Count > 0 && bookTypeWishes.Count == 0)
                parameters.Add(string.Format("хочу - {0}", offerWishesString));
            else if (offerWishes.Count == 0 && bookTypeWishes.Count > 0)
                parameters.Add(string.Format("хочу - {0}", bookTypeWishesString));

            csps += string.Join("; ", parameters.ToArray());
            csps += ".";

            return parameters.Count == 0 ? string.Empty : csps;
        }

        private string PrepareTotalItemCountString(int totalCount)
        {
            if (totalCount <= 0)
                return string.Empty;

            var totalCountToCheck = totalCount % 100;

            if (totalCountToCheck >= 5 && totalCountToCheck <= 20)
                return string.Format("В результате поиска найдено {0} книг.", totalCount);
            else
            {
                totalCountToCheck = totalCountToCheck % 10;

                if (totalCountToCheck == 1)
                    return string.Format("В результате поиска найдена {0} книга.", totalCount);
                else if (totalCountToCheck >= 2 && totalCountToCheck <= 4)
                    return string.Format("В результате поиска найдено {0} книги.", totalCount);
                else
                    return string.Format("В результате поиска найдено {0} книг.", totalCount);
            }
        }

        private UserBook GetUserBook(User user, Book book)
        {
            if (user == null || book == null)
                return null;

            return ControllerFactory.BooksController.GetUserBook(user, book);
        }

        private bool ContainsBookInUserLibrary(User user, Book book)
        {
            if (user == null || book == null)
                return false;

            return ControllerFactory.BooksController.ContainsBookInUserLibrary(user, book);
        }

        private bool ContainsBookInOtherUserLibraryFromSameCity(User user, Book book)
        {
            if (user == null || book == null)
                return false;

            return ControllerFactory.BooksController.GetUserCountFromSameCityByUserBook(user, book) > 0;
        }

        private OfferItem CreateOfferItem(User user, Book book, UserBookOfferType offer)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            int partialCount = 0;
            int totalCount = 0;
            string totalLink = string.Empty;
            string partialLink = string.Empty;

            totalCount = ControllerFactory.BooksController.GetBookCountByOffer(book, user, offer, false);
            if (totalCount == 0)
                return new OfferItem(0, string.Empty, 0, string.Empty);
            totalLink = RedirectHelper.ResolveUrl(RedirectDirection.UserList, book.UrlRewrite, AttributeHelper.GetEnumValueUrlRewrite(offer));

            if (user != null)
            {
                partialCount = ControllerFactory.BooksController.GetBookCountByOffer(book, user, offer, true);
                if (partialCount > 0)
                {
                    Profile profile = ControllerFactory.ProfilesController.GetProfile(user);
                    if (profile == null)
                        throw new InvalidOperationException("Profile of the user should exists.");
                    if (profile.City == null)
                        throw new InvalidOperationException("City of the profile should exists.");

                    partialLink = RedirectHelper.ResolveUrl(RedirectDirection.UserList, profile.City.UrlRewrite, book.UrlRewrite,
                      AttributeHelper.GetEnumValueUrlRewrite(offer));
                }
            }

            return new OfferItem(partialCount, partialLink, totalCount, totalLink);
        }

        private BookTypeItem CreateBookTypeItem(User user, Book book, UserBookType type)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            int partialCount = 0;
            int totalCount = 0;
            string totalLink = string.Empty;
            string partialLink = string.Empty;

            totalCount = ControllerFactory.BooksController.GetBookCountByType(book, user, type, false);
            if (totalCount == 0)
                return new BookTypeItem(0, string.Empty, 0, string.Empty);
            totalLink = RedirectHelper.ResolveUrl(RedirectDirection.UserList, book.UrlRewrite, AttributeHelper.GetEnumValueUrlRewrite(type));

            if (user != null)
            {
                partialCount = ControllerFactory.BooksController.GetBookCountByType(book, user, type, true);
                if (partialCount > 0)
                {
                    Profile profile = ControllerFactory.ProfilesController.GetProfile(user);
                    if (profile == null)
                        throw new InvalidOperationException("Profile of the user should exists.");
                    if (profile.City == null)
                        throw new InvalidOperationException("City of the profile should exists.");

                    partialLink = RedirectHelper.ResolveUrl(RedirectDirection.UserList, profile.City.UrlRewrite, book.UrlRewrite,
                      AttributeHelper.GetEnumValueUrlRewrite(type));
                }
            }

            return new BookTypeItem(partialCount, partialLink, totalCount, totalLink);
        }

        private BookItem CreateBookItem(Book book)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            UserBook userBook = GetUserBook(UserManagement.CurrentUser, book);

            BookItem bi = new BookItem();

            bi.id = book.Id;
            bi.title = book.Title;
            if (!book.Confirmed)
                bi.title += " (На проверке...)";
            bi.originalTitle = book.OriginalTitle;
            bi.description = book.Confirmed ? book.Description : string.Format("Книга \"{0}\" не проверена.", book.Title);
            bi.restrictedDescription = book.Confirmed ? UIUtilities.GetRestrictedBookDescription(book) : string.Format("Книга \"{0}\" не проверена.", book.Title);
            bi.confirmed = book.Confirmed;
            bi.collection = book.Collection;
            bi.seoImageAlt = UIUtilities.GetBookImageAltForSEO(book.Title);
            bi.coverPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(book.CoverPath, Settings.EmptyBookCoverPath));
            bi.viewBookLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewBook, book.UrlRewrite);
            bi.editBookLink = RedirectHelper.ResolveUrl(RedirectDirection.EditBook, book.UrlRewrite);
            bi.containsInUserLibrary = ContainsBookInUserLibrary(UserManagement.CurrentUser, book);
            bi.containsInOtherUserLibraryFromSameCity = ContainsBookInOtherUserLibraryFromSameCity(UserManagement.CurrentUser, book);
            if (userBook != null)
            {
                bi.status = (int)userBook.Status;
                bi.offer = (int)userBook.Offer;
                bi.type = (int)userBook.BookType;
                bi.userComment = userBook.Comment;
            }

            bi.paperBookType = CreateBookTypeItem(UserManagement.CurrentUser, book, UserBookType.PaperBook);
            bi.eBookType = CreateBookTypeItem(UserManagement.CurrentUser, book, UserBookType.EBook);
            bi.audiobookType = CreateBookTypeItem(UserManagement.CurrentUser, book, UserBookType.Audiobook);

            bi.anyOffer = CreateOfferItem(UserManagement.CurrentUser, book, UserBookOfferType.None);
            bi.sellOffer = CreateOfferItem(UserManagement.CurrentUser, book, UserBookOfferType.Sell);
            bi.buyOffer = CreateOfferItem(UserManagement.CurrentUser, book, UserBookOfferType.Buy);
            bi.barterOffer = CreateOfferItem(UserManagement.CurrentUser, book, UserBookOfferType.Barter);
            bi.willGiveReadOffer = CreateOfferItem(UserManagement.CurrentUser, book, UserBookOfferType.WillGiveRead);
            bi.willGrantGratisOffer = CreateOfferItem(UserManagement.CurrentUser, book, UserBookOfferType.WillGrantGratis);

            bi.authors = new List<AuthorItem>();
            if (book.Confirmed)
            {
                foreach (BookAuthor bookAuthor in book.BookAuthors)
                {
                    bi.authors.Add(new AuthorItem()
                    {
                        id = bookAuthor.AuthorId,
                        fullName = bookAuthor.Author.FullName,
                        viewAuthorLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewAuthor, bookAuthor.Author.UrlRewrite)
                    });
                }
            }
            else
            {
                bi.authors.Add(new AuthorItem()
                {
                    fullName = book.Description
                });
            }

            bi.authorsAndTitle = string.Format("{0} - {1}", string.Join(", ", bi.authors.Select(a => a.fullName).ToArray()), bi.title);

            bi.genres = new List<GenreItem>();
            if (book.Confirmed)
            {
                foreach (BookGenre bookGenre in book.BookGenres)
                {
                    bi.genres.Add(new GenreItem()
                    {
                        id = bookGenre.GenreId,
                        name = bookGenre.Genre.Name,
                        viewGenreBooksLink = RedirectHelper.ResolveUrl(RedirectDirection.BookList, bookGenre.Genre.UrlRewrite)
                    });
                }
            }

            return bi;
        }

        private CommentItem CreateCommentItem(Book book, Comment comment)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (comment == null)
                throw new ArgumentNullException("comment");

            CommentItem ci = new CommentItem();
            ci.id = comment.Id;
            ci.message = UIUtilities.PrepareTextContent(comment.Message);

            Profile userProfile = ControllerFactory.ProfilesController.GetProfile(comment.User);
            if (userProfile != null)
            {
                ci.userFullName = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(userProfile));
                ci.viewUserLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewProfile, userProfile.UrlRewrite);
                ci.userPhotoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(userProfile.ImagePath, Settings.EmptyProfileAvatarPath));
            }
            ci.seoImageAlt = string.Format("Комментарии к книге {0} | Отзывы о книге {0}", book.Title);
            ci.isUserComment = UserManagement.IsAdminAuthenticated || userProfile.User == UserManagement.CurrentUser;
            ci.createdDate = UIUtilities.GetFullDateString(comment.CreatedDate);
            return ci;
        }

        #endregion Private Methods
    }
}