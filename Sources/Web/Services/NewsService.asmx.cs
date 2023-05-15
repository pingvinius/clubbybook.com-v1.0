namespace ClubbyBook.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Utilities;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class NewsService : System.Web.Services.WebService
    {
        private const int topNewCount = 5;

        [WebMethod]
        [ScriptMethod]
        public ListResponse<NewsItem> GetNews(IList<SearchItem> searchParameters)
        {
            NewsSearchInfo searchInfo;

            if (ParseSearchParameters(searchParameters, out searchInfo))
                return GetNewsInternal(searchInfo, false);

            return GetNewsInternal(new NewsSearchInfo(), false);
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<NewsItem> GetTopNews(IList<SearchItem> searchParameters)
        {
            return GetNewsInternal(new NewsSearchInfo(), true);
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveNews(int newsId)
        {
            if (!AccessManagement.CanRemoveNews)
                throw new UnauthorizedAccessException();

            News news = ControllerFactory.NewsController.Load(newsId);
            if (news != null)
            {
                ControllerFactory.NewsController.Delete(news);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        #region Private Methods

        private bool ParseSearchParameters(IList<SearchItem> searchParameters, out NewsSearchInfo info)
        {
            info = new NewsSearchInfo();

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

        private ListResponse<NewsItem> GetNewsInternal(NewsSearchInfo searchInfo, bool isTopNews)
        {
            ListResponse<NewsItem> response = new ListResponse<NewsItem>();
            response.searchParametersString = string.Empty;
            response.totalItemCountString = string.Empty;
            response.items = new List<NewsItem>();
            response.ids = new List<int>();

            if (isTopNews)
                searchInfo.ReturnCount = topNewCount;

            foreach (News news in ControllerFactory.NewsController.Search(searchInfo))
            {
                response.items.Add(CreateNewsItem(news));
                response.ids.Add(news.Id);
            }

            response.moreItems = isTopNews ? false : response.items.Count == searchInfo.ReturnCount;

            return response;
        }

        private NewsItem CreateNewsItem(News news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            NewsItem ni = new NewsItem();

            ni.id = news.Id;
            ni.title = news.Title;
            ni.restrictedMessage = UIUtilities.GetRestrictedNewsMessage(news);
            ni.createdDateFull = UIUtilities.GetFullDateString(news.CreatedDate);
            ni.createdDateShort = UIUtilities.GetShortDateString(news.CreatedDate);
            ni.viewNewsLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewNews, news.UrlRewrite);
            ni.editNewsLink = RedirectHelper.ResolveUrl(RedirectDirection.EditNews, news.UrlRewrite);

            return ni;
        }

        #endregion Private Methods
    }
}