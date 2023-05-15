namespace ClubbyBook.Web.Common
{
    using System;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class ViewBook : EntityViewPage<Book, BooksController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - {1}", UIUtilities.SiteBrandName, UIUtilities.GetBookImageAltForSEO(Entity.Title));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            // Adjust header title
            string title = UIUtilities.ValidateStringValue(Entity.Title);
            if (!Entity.Confirmed)
                title += " (На проверке...)";
            contentHeader.Title = title;

            if (AccessManagement.CanResolveDuplicateBook)
                contentHeader.Actions.Add(new HeaderAction("Это дубликат", RedirectHelper.ResolveUrl(RedirectDirection.ResolveDuplicateBook, Entity.UrlRewrite)));

            if (AccessManagement.CanEditBook)
                contentHeader.Actions.Add(new HeaderAction("Редактировать", RedirectHelper.ResolveUrl(RedirectDirection.EditBook, Entity.UrlRewrite)));

            // Fill information fields
            rating.EntityId = Entity.Id.ToString();

            if (!UserManagement.IsAuthenticated)
            {
                infoMessage.Type = InfoMessageType.ViewBookRegistrationMessage;
                addingCommentInfoMessage.Type = InfoMessageType.AddCommentRequirements;
            }
            else if (UserManagement.IsAuthenticated && Entity.BookRates.FirstOrDefault(br => br.User == UserManagement.CurrentUser) == null &&
              ControllerFactory.BooksController.ContainsBookInUserLibrary(UserManagement.CurrentUser, Entity))
            {
                infoMessage.Type = InfoMessageType.ViewBookPleaseRateBook;
                rating.InfoMessageControlIdToRemove = infoMessage.ClientID;
            }
        }
    }
}