namespace ClubbyBook.Web.Editor
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class ResolveDuplicateBook : EntityEditPage<Book, BooksController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Исправить дубликат книги {1}", UIUtilities.SiteBrandName, Entity.Title);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            contentHeader.Title = string.Format("Исправить дубликат книги \"{0}\"", Entity.Title);
        }

        #region Page Events

        protected void btnResolve_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                Book originalBook = null;
                int originalBookId = -1;
                if (int.TryParse(hfDuplicateBook.Value, out originalBookId))
                    originalBook = ControllerFactory.BooksController.Load(originalBookId);

                if (originalBook == null)
                    throw new InvalidOperationException("The original book didn't found.");

                ControllerFactory.BooksController.ResolveDuplicateBook(Entity, originalBook);

                RedirectHelper.Redirect(RedirectDirection.ViewBook, originalBook.UrlRewrite);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Entity != null)
                RedirectHelper.Redirect(RedirectDirection.ViewBook, Entity.UrlRewrite);
            else
                RedirectHelper.Redirect(RedirectDirection.BookList);
        }

        #endregion Page Events
    }
}