namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class ViewProfile : EntityViewPage<Profile, ProfilesController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - {1}", UIUtilities.SiteBrandName,
                  UIUtilities.GetProfileImageAltForSEO(UIUtilities.GetProfileFullName(Entity)));
            }
        }

        public bool IsThatYou
        {
            get
            {
                return UserManagement.IsAuthenticated && UserManagement.CurrentUser == Entity.User;
            }
        }

        public bool ShowFillMessageToUser
        {
            get
            {
                return IsThatYou && (Entity.City == null || (string.IsNullOrEmpty(Entity.Nickname) && string.IsNullOrEmpty(Entity.Name) && string.IsNullOrEmpty(Entity.Surname)));
            }
        }

        public int UserLibraryBookCount
        {
            get
            {
                try
                {
                    return ControllerFactory.BooksController.GetUserBookCount(Entity.User);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int UserReadBookCount
        {
            get
            {
                try
                {
                    return ControllerFactory.BooksController.GetUserBookCountByStatus(Entity.User, UserBookStatusType.AlreadyRead);
                }
                catch
                {
                    return 0;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            // Adjust profile header title
            contentHeader.Title = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(Entity));

            if (IsThatYou || AccessManagement.CanEditUser(Entity.User))
            {
                contentHeader.Actions.Add(new HeaderAction("Настройка аккаунта", RedirectHelper.ResolveUrl(RedirectDirection.EditAccount, Entity.UrlRewrite)));
                contentHeader.Actions.Add(new HeaderAction("Редактировать", RedirectHelper.ResolveUrl(RedirectDirection.EditProfile, Entity.UrlRewrite)));
            }

            // Adjust profile books header title
            if (IsThatYou)
                profileBooksContentHeader.Title = "Мои книги";
            else
                profileBooksContentHeader.Title = "Книги читателя";

            profileBooksContentHeader.Actions.Add(new HeaderAction("Посмотреть все списком", RedirectHelper.ResolveUrl(RedirectDirection.BookList, Entity.UrlRewrite)));

            // Fill information fields
            infoMessage.Type = InfoMessageType.ViewProfileFillFieldsMessage;
        }

        #region Private Methods

        protected string ValidateProfileCity(City city)
        {
            if (city != null && !string.IsNullOrEmpty(city.Name))
                return city.Name;

            return UIUtilities.NotSpecifiedString;
        }

        #endregion Private Methods
    }
}