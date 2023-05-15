namespace ClubbyBook.Web.Editor
{
    using System;
    using System.IO;
    using ClubbyBook.Business;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class EditAuthor : EntityEditPage<Author, AuthorsController>
    {
        public override string PageTitle
        {
            get
            {
                if (Entity == null)
                    return string.Format("{0} - Добавление нового автора", UIUtilities.SiteBrandName);
                return string.Format("{0} - Редактирование автора {1}", UIUtilities.SiteBrandName, Entity.FullName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity != null)
                contentHeader.Title = Entity.FullName;

            if (!IsPostBack)
            {
                ddlType.EnumType = typeof(AuthorType);
                ddlGender.EnumType = typeof(GenderType);

                if (Entity != null)
                {
                    tbFullName.Text = Entity.FullName;
                    ddlType.SelectedEnumValue = Entity.Type;
                    ddlGender.SelectedEnumValue = Entity.Gender;
                    if (Entity.BirthdayYear.HasValue)
                        tbBirthdayYear.Text = Entity.BirthdayYear.Value.ToString();
                    if (Entity.DeathYear.HasValue)
                        tbDeathYear.Text = Entity.DeathYear.Value.ToString();
                    tbShortDescription.Text = Entity.ShortDescription ?? string.Empty;
                    tbBiography.Text = Entity.Biography ?? string.Empty;
                }
            }
        }

        #region Page Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                Author authorEntity = Entity ?? new Author();
                authorEntity.FullName = tbFullName.Text.Trim();
                authorEntity.ShortDescription = tbShortDescription.Text.Trim();
                authorEntity.Biography = tbBiography.Text.Trim();

                authorEntity.Type = (AuthorType)ddlType.SelectedEnumValue;

                // reset gender and life years
                if (authorEntity.Type == AuthorType.PublishingHouse)
                {
                    ddlGender.SelectedEnumValue = GenderType.NotSpecified;
                    tbBirthdayYear.Text = string.Empty;
                    tbDeathYear.Text = string.Empty;
                }

                authorEntity.Gender = (GenderType)ddlGender.SelectedEnumValue;

                int birthdayYear = -1;
                if (!string.IsNullOrEmpty(tbBirthdayYear.Text))
                    int.TryParse(tbBirthdayYear.Text, out birthdayYear);
                authorEntity.BirthdayYear = birthdayYear == -1 ? new int?() : birthdayYear;

                int deathYear = -1;
                if (!string.IsNullOrEmpty(tbDeathYear.Text))
                    int.TryParse(tbDeathYear.Text, out deathYear);
                authorEntity.DeathYear = deathYear == -1 ? new int?() : deathYear;

                EntityController.Update(authorEntity);

                string fileTempPath = imageUploader.ImagePath;
                if (!string.IsNullOrEmpty(fileTempPath))
                {
                    try
                    {
                        string authorPhotoFileName = string.Format(Settings.AuthorPhotoFileName, authorEntity.Id);
                        string authorPhotoPath = Path.Combine(Settings.ImagesAuthorsPath, authorPhotoFileName);

                        authorEntity.PhotoPath = authorPhotoPath;

                        File.Copy(Server.MapPath(fileTempPath), Server.MapPath(authorPhotoPath), true);
                        File.Delete(Server.MapPath(fileTempPath));
                    }
                    catch (Exception ex)
                    {
                        authorEntity.PhotoPath = null;
                        Logger.Write(ex);
                    }

                    EntityController.Update(authorEntity);
                }

                RedirectHelper.Redirect(RedirectDirection.ViewAuthor, authorEntity.UrlRewrite);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Entity != null)
                RedirectHelper.Redirect(RedirectDirection.ViewAuthor, Entity.UrlRewrite);
            else
                RedirectHelper.Redirect(RedirectDirection.AuthorList);
        }

        #endregion Page Events
    }
}