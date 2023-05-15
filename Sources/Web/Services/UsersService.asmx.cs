namespace ClubbyBook.Web.Services
{
    using System;
    using System.Collections.Generic;
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
    public class UsersService : WebService
    {
        [WebMethod]
        [ScriptMethod]
        public ListResponse<ProfileItem> GetUsers(IList<SearchItem> searchParameters)
        {
            UsersSearchInfo searchInfo;

            if (ParseSearchParameters(searchParameters, out searchInfo))
                return GetUsersInternal(searchInfo);

            return GetUsersInternal(new UsersSearchInfo());
        }

        [WebMethod]
        [ScriptMethod]
        public int RemoveUserAccount(int userId)
        {
            User userToRemove = ControllerFactory.UsersController.Load(userId);
            if (userToRemove == null)
                throw new ArgumentException("The userId variable should contain user id of existent user.");

            if (!AccessManagement.CanRemoveUser(userToRemove))
                throw new UnauthorizedAccessException();

            ControllerFactory.UsersController.Delete(userToRemove);
            return (int)ServiceResultType.OK;
        }

        private bool ParseSearchParameters(IList<SearchItem> searchParameters, out UsersSearchInfo info)
        {
            info = new UsersSearchInfo();

            try
            {
                foreach (var item in searchParameters)
                {
                    switch (item.paramName.ToLower())
                    {
                        case "searchtext":
                            info.SearchText = item.paramValue as string;
                            break;

                        case "existentid":
                            info.ExistentIds.Add((int)item.paramValue);
                            break;

                        case "bookid":
                            info.BookId = (int)item.paramValue;
                            break;

                        case "cityid":
                            info.CityId = (int)item.paramValue;
                            break;

                        case "offer":
                            info.Offer = (UserBookOfferType)((int)item.paramValue);
                            break;

                        case "booktype":
                            info.BookType = (UserBookType)((int)item.paramValue);
                            break;

                        case "role":
                            if (!string.IsNullOrEmpty(item.paramValue as string))
                            {
                                if (info.Roles == null)
                                    info.Roles = new List<string>();
                                info.Roles.Add(item.paramValue as string);
                            }
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

        private string PrepareTotalItemCountString(int totalCount)
        {
            if (totalCount <= 0)
                return string.Empty;

            var totalCountToCheck = totalCount % 100;

            if (totalCountToCheck >= 5 && totalCountToCheck <= 20)
                return string.Format("В результате поиска найдено {0} читателей.", totalCount);
            else
            {
                totalCountToCheck = totalCountToCheck % 10;

                if (totalCountToCheck == 1)
                    return string.Format("В результате поиска найден {0} читатель.", totalCount);
                else if (totalCountToCheck >= 2 && totalCountToCheck <= 4)
                    return string.Format("В результате поиска найдено {0} читателя.", totalCount);
                else
                    return string.Format("В результате поиска найдено {0} читателей.", totalCount);
            }
        }

        private string PrepareSearchParametersString(UsersSearchInfo searchInfo)
        {
            string csps = "Параметры поиска: ";

            List<string> parameters = new List<string>();

            if (searchInfo.BookId.HasValue)
            {
                Book book = ControllerFactory.BooksController.Load(searchInfo.BookId.Value);
                if (book != null)
                    parameters.Add(string.Format("книга - {0}", book.Title));
            }

            if (searchInfo.CityId.HasValue)
            {
                City city = ControllerFactory.CitiesController.Load(searchInfo.CityId.Value);
                if (city != null)
                    parameters.Add(string.Format("город - {0}", city.Name));
            }

            if (searchInfo.Offer.HasValue)
                parameters.Add(string.Format("предложение - {0}", AttributeHelper.GetEnumValueDescription(searchInfo.Offer)));

            if (searchInfo.BookType.HasValue)
                parameters.Add(string.Format("вид книги - {0}", AttributeHelper.GetEnumValueDescription(searchInfo.BookType)));

            if (searchInfo.Roles != null && searchInfo.Roles.Count > 0)
                parameters.Add(string.Format("роли - {0}", string.Join(", ", searchInfo.Roles)));

            csps += string.Join("; ", parameters.ToArray());
            csps += ".";

            return parameters.Count == 0 ? string.Empty : csps;
        }

        private ListResponse<ProfileItem> GetUsersInternal(UsersSearchInfo searchInfo)
        {
            ListResponse<ProfileItem> response = new ListResponse<ProfileItem>();
            response.searchParametersString = PrepareSearchParametersString(searchInfo);
            response.items = new List<ProfileItem>();
            response.ids = new List<int>();

            int totalCount = 0;
            foreach (User user in ControllerFactory.UsersController.Search(searchInfo, out totalCount))
            {
                response.items.Add(CreateUserItem(user));
                response.ids.Add(user.Id);
            }

            response.totalItemCountString = PrepareTotalItemCountString(totalCount);
            response.moreItems = response.items.Count == searchInfo.ReturnCount;

            return response;
        }

        private ProfileItem CreateUserItem(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            Profile profile = ControllerFactory.ProfilesController.GetProfile(user);
            if (profile == null)
                throw new InvalidOperationException("profile should exists.");

            ProfileItem pi = new ProfileItem();

            pi.userId = user.Id;
            pi.seoImageAlt = UIUtilities.GetProfileImageAltForSEO(UIUtilities.GetProfileFullName(profile));
            pi.fullName = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(profile));
            pi.nickname = UIUtilities.ValidateStringValue(profile.Nickname);
            pi.genderName = AttributeHelper.GetEnumValueDescription(profile.Gender);
            pi.photoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(profile.ImagePath, Settings.EmptyProfileAvatarPath));
            pi.viewProfileLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewProfile, profile.UrlRewrite);
            pi.cityName = profile.City != null ? profile.City.Name : UIUtilities.NotSpecifiedString;
            pi.userBookCount = ControllerFactory.BooksController.GetUserBookCount(user);

            return pi;
        }
    }
}