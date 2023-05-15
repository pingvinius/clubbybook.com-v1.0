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

    public partial class ViewAuthor : EntityViewPage<Author, AuthorsController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - {1}", UIUtilities.SiteBrandName, UIUtilities.GetAuthorImageAltForSEO(Entity.FullName));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            // Adjust author header title
            contentHeader.Title = UIUtilities.ValidateStringValue(Entity.FullName);

            contentHeader.Actions.Add(new HeaderAction("Книги автора", RedirectHelper.ResolveUrl(RedirectDirection.BookList, Entity.UrlRewrite)));
            if (AccessManagement.CanEditAuthor)
                contentHeader.Actions.Add(new HeaderAction("Редактировать", RedirectHelper.ResolveUrl(RedirectDirection.EditAuthor, Entity.UrlRewrite)));

            // Fill information fields
            rating.EntityId = Entity.Id.ToString();

            if (!UserManagement.IsAuthenticated)
            {
                infoMessage.Type = InfoMessageType.ViewAuthorRegistrationMessage;
                addingCommentInfoMessage.Type = InfoMessageType.AddCommentRequirements;
            }
            else if (UserManagement.IsAuthenticated && Entity.AuthorRates.FirstOrDefault(ar => ar.User == UserManagement.CurrentUser) == null)
            {
                infoMessage.Type = InfoMessageType.ViewAuthorPleaseRateAuthor;
                rating.InfoMessageControlIdToRemove = infoMessage.ClientID;
            }
        }
    }
}