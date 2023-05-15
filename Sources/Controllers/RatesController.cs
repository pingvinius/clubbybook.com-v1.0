namespace ClubbyBook.Controllers
{
    using System;
    using System.Linq;
    using ClubbyBook.Business;

#warning REFFCTORING

    public sealed class RatesController
    {
        public static double GetBookRating(Book book)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (book.BookRates.Count == 0)
                return .0;

            return book.BookRates.Average(bookRate => (double)bookRate.Value);
        }

        public static int GetBookRating(Book book, User user)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (user == null)
                return 0;

            BookRate bookRate = ContextManager.Current.BookRates.SingleOrDefault<BookRate>(br =>
              br.BookId == book.Id && br.UserId == user.Id);
            return bookRate != null ? bookRate.Value : 0;
        }

        public static void SetBookRating(Book book, User user, int value)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            if (user == null)
                throw new ArgumentNullException("user");

            BookRate bookRate = ContextManager.Current.BookRates.SingleOrDefault<BookRate>(br => br.BookId == book.Id && br.UserId == user.Id);
            if (bookRate == null)
            {
                bookRate = new BookRate();
                bookRate.Book = book;
                bookRate.User = user;
                ContextManager.Current.AddToBookRates(bookRate);
            }

            bookRate.Value = value;

            ContextManager.Current.SaveChanges();
        }

        public static int GetBookVotesCount(Book book)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            return book.BookRates.Count;
        }

        public static double GetAuthorRating(Author author)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            if (author.AuthorRates.Count == 0)
                return .0;

            return author.AuthorRates.Average(authorRate => (double)authorRate.Value);
        }

        public static int GetAuthorRating(Author author, User user)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            if (user == null)
                return 0;

            AuthorRate authorRate = ContextManager.Current.AuthorRates.SingleOrDefault<AuthorRate>(ar =>
              ar.AuthorId == author.Id && ar.UserId == user.Id);
            return authorRate != null ? authorRate.Value : 0;
        }

        public static void SetAuthorRating(Author author, User user, int value)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            if (user == null)
                throw new ArgumentNullException("user");

            AuthorRate authorRate = ContextManager.Current.AuthorRates.SingleOrDefault<AuthorRate>(ar => ar.AuthorId == author.Id && ar.UserId == user.Id);
            if (authorRate == null)
            {
                authorRate = new AuthorRate();
                authorRate.Author = author;
                authorRate.User = user;
                ContextManager.Current.AddToAuthorRates(authorRate);
            }

            authorRate.Value = value;

            ContextManager.Current.SaveChanges();
        }

        public static int GetAuthorVotesCount(Author author)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            return author.AuthorRates.Count;
        }
    }
}