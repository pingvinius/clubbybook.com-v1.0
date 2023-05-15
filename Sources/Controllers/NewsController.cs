namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;

    public sealed class NewsController : IEntityController<News>, IRewritableController<News>
    {
        #region Search Routine

        public IList<News> Search(NewsSearchInfo searchInfo)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from News news in ContextManager.Current.News
                         where !searchInfo.ExistentIds.Contains(news.Id) && news.Message.ToLower().Contains(searchInfo.SearchText)
                         orderby news.CreatedDate descending
                         select news;

            return result
              .Take(searchInfo.ReturnCount)
              .ToList();
        }

        #endregion Search Routine

        #region IEntityController<News> Members

        public IList<News> Load()
        {
            return new List<News>(ContextManager.Current.News.AsQueryable<News>());
        }

        public News Load(int id)
        {
            return ContextManager.Current.News.SingleOrDefault<News>(news => news.Id == id);
        }

        public void Update(News entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entity.LastModifiedDate = DateTimeHelper.Now;

            if (entity.Id == 0)
            {
                entity.CreatedDate = DateTimeHelper.Now;
                ContextManager.Current.AddToNews(entity);
            }

            UrlRewriteHelper.ApplyUrlRewrite(entity);

            ContextManager.Current.SaveChanges();
        }

        public void Delete(News entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ContextManager.Current.DeleteObject(entity);
            ContextManager.Current.SaveChanges();
        }

        #endregion IEntityController<News> Members

        #region IRewritableController<News> Members

        public News FindByUrlRewrite(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                return ContextManager.Current.News.SingleOrDefault<News>(news => news.UrlRewrite == alias);
            return null;
        }

        #endregion IRewritableController<News> Members
    }
}