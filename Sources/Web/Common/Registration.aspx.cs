namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.BackgroundActions;
    using ClubbyBook.BackgroundActions.Mailing;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Services;
    using ClubbyBook.Web.Utilities;

    public partial class Registration : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Зарегистрироваться в социальную сеть любителей книг", UIUtilities.SiteBrandName);
            }
        }

        #region Page Events

        protected void btnRegistration_Click(object sender, EventArgs e)
        {
            using (ValidateService validateService = new ValidateService())
            {
                if (validateService.CheckRegistration(tbEmail.Text, tbPassword.Text) == LoginResultType.OK)
                {
                    var registeredUser = UserManagement.RegisterUser(tbEmail.Text, tbPassword.Text);
                    if (registeredUser != null)
                    {
                        BackgroundActionManager.Instance.ExecuteAction(new SendRegistrationMailAction(registeredUser, tbPassword.Text));

                        UserManagement.Login(registeredUser, false);
                        GoToReturnUrl(RedirectDirection.ViewProfile, ControllerFactory.ProfilesController.GetProfile(registeredUser).UrlRewrite);
                    }
                }
            }
        }

        #endregion Page Events
    }
}