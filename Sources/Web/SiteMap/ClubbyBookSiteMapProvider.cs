namespace ClubbyBook.Web.SiteMap
{
    using System;
    using System.Web;
    using System.Web.Caching;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Controllers;
    using ClubbyBook.Web.Utilities;

    public class ClubbyBookSiteMapProvider : StaticSiteMapProvider
    {
        public void ResetSiteMap()
        {
            HttpContext.Current.Cache.Remove("SiteMap");
        }

        public override SiteMapNode BuildSiteMap()
        {
            lock (this)
            {
                SiteMapNode parentNode = HttpContext.Current.Cache["SiteMap"] as SiteMapNode;
                if (parentNode == null)
                {
                    Clear();

                    parentNode = new SiteMapNode(this, RedirectDirection.Home.ToString());
                    parentNode.Title = "Главная";
                    parentNode.Url = RedirectHelper.ResolveUrl(RedirectDirection.Home);
                    parentNode[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.Home);
                    parentNode[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Daily);
                    AddNode(parentNode);

                    try
                    {
                        FillBookNodes(parentNode);
                        FillAuthorNodes(parentNode);
                        FillNewsNodes(parentNode);

                        AddGeneralPage(parentNode, RedirectDirection.About, "О проекте", ChangeFrequently.Monthly);
                        AddGeneralPage(parentNode, RedirectDirection.Contacts, "Контакты", ChangeFrequently.Monthly);
                        AddGeneralPage(parentNode, RedirectDirection.SiteMap, "Карта сайта", ChangeFrequently.Weekly);
                        AddGeneralPage(parentNode, RedirectDirection.UserAgreement, "Пользовательское соглашение", ChangeFrequently.Monthly);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("The error has been occupied while creating SiteMap.", ex);
                    }

                    HttpContext.Current.Cache.Insert("SiteMap", parentNode, null, Cache.NoAbsoluteExpiration, new TimeSpan(24, 0, 0));
                }

                return parentNode;
            }
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return BuildSiteMap();
        }

        private void AddGeneralPage(SiteMapNode parentNode, RedirectDirection page, string title, ChangeFrequently changeFrequently)
        {
            if (parentNode == null)
                throw new ArgumentNullException("parentNode");

            SiteMapNode pageNode = new SiteMapNode(this, page.ToString());
            pageNode.Title = title;
            pageNode.Url = RedirectHelper.ResolveUrl(page);
            pageNode[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(page);
            pageNode[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(changeFrequently);
            AddNode(pageNode, parentNode);
        }

        private void FillBookNodes(SiteMapNode parentNode)
        {
            if (parentNode == null)
                throw new ArgumentNullException("parentNode");

            SiteMapNode bookNodes = new SiteMapNode(this, RedirectDirection.BookList.ToString());
            bookNodes.Title = "Книги";
            bookNodes.Url = RedirectHelper.ResolveUrl(RedirectDirection.BookList);
            bookNodes[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.BookList);
            bookNodes[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Weekly);
            AddNode(bookNodes, parentNode);

            foreach (Book book in ControllerFactory.BooksController.Load())
            {
                if (book.Confirmed)
                {
                    SiteMapNode node = new SiteMapNode(this, string.Format("{0}{1}", RedirectDirection.ViewBook, book.Id));
                    node.Title = book.Title;
                    node.Url = RedirectHelper.ResolveUrl(RedirectDirection.ViewBook, book.UrlRewrite);
                    node[SiteMapHelper.LastModifiedDateKey] = SiteMapHelper.GetLastModifiedDate(book.LastModifiedDate);
                    node[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.ViewBook);
                    node[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Weekly);
                    AddNode(node, bookNodes);
                }
            }
        }

        private void FillAuthorNodes(SiteMapNode parentNode)
        {
            if (parentNode == null)
                throw new ArgumentNullException("parentNode");

            SiteMapNode authorNodes = new SiteMapNode(this, RedirectDirection.AuthorList.ToString());
            authorNodes.Title = "Авторы";
            authorNodes.Url = RedirectHelper.ResolveUrl(RedirectDirection.AuthorList);
            authorNodes[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.AuthorList);
            authorNodes[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Weekly);
            AddNode(authorNodes, parentNode);

            foreach (Author author in ControllerFactory.AuthorsController.Load())
            {
                SiteMapNode node = new SiteMapNode(this, string.Format("{0}{1}", RedirectDirection.ViewAuthor, author.Id));
                node.Title = author.FullName;
                node.Url = RedirectHelper.ResolveUrl(RedirectDirection.ViewAuthor, author.UrlRewrite);
                node[SiteMapHelper.LastModifiedDateKey] = SiteMapHelper.GetLastModifiedDate(author.LastModifiedDate);
                node[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.ViewAuthor);
                node[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Weekly);
                AddNode(node, authorNodes);
            }
        }

        private void FillNewsNodes(SiteMapNode parentNode)
        {
            if (parentNode == null)
                throw new ArgumentNullException("parentNode");

            SiteMapNode newsNodes = new SiteMapNode(this, RedirectDirection.NewsList.ToString());
            newsNodes.Title = "Новости";
            newsNodes.Url = RedirectHelper.ResolveUrl(RedirectDirection.NewsList);
            newsNodes[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.NewsList);
            newsNodes[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Weekly);
            AddNode(newsNodes, parentNode);

            foreach (News news in ControllerFactory.NewsController.Load())
            {
                SiteMapNode node = new SiteMapNode(this, string.Format("{0}{1}", RedirectDirection.ViewNews, news.Id));
                node.Title = news.Title;
                node.Url = RedirectHelper.ResolveUrl(RedirectDirection.ViewNews, news.UrlRewrite);
                node[SiteMapHelper.LastModifiedDateKey] = SiteMapHelper.GetLastModifiedDate(news.LastModifiedDate);
                node[SiteMapHelper.PriorityKey] = SiteMapHelper.GetPriority(RedirectDirection.ViewNews);
                node[SiteMapHelper.ChangeFrequentlyKey] = SiteMapHelper.GetChangeFrequently(ChangeFrequently.Monthly);
                AddNode(node, newsNodes);
            }
        }
    }
}