namespace ClubbyBook.Web.Editor
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class EditNews : EntityEditPage<ClubbyBook.Business.News, NewsController>
    {
        public override string PageTitle
        {
            get
            {
                if (Entity == null)
                    return string.Format("{0} - Добавление новой новости", UIUtilities.SiteBrandName);
                return string.Format("{0} - Редактирование новости {1}", UIUtilities.SiteBrandName,
                  Entity.Title);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity != null)
                contentHeader.Title = Entity.Title;

            if (!IsPostBack && Entity != null)
            {
                tbTitle.Text = Entity.Title;
                tbMessage.Text = Entity.Message ?? string.Empty;
            }
        }

        #region Page Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                News newsEntity = Entity ?? new News();

                newsEntity.Title = tbTitle.Text.Trim();
                newsEntity.Message = tbMessage.Text.Trim();

                EntityController.Update(newsEntity);

                RedirectHelper.Redirect(RedirectDirection.ViewNews, newsEntity.UrlRewrite);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Entity != null)
                RedirectHelper.Redirect(RedirectDirection.ViewNews, Entity.UrlRewrite);
            else
                RedirectHelper.Redirect(RedirectDirection.NewsList);
        }

        #endregion Page Events
    }
}