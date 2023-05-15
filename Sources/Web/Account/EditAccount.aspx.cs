namespace ClubbyBook.Web.Account
{
    using System;
    using ClubbyBook.BackgroundActions;
    using ClubbyBook.BackgroundActions.Mailing;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class EditAccount : EntityEditPage<User, UsersController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Редактирование аккаунта {1}", UIUtilities.SiteBrandName, Entity.EMail);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            if (Entity.Id != UserManagement.CurrentUser.Id)
                RedirectHelper.Redirect(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.UnauthorizedAccess));

            contentHeader.Title = string.Format("Редактирование аккаунта: {0}", Entity.EMail);

            if (!IsPostBack)
                tbEmail.Text = Entity.EMail;
        }

        #region Page Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            if (!string.IsNullOrEmpty(tbNewPassword.Text) && tbNewPassword.Text != tbNewPassword2.Text)
                throw new InvalidOperationException("The new password should be equals to repeated new password.");

            bool passwordWasChanged = false;

            Entity.EMail = tbEmail.Text.Trim();
            if (!string.IsNullOrEmpty(tbNewPassword.Text))
            {
                Entity.Password = MD5Helper.Calculate(tbNewPassword.Text);
                passwordWasChanged = true;
            }

            EntityController.Update(Entity);

            var profile = ControllerFactory.ProfilesController.GetProfile(Entity);
            ControllerFactory.ProfilesController.Update(profile); // update url rewrite

            if (passwordWasChanged)
            {
                BackgroundActionManager.Instance.ExecuteAction(new SendPasswordWasChangedMailAction(Entity, tbNewPassword.Text));
            }

            RedirectHelper.Redirect(RedirectDirection.ViewProfile, profile.UrlRewrite);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            var profile = ControllerFactory.ProfilesController.GetProfile(Entity);
            RedirectHelper.Redirect(RedirectDirection.ViewProfile, profile.UrlRewrite);
        }

        #endregion Page Events
    }
}