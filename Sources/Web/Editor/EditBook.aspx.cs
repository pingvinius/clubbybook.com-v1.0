namespace ClubbyBook.Web.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.UI.WebControls;
    using ClubbyBook.Business;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class EditBook : EntityEditPage<Book, BooksController>
    {
        public override string PageTitle
        {
            get
            {
                if (Entity == null)
                    return string.Format("{0} - Добавление новой книги", UIUtilities.SiteBrandName);
                return string.Format("{0} - Редактирование книги {1}", UIUtilities.SiteBrandName,
                  Entity.Title);
            }
        }

        protected IList<Author> BookAuthorsList
        {
            get
            {
                if (Entity == null)
                    return new List<Author>();

                if (ViewState["ebBookAuthorsList"] == null)
                    ViewState["ebBookAuthorsList"] = EntityController.GetBookAuthors(Entity);

                return ViewState["ebBookAuthorsList"] as IList<Author>;
            }
        }

        protected string AuthorsJavascriptArray
        {
            get
            {
                List<string> jsArrayItem = new List<string>();
                foreach (Author author in BookAuthorsList)
                    jsArrayItem.Add(string.Format("{{ value: {0}, text: \"{1}\" }}", author.Id, author.FullName));

                return string.Format("[{0}]", string.Join(",", jsArrayItem.ToArray()));
            }
        }

        protected bool CanModifyConfirmedState
        {
            get
            {
                return Entity == null || (Entity != null && !Entity.Confirmed);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity != null)
            {
                string title = Entity.Title;
                if (!Entity.Confirmed)
                    title += " (На проверке...)";
                contentHeader.Title = title;
            }

            if (!IsPostBack)
            {
                ddlGenres.Items.Clear();
                ddlGenres.Items.Add(new ListItem(UIUtilities.SelectString, "-1"));
                foreach (var genreTreeItem in ControllerFactory.GenresController.GetGenresTreeViewList())
                    ddlGenres.Items.Add(new ListItem(genreTreeItem.Description, genreTreeItem.Value.ToString()));

                chkConfirmedBook.Checked = Entity == null ? true : Entity.Confirmed;

                if (Entity != null)
                {
                    tbTitle.Text = Entity.Title;
                    tbOriginalTitle.Text = Entity.OriginalTitle;

                    List<int> bookAuthorsIds = new List<int>();
                    foreach (var author in BookAuthorsList)
                        bookAuthorsIds.Add(author.Id);
                    hfAuthors.Value = string.Join(",", bookAuthorsIds);

                    string firstBookGenreId = "-1";
                    foreach (var genre in Entity.BookGenres)
                    {
                        firstBookGenreId = genre.Genre.Id.ToString();
                        break;
                    }

                    foreach (ListItem listItem in ddlGenres.Items)
                    {
                        if (listItem.Value == firstBookGenreId)
                        {
                            listItem.Selected = true;
                            break;
                        }
                    }

                    tbDescription.Text = Entity.Description ?? string.Empty;
                    chkCollection.Checked = Entity.Collection;
                }
            }
        }

        #region Page Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                Book bookEntity = Entity ?? new Book();
                bookEntity.Title = tbTitle.Text.Trim();
                bookEntity.OriginalTitle = tbOriginalTitle.Text.Trim();
                bookEntity.Description = tbDescription.Text.Trim();
                bookEntity.Collection = chkCollection.Checked;
                if (CanModifyConfirmedState)
                    bookEntity.Confirmed = chkConfirmedBook.Checked;

                List<Author> authorsList = new List<Author>();
                string[] ids = hfAuthors.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string idStr in ids)
                {
                    int tmpInt = -1;
                    if (int.TryParse(idStr, out tmpInt) && tmpInt >= 0)
                    {
                        Author selectedAuthor = ControllerFactory.AuthorsController.Load(tmpInt);
                        if (selectedAuthor != null)
                            authorsList.Add(selectedAuthor);
                    }
                }
                EntityController.SetBookAuthors(bookEntity, authorsList);

                List<Genre> genresList = new List<Genre>();
                int genreId = -1;
                if (int.TryParse(ddlGenres.SelectedValue, out genreId))
                {
                    Genre genre = ControllerFactory.GenresController.Load(genreId);
                    if (genre != null)
                        genresList.Add(genre);
                }
                EntityController.SetBookGenres(bookEntity, genresList);

                EntityController.Update(bookEntity);

                string fileTempPath = imageUploader.ImagePath;
                if (!string.IsNullOrEmpty(fileTempPath))
                {
                    try
                    {
                        string bookCoverFileName = string.Format(Settings.BookCoverFileName, bookEntity.Id);
                        string bookCoverPath = Path.Combine(Settings.ImagesBooksPath, bookCoverFileName);

                        bookEntity.CoverPath = bookCoverPath;

                        File.Copy(Server.MapPath(fileTempPath), Server.MapPath(bookCoverPath), true);
                        File.Delete(Server.MapPath(fileTempPath));
                    }
                    catch (Exception ex)
                    {
                        bookEntity.CoverPath = null;
                        Logger.Write(ex);
                    }

                    EntityController.Update(bookEntity);
                }

                RedirectHelper.Redirect(RedirectDirection.ViewBook, bookEntity.UrlRewrite);
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