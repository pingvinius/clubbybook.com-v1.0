namespace ClubbyBook.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Services;
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
    public class AuthorsService : System.Web.Services.WebService
    {
        #region Autocomplete

        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetAutoCompleteAuthors(string prefixText)
        {
            List<SimpleListItem> authorFullNames = new List<SimpleListItem>();
            foreach (Author author in ControllerFactory.AuthorsController.GetAutoCompleteList(prefixText))
                authorFullNames.Add(new SimpleListItem(author.FullName, author.Id));

            return authorFullNames;
        }

        #endregion Autocomplete

        [WebMethod]
        [ScriptMethod]
        public ListResponse<AuthorItem> GetAuthors(IList<SearchItem> searchParameters)
        {
            AuthorsSearchInfo searchInfo;

            if (ParseSearchParameters(searchParameters, out searchInfo))
                return GetAuthorsInternal(searchInfo);

            return GetAuthorsInternal(new AuthorsSearchInfo());
        }

        [WebMethod]
        [ScriptMethod]
        public ListResponse<CommentItem> GetAuthorComments(IList<SearchItem> searchParameters)
        {
            CommentSearchInfo searchInfo;

            if (ParseSearchParameters(searchParameters, out searchInfo))
                return GetAuthorCommentsInternal(searchInfo);

            return new ListResponse<CommentItem>();
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveAuthor(int authorId)
        {
            if (!AccessManagement.CanRemoveAuthor)
                throw new UnauthorizedAccessException();

            Author author = ControllerFactory.AuthorsController.Load(authorId);
            if (author != null)
            {
                ControllerFactory.AuthorsController.Delete(author);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public CommentItem AddAuthorComment(int authorId, string comment)
        {
            // TODO: validate strings and trim it

            if (!AccessManagement.CanAddAuthorComment)
                throw new UnauthorizedAccessException();

            Author author = ControllerFactory.AuthorsController.Load(authorId);
            if (author != null && !string.IsNullOrEmpty(comment))
                return CreateCommentItem(author, ControllerFactory.AuthorsController.AddAuthorComment(UserManagement.CurrentUser, author, comment));

            return null;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveAuthorComment(int authorId, int commentId)
        {
            Author author = ControllerFactory.AuthorsController.Load(authorId);
            if (author != null)
            {
                Comment comment = author.Comments.Where(c => c.Id == commentId).FirstOrDefault();
                if (comment != null)
                {
                    if (!AccessManagement.CanRemoveAuthorComment(comment))
                        throw new UnauthorizedAccessException();

                    ControllerFactory.AuthorsController.RemoveAuthorComment(author, comment);
                    return ServiceResultType.OK;
                }
            }

            return ServiceResultType.Fail;
        }

        #region Private Methods

        private bool ParseSearchParameters(IList<SearchItem> searchParameters, out AuthorsSearchInfo info)
        {
            info = new AuthorsSearchInfo();

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

                        case "authorid":
                            info.AuthorId = (int)item.paramValue;
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

        private ListResponse<AuthorItem> GetAuthorsInternal(AuthorsSearchInfo searchInfo)
        {
            ListResponse<AuthorItem> response = new ListResponse<AuthorItem>();
            response.searchParametersString = PrepareSearchParametersString(searchInfo);
            response.items = new List<AuthorItem>();
            response.ids = new List<int>();

            int totalCount = 0;
            foreach (Author author in ControllerFactory.AuthorsController.Search(searchInfo, out totalCount))
            {
                response.items.Add(CreateAuthorItem(author));
                response.ids.Add(author.Id);
            }

            response.totalItemCountString = PrepareTotalItemCountString(totalCount);
            response.moreItems = response.items.Count == searchInfo.ReturnCount;

            return response;
        }

        private ListResponse<CommentItem> GetAuthorCommentsInternal(CommentSearchInfo searchInfo)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            if (!searchInfo.AuthorId.HasValue)
                throw new ArgumentNullException("searchInfo.AuthorId");

            ListResponse<CommentItem> response = new ListResponse<CommentItem>();
            response.items = new List<CommentItem>();
            response.ids = new List<int>();

            Author author = ControllerFactory.AuthorsController.Load(searchInfo.AuthorId.Value);
            if (author == null)
                throw new ArgumentException(string.Format("The author entity {0} doesn't exist!", searchInfo.AuthorId.Value));

            int totalCount = 0;
            foreach (Comment comment in ControllerFactory.AuthorsController.GetAuthorComments(searchInfo, out totalCount))
            {
                response.items.Add(CreateCommentItem(author, comment));
                response.ids.Add(comment.Id);
            }

            response.totalItemCountString = string.Empty; // TODO: if it is necessary
            response.moreItems = searchInfo.ReturnCount != int.MaxValue ? response.items.Count == searchInfo.ReturnCount : false;

            return response;
        }

        private string PrepareTotalItemCountString(int totalCount)
        {
            if (totalCount <= 0)
                return string.Empty;

            var totalCountToCheck = totalCount % 100;

            if (totalCountToCheck >= 5 && totalCountToCheck <= 20)
                return string.Format("В результате поиска найдено {0} авторов.", totalCount);
            else
            {
                totalCountToCheck = totalCountToCheck % 10;

                if (totalCountToCheck == 1)
                    return string.Format("В результате поиска найден {0} автор.", totalCount);
                else if (totalCountToCheck >= 2 && totalCountToCheck <= 4)
                    return string.Format("В результате поиска найдено {0} автора.", totalCount);
                else
                    return string.Format("В результате поиска найдено {0} авторов.", totalCount);
            }
        }

        private string PrepareSearchParametersString(AuthorsSearchInfo searchInfo)
        {
            // TODO:
            return string.Empty;
        }

        private AuthorItem CreateAuthorItem(Author author)
        {
            if (author == null)
                throw new ArgumentNullException("author");

            AuthorItem ai = new AuthorItem();

            ai.id = author.Id;
            ai.fullName = author.FullName;
            ai.seoImageAlt = UIUtilities.GetAuthorImageAltForSEO(author.FullName);
            ai.yearsString = UIUtilities.GetAuthorLifeYearsString(author.Gender, author.BirthdayYear, author.DeathYear);
            ai.isPublishingHouse = author.Type == AuthorType.PublishingHouse;
            ai.typeString = AttributeHelper.GetEnumValueDescription(author.Type);
            ai.photoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(author.PhotoPath, Settings.EmptyAuthorPhotoPath));
            ai.booksCount = author.BookAuthors.Count;
            ai.viewAuthorLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewAuthor, author.UrlRewrite);
            ai.editAuthorLink = RedirectHelper.ResolveUrl(RedirectDirection.EditAuthor, author.UrlRewrite);
            ai.viewAuthorBooksLink = RedirectHelper.ResolveUrl(RedirectDirection.BookList, author.UrlRewrite);
            ai.restrictedDescription = UIUtilities.GetRestrictedAuthorDescription(author);

            return ai;
        }

        private CommentItem CreateCommentItem(Author author, Comment comment)
        {
            if (author == null)
                throw new ArgumentNullException("author");

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
            ci.seoImageAlt = string.Format("Комментарии к автору {0} | Отзывы о авторе {0}", author.FullName);
            ci.isUserComment = UserManagement.IsAdminAuthenticated || userProfile.User == UserManagement.CurrentUser;
            ci.createdDate = UIUtilities.GetFullDateString(comment.CreatedDate);
            return ci;
        }

        #endregion Private Methods
    }
}