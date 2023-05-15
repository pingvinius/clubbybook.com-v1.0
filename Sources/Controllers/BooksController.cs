namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;
    using LinqKit;

    public sealed class BooksController : IEntityController<Book>, IRewritableController<Book>, IAutoCompletableController<BookAuthor>
    {
        public void ResolveDuplicateBook(Book duplicateBook, Book originalBook)
        {
            if (duplicateBook == null)
                throw new ArgumentNullException("duplicateBook");

            if (originalBook == null)
                throw new ArgumentNullException("originalBook");

            List<UserBook> changeList = new List<UserBook>(duplicateBook.UserBooks);
            List<UserBook> listToRemove = new List<UserBook>();
            foreach (var userBook in changeList)
            {
                if (GetUserBook(userBook.User, originalBook) == null)
                    userBook.Book = originalBook;
                else
                    listToRemove.Add(userBook);
            }

            foreach (var userBook in listToRemove)
                ContextManager.Current.DeleteObject(userBook);
            ContextManager.Current.DeleteObject(duplicateBook);

            ContextManager.Current.SaveChanges();
        }

        #region User Book Routine

        public UserBook GetUserBook(User user, Book book)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            var result = from UserBook userBook in ContextManager.Current.UserBooks
                         where userBook.UserId == user.Id && userBook.BookId == book.Id
                         select userBook;

            return result.FirstOrDefault();
        }

        public int GetUserBookCount(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return ContextManager.Current.UserBooks.Count(userBook => userBook.UserId == user.Id);
        }

        public int GetUserBookCountByStatus(User user, UserBookStatusType status)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return ContextManager.Current.UserBooks.Count(userBook => userBook.UserId == user.Id && (userBook.iStatus & (int)status) != (int)UserBookStatusType.None);
        }

        public void ChangeUserBookStatus(User user, Book book, UserBookStatusType status)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            UserBook userBook = GetUserBook(user, book);
            if (userBook == null)
                throw new InvalidOperationException("userBook cannot be null");

            userBook.Status = status;

            ContextManager.Current.SaveChanges();
        }

        public void ChangeUserBookOffer(User user, Book book, UserBookOfferType offer)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            UserBook userBook = GetUserBook(user, book);
            if (userBook == null)
                throw new InvalidOperationException("userBook cannot be null");

            userBook.Offer = offer;

            ContextManager.Current.SaveChanges();
        }

        public void ChangeUserBookType(User user, Book book, UserBookType type)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            UserBook userBook = GetUserBook(user, book);
            if (userBook == null)
                throw new InvalidOperationException("userBook cannot be null");

            userBook.BookType = type;

            ContextManager.Current.SaveChanges();
        }

        public void ChangeUserBookComment(User user, Book book, string comment)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            UserBook userBook = GetUserBook(user, book);
            if (userBook == null)
                throw new InvalidOperationException("userBook cannot be null");

            userBook.Comment = comment;

            ContextManager.Current.SaveChanges();
        }

        public Book AddUserUnprovenBook(User user, string title, string authors, string comment, bool markAsRead)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");

            if (string.IsNullOrEmpty(authors))
                throw new ArgumentNullException("authors");

            Book book = new Book();
            book.Confirmed = false;
            book.Collection = false;
            book.Title = title;
            book.Description = authors;

            Update(book);

            UserBook userBook = new UserBook();
            userBook.User = user;
            userBook.Book = book;
            userBook.Comment = comment;
            if (markAsRead)
                userBook.Status = UserBookStatusType.AlreadyRead;
            ContextManager.Current.AddToUserBooks(userBook);

            ContextManager.Current.SaveChanges();

            return book;
        }

        public void AddBookToUserLibrary(User user, Book book)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            // don't add user book if it is already exist
            if (GetUserBook(user, book) != null)
                return;

            UserBook userBook = new UserBook();
            userBook.User = user;
            userBook.Book = book;
            ContextManager.Current.AddToUserBooks(userBook);

            ContextManager.Current.SaveChanges();
        }

        public void RemoveBookFromUserLibrary(User user, Book book)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            var result = from UserBook userBook in ContextManager.Current.UserBooks
                         where userBook.User.Id == user.Id && userBook.BookId == book.Id
                         select userBook;

            foreach (var userBook in result)
                ContextManager.Current.UserBooks.DeleteObject(userBook);

            ContextManager.Current.SaveChanges();
        }

        public bool ContainsBookInUserLibrary(User user, Book book)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            return GetUserBook(user, book) != null;
        }

        public int GetUserCountFromSameCityByUserBook(User user, Book book)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            Profile userProfile = ControllerFactory.ProfilesController.GetProfile(user);
            if (userProfile != null && userProfile.City != null)
            {
                var result = from UserBook userBook in ContextManager.Current.UserBooks
                             join profile in ContextManager.Current.Profiles on userBook.UserId equals profile.UserId
                             where userBook.UserId != user.Id && userBook.BookId == book.Id && profile.CityId == userProfile.CityId
                             select userBook;

                return result.ToList<UserBook>().Count;
            }

            return 0;
        }

        public int GetBookCountByOffer(Book book, User user, UserBookOfferType offer, bool onlyFromUserCity)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (user == null && onlyFromUserCity)
                throw new ArgumentNullException("user == null && onlyFromUserCity");

            City userCity = null;
            if (onlyFromUserCity && user != null)
            {
                Profile userProfile = ControllerFactory.ProfilesController.GetProfile(user);
                userCity = userProfile != null ? userProfile.City : null;
                if (userCity == null)
                    return 0;
            }

            int offerNumber = (int)offer;

            var result = from UserBook userBook in ContextManager.Current.UserBooks
                         where userBook.BookId == book.Id
                         select userBook;

            if (offer != UserBookOfferType.None)
            {
                result = from UserBook userBook in result
                         where (userBook.iOffer & offerNumber) == offerNumber
                         select userBook;
            }

            if (user != null)
            {
                result = from UserBook userBook in result
                         where userBook.UserId != user.Id
                         select userBook;

                if (onlyFromUserCity)
                {
                    result = from UserBook userBook in result
                             join profile in ContextManager.Current.Profiles on userBook.UserId equals profile.UserId
                             join city in ContextManager.Current.Cities on profile.CityId equals city.Id
                             where userCity.Id == city.Id
                             select userBook;
                }
            }

            return result.ToList<UserBook>().Count;
        }

        public int GetBookCountByType(Book book, User user, UserBookType type, bool onlyFromUserCity)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (user == null && onlyFromUserCity)
                throw new ArgumentNullException("user == null && onlyFromUserCity");

            City userCity = null;
            if (onlyFromUserCity && user != null)
            {
                Profile userProfile = ControllerFactory.ProfilesController.GetProfile(user);
                userCity = userProfile != null ? userProfile.City : null;
                if (userCity == null)
                    return 0;
            }

            int bookTypeNumber = (int)type;

            var result = from UserBook userBook in ContextManager.Current.UserBooks
                         where userBook.BookId == book.Id && (userBook.iBookType & bookTypeNumber) == bookTypeNumber
                         select userBook;

            if (user != null)
            {
                result = from UserBook userBook in result
                         where userBook.UserId != user.Id
                         select userBook;

                if (onlyFromUserCity)
                {
                    result = from UserBook userBook in result
                             join profile in ContextManager.Current.Profiles on userBook.UserId equals profile.UserId
                             join city in ContextManager.Current.Cities on profile.CityId equals city.Id
                             where userCity.Id == city.Id
                             select userBook;
                }
            }

            return result.ToList<UserBook>().Count;
        }

        #endregion User Book Routine

        #region Book Authors Routine

        public IList<Author> GetBookAuthors(Book book)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            var result = from BookAuthor bookAuthor in ContextManager.Current.BookAuthors
                         where bookAuthor.BookId == book.Id && bookAuthor.Author != null
                         orderby bookAuthor.Author.FullName
                         select bookAuthor.Author;

            return result.ToList<Author>();
        }

        public void SetBookAuthors(Book book, IList<Author> authors)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (authors == null)
                throw new ArgumentNullException("authors");

            if (book.Id != 0)
            { // Remove old book authors, only when it is old entity
                var bookAuthorsToDelete = from BookAuthor bookAuthor in ContextManager.Current.BookAuthors
                                          where bookAuthor.BookId == book.Id
                                          select bookAuthor;
                foreach (var bookAuthor in bookAuthorsToDelete)
                    ContextManager.Current.BookAuthors.DeleteObject(bookAuthor);
            }

            // Add new bok authors
            foreach (var author in authors)
            {
                BookAuthor bookAuthor = new BookAuthor();
                bookAuthor.Book = book;
                bookAuthor.Author = author;
                ContextManager.Current.AddToBookAuthors(bookAuthor);
            }
        }

        #endregion Book Authors Routine

        #region Book Genres Routine

        public void SetBookGenres(Book book, IList<Genre> genres)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (genres == null)
                throw new ArgumentNullException("genres");

            if (book.Id != 0)
            { // Remove old book genres, only when it is old entity
                var bookGenresToDelete = from BookGenre bookGenre in ContextManager.Current.BookGenres
                                         where bookGenre.BookId == book.Id
                                         select bookGenre;
                foreach (var bookGenre in bookGenresToDelete)
                    ContextManager.Current.BookGenres.DeleteObject(bookGenre);
            }

            // Add new book genres
            foreach (var genre in genres)
            {
                BookGenre bookGenre = new BookGenre();
                bookGenre.Book = book;
                bookGenre.Genre = genre;
                ContextManager.Current.AddToBookGenres(bookGenre);
            }
        }

        #endregion Book Genres Routine

        #region Book Comments Routine

        public IList<Comment> GetBookComments(CommentSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            if (!searchInfo.BookId.HasValue)
                throw new ArgumentNullException("searchInfo.BookId");

            Book book = Load(searchInfo.BookId.Value);
            if (book == null)
                throw new ArgumentException(string.Format("The book entity {0} should exist!", searchInfo.BookId.Value));
            ;
            var result = book.Comments.Where(c => !searchInfo.ExistentIds.Contains(c.Id));

            if (searchInfo.ExistentIds.Count == 0)
                totalCount = result.Count();
            else
                totalCount = -1;

            return result
              .OrderByDescending(comment => comment.CreatedDate)
              .Take(searchInfo.ReturnCount)
              .ToList<Comment>();
        }

        public Comment AddBookComment(User user, Book book, string comment)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (book == null)
                throw new ArgumentNullException("book");

            if (string.IsNullOrEmpty(comment))
                throw new ArgumentNullException("comment");

            var newComment = new Comment()
            {
                Message = comment,
                CreatedDate = DateTimeHelper.Now,
                User = user
            };
            book.Comments.Add(newComment);

            ContextManager.Current.SaveChanges();

            return newComment;
        }

        public void RemoveBookComment(Book book, Comment comment)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (comment == null)
                throw new ArgumentNullException("comment");

            if (book.Comments.Contains(comment))
            {
                ContextManager.Current.DeleteObject(comment);
                ContextManager.Current.SaveChanges();
            }
        }

        #endregion Book Comments Routine

        #region Tops Routine

        public IList<Book> GetRecommendedBooks(User user, int count)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var booksOfUserIds = from ub in ContextManager.Current.UserBooks
                                 where ub.User.Id == user.Id
                                 select ub.BookId;

            var recommendedUsersIds = from ub in ContextManager.Current.UserBooks
                                      where ub.User.Id != user.Id && booksOfUserIds.Contains(ub.BookId)
                                      select ub.UserId;

            return (from ub in ContextManager.Current.UserBooks
                    join br in ContextManager.Current.BookRates on ub.BookId equals br.BookId
                    join b in ContextManager.Current.Books on ub.BookId equals b.Id
                    where !booksOfUserIds.Contains(ub.BookId) && recommendedUsersIds.Contains(ub.UserId) &&
                          b.Confirmed && br.Value == 5
                    select b)
              .Distinct()
              .Take(count)
              .ToList();
        }

        public IList<Book> GetMostPopularBooks(int count)
        {
            return (from BookRate bookRate in ContextManager.Current.BookRates
                    where bookRate.Book.Confirmed
                    group bookRate by bookRate.Book into g
                    orderby g.Count() descending, g.Average(br => br.Value) descending
                    select g.Key)
              .Take(count)
              .ToList();
        }

        public IList<Book> GetMostPopularBooksOnSale(int count)
        {
            return (from BookRate bookRate in ContextManager.Current.BookRates
                    join userBook in ContextManager.Current.UserBooks on bookRate.BookId equals userBook.BookId
                    where bookRate.UserId == userBook.UserId && bookRate.Book.Confirmed &&
                          (userBook.iOffer & (int)UserBookOfferType.Sell) == (int)UserBookOfferType.Sell
                    group bookRate by bookRate.Book into g
                    orderby g.Count() descending, g.Average(br => br.Value) descending
                    select g.Key)
              .Take(count)
              .ToList();
        }

        public IList<Book> GetMostPopularBooksInExchange(int count)
        {
            return (from BookRate bookRate in ContextManager.Current.BookRates
                    join userBook in ContextManager.Current.UserBooks on bookRate.BookId equals userBook.BookId
                    where bookRate.UserId == userBook.UserId && bookRate.Book.Confirmed &&
                          (userBook.iOffer & (int)UserBookOfferType.Barter) == (int)UserBookOfferType.Barter
                    group bookRate by bookRate.Book into g
                    orderby g.Count() descending, g.Average(br => br.Value) descending
                    select g.Key)
              .Take(count)
              .ToList();
        }

        public IList<Book> GetMostPopularBooksWillGiveRead(int count)
        {
            return (from BookRate bookRate in ContextManager.Current.BookRates
                    join userBook in ContextManager.Current.UserBooks on bookRate.BookId equals userBook.BookId
                    where bookRate.UserId == userBook.UserId && bookRate.Book.Confirmed &&
                          (userBook.iOffer & (int)UserBookOfferType.WillGiveRead) == (int)UserBookOfferType.WillGiveRead
                    group bookRate by bookRate.Book into g
                    orderby g.Count() descending, g.Average(br => br.Value) descending
                    select g.Key)
              .Take(count)
              .ToList();
        }

        public IList<Book> GetMostPopularBooksWillGrantGratis(int count)
        {
            return (from BookRate bookRate in ContextManager.Current.BookRates
                    join userBook in ContextManager.Current.UserBooks on bookRate.BookId equals userBook.BookId
                    where bookRate.UserId == userBook.UserId && bookRate.Book.Confirmed &&
                          (userBook.iOffer & (int)UserBookOfferType.WillGrantGratis) == (int)UserBookOfferType.WillGrantGratis
                    group bookRate by bookRate.Book into g
                    orderby g.Count() descending, g.Average(br => br.Value) descending
                    select g.Key)
              .Take(count)
              .ToList();
        }

        public IList<Book> GetLatestAddedBooks(int count)
        {
            return (from Book book in ContextManager.Current.Books
                    where book.Confirmed
                    orderby book.CreatedDate descending
                    select book)
              .Take(count)
              .ToList();
        }

        #endregion Tops Routine

        #region Search Routine

        public IList<Book> Search(BooksSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from Book book in ContextManager.Current.Books
                         where !searchInfo.ExistentIds.Contains(book.Id) &&
                               (book.Title.ToLower().Contains(searchInfo.SearchText) ||
                                book.OriginalTitle.ToLower().Contains(searchInfo.SearchText))
                         select book;

            if (searchInfo.BookProgress == BookProgressType.Proven)
            {
                result = from Book book in result
                         where book.Confirmed
                         select book;
            }
            else if (searchInfo.BookProgress == BookProgressType.Unproven)
            {
                result = from Book book in result
                         where !book.Confirmed
                         select book;
            }

            if (searchInfo.BookCollectionsOrNot == BookCollectionsOrNotType.CollectionsOnly)
            {
                result = from Book book in result
                         where book.Collection
                         select book;
            }
            else if (searchInfo.BookCollectionsOrNot == BookCollectionsOrNotType.ExcludeCollections)
            {
                result = from Book book in result
                         where !book.Collection
                         select book;
            }

            if (searchInfo.AuthorsIds != null)
            {
                result = from Book book in result
                         join BookAuthor bookAuthor in ContextManager.Current.BookAuthors on book.Id equals bookAuthor.BookId
                         where searchInfo.AuthorsIds.Contains(bookAuthor.AuthorId)
                         select book;
            }

            if (searchInfo.CitiesIds != null)
            {
                result = from Book book in result
                         join userBook in ContextManager.Current.UserBooks on book.Id equals userBook.BookId
                         join profile in ContextManager.Current.Profiles on userBook.UserId equals profile.UserId
                         join city in ContextManager.Current.Cities on profile.CityId equals city.Id
                         where searchInfo.CitiesIds.Contains(city.Id)
                         select book;
            }

            if (searchInfo.GenreId.HasValue)
            {
                List<int> approachedGenresId = new List<int>();
                var genre = ControllerFactory.GenresController.Load(searchInfo.GenreId.Value);
                if (genre != null)
                {
                    approachedGenresId.Add(genre.Id);
                    foreach (var child in ControllerFactory.GenresController.GetAllGenreChildren(genre))
                        approachedGenresId.Add(child.Id);
                }

                if (approachedGenresId.Count > 0)
                {
                    result = from Book book in result
                             join bookGenre in ContextManager.Current.BookGenres on book.Id equals bookGenre.BookId
                             where approachedGenresId.Contains(bookGenre.GenreId)
                             select book;
                }
            }

            if (searchInfo.Offer.HasValue || searchInfo.BookType.HasValue || searchInfo.Status.HasValue || searchInfo.UserId.HasValue)
            {
                var userBookPredicate = PredicateBuilder.True<UserBook>();

                if (searchInfo.Offer.HasValue)
                    userBookPredicate = userBookPredicate.And(ub => (ub.iOffer & (int)searchInfo.Offer) != (int)UserBookOfferType.None);

                if (searchInfo.BookType.HasValue)
                    userBookPredicate = userBookPredicate.And(ub => (ub.iBookType & (int)searchInfo.BookType) != (int)UserBookType.None);

                if (searchInfo.Status.HasValue)
                    userBookPredicate = userBookPredicate.And(ub => (ub.iStatus & (int)searchInfo.Status) != (int)UserBookStatusType.None);

                if (searchInfo.UserId.HasValue)
                    userBookPredicate = userBookPredicate.And(ub => ub.UserId == searchInfo.UserId);

                result = result.Join(
                  ContextManager.Current.UserBooks.AsExpandable().Where(userBookPredicate),
                  book => book.Id,
                  userBook => userBook.BookId,
                  (book, userBook) => book);
            }

            if (searchInfo.ExistentIds.Count == 0)
                totalCount = result.Count();
            else
                totalCount = -1;

            return result
              .OrderBy(book => book.Title)
              .Take(searchInfo.ReturnCount)
              .ToList<Book>();
        }

        #endregion Search Routine

        public void UpdateBooksLastModifiedDate()
        {
            foreach (Book book in ContextManager.Current.Books)
            {
                book.LastModifiedDate = DateTimeHelper.Now;
            }

            ContextManager.Current.SaveChanges();
        }

        #region IEntityController<Book> Members

        public IList<Book> Load()
        {
            var result = from Book book in ContextManager.Current.Books
                         orderby book.Title
                         select book;

            return result.ToList<Book>();
        }

        public Book Load(int id)
        {
            return ContextManager.Current.Books.SingleOrDefault<Book>(book => book.Id == id);
        }

        public void Update(Book entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entity.LastModifiedDate = DateTimeHelper.Now;

            if (entity.Id == 0)
            {
                entity.CreatedDate = DateTimeHelper.Now;
                ContextManager.Current.AddToBooks(entity);
            }

            UrlRewriteHelper.ApplyUrlRewrite(entity);

            ContextManager.Current.SaveChanges();
        }

        public void Delete(Book entity)
        {
            ContextManager.Current.DeleteObject(entity);
            ContextManager.Current.SaveChanges();
        }

        #endregion IEntityController<Book> Members

        #region IRewritableController<Book> Members

        public Book FindByUrlRewrite(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                return ContextManager.Current.Books.SingleOrDefault<Book>(book => book.UrlRewrite == alias);
            return null;
        }

        #endregion IRewritableController<Book> Members

        #region IAutoCompletableController<BookAuthor> Members

        public IList<BookAuthor> GetAutoCompleteList(string prefixText)
        {
            if (prefixText != null)
                prefixText = prefixText.ToLower();
            else
                prefixText = string.Empty;

            var result = from BookAuthor ba in ContextManager.Current.BookAuthors
                         where (ba.Book.Title.ToLower().Contains(prefixText) || ba.Author.FullName.ToLower().Contains(prefixText)) &&
                               ba.Book.Confirmed
                         orderby ba.Book.Title, ba.Author.FullName
                         select ba;

            return result.ToList<BookAuthor>();
        }

        #endregion IAutoCompletableController<BookAuthor> Members
    }
}