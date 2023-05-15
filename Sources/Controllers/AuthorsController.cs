namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;

    public sealed class AuthorsController : IEntityController<Author>, IRewritableController<Author>, IAutoCompletableController<Author>
    {
        public IList<Book> GetAuthorBooks(Author author)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            var result = from BookAuthor bookAuthor in ContextManager.Current.BookAuthors
                         where bookAuthor.AuthorId == author.Id
                         select bookAuthor.Book;

            return result.ToList<Book>();
        }

        public void UpdateAuthorsLastModifiedDate()
        {
            foreach (Author author in ContextManager.Current.Authors)
            {
                author.LastModifiedDate = DateTimeHelper.Now;
            }

            ContextManager.Current.SaveChanges();
        }

        #region Author Comments Routine

        public IList<Comment> GetAuthorComments(CommentSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            if (!searchInfo.AuthorId.HasValue)
                throw new ArgumentNullException("searchInfo.AuthorId");

            Author author = Load(searchInfo.AuthorId.Value);
            if (author == null)
                throw new ArgumentException(string.Format("The author entity {0} should exist!", searchInfo.AuthorId.Value));
            ;
            var result = author.Comments.Where(c => !searchInfo.ExistentIds.Contains(c.Id));

            if (searchInfo.ExistentIds.Count == 0)
                totalCount = result.Count();
            else
                totalCount = -1;

            return result
              .OrderByDescending(comment => comment.CreatedDate)
              .Take(searchInfo.ReturnCount)
              .ToList<Comment>();
        }

        public Comment AddAuthorComment(User user, Author author, string comment)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (author == null)
                throw new ArgumentNullException("author");

            if (string.IsNullOrEmpty(comment))
                throw new ArgumentNullException("comment");

            var newComment = new Comment()
            {
                Message = comment,
                CreatedDate = DateTimeHelper.Now,
                User = user
            };
            author.Comments.Add(newComment);

            ContextManager.Current.SaveChanges();

            return newComment;
        }

        public void RemoveAuthorComment(Author author, Comment comment)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            if (comment == null)
                throw new ArgumentNullException("comment");

            if (author.Comments.Contains(comment))
            {
                ContextManager.Current.DeleteObject(comment);
                ContextManager.Current.SaveChanges();
            }
        }

        #endregion Author Comments Routine

        #region Search Routine

        public IList<Author> Search(AuthorsSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from Author author in ContextManager.Current.Authors
                         where !searchInfo.ExistentIds.Contains(author.Id) &&
                               author.FullName.ToLower().Contains(searchInfo.SearchText)
                         select author;

            if (searchInfo.ExistentIds.Count == 0)
                totalCount = result.Count();
            else
                totalCount = -1;

            return result
              .OrderBy(author => author.FullName)
              .Take(searchInfo.ReturnCount)
              .ToList<Author>();
        }

        #endregion Search Routine

        #region IEntityController<Author> Members

        public IList<Author> Load()
        {
            return new List<Author>(ContextManager.Current.Authors.AsQueryable<Author>());
        }

        public Author Load(int id)
        {
            return ContextManager.Current.Authors.SingleOrDefault<Author>(author => author.Id == id);
        }

        public void Update(Author entity)
        {
            if (entity == null)
                throw new ArgumentNullException("author");

            entity.LastModifiedDate = DateTimeHelper.Now;

            if (entity.Id == 0)
            {
                entity.CreatedDate = DateTimeHelper.Now;
                ContextManager.Current.AddToAuthors(entity);
            }

            UrlRewriteHelper.ApplyUrlRewrite(entity);

            ContextManager.Current.SaveChanges();
        }

        public void Delete(Author entity)
        {
            ContextManager.Current.DeleteObject(entity);
            ContextManager.Current.SaveChanges();
        }

        #endregion IEntityController<Author> Members

        #region IRewritableController<Author> Members

        public Author FindByUrlRewrite(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                return ContextManager.Current.Authors.SingleOrDefault<Author>(author => author.UrlRewrite == alias);
            return null;
        }

        #endregion IRewritableController<Author> Members

        #region IAutoCompletableController<Author> Members

        public IList<Author> GetAutoCompleteList(string prefixText)
        {
            if (string.IsNullOrEmpty(prefixText))
                return Load();

            prefixText = prefixText.ToLower();

            var result = from Author author in ContextManager.Current.Authors
                         where author.FullName.ToLower().Contains(prefixText)
                         select author;

            return result.ToList<Author>();
        }

        #endregion IAutoCompletableController<Author> Members
    }
}