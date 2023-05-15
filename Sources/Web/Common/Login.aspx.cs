namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Services;
    using ClubbyBook.Web.Utilities;

    public partial class Login : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Войти в социальную сеть любителей книг", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserManagement.IsAuthenticated)
                LetsWork();
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            using (ValidateService validateService = new ValidateService())
            {
                if (validateService.CheckLogin(tbEmail.Text, tbPassword.Text) == LoginResultType.OK)
                {
                    User user = ControllerFactory.UsersController.GetUser(tbEmail.Text, tbPassword.Text);
                    if (user != null)
                    {
                        UserManagement.Login(user, chkRememberMe.Checked);
                        RedirectHelper.RedirectDirectly(Request.RawUrl);
                    }
                }
            }
        }

        private void LetsWork()
        {
            // check access to return url
            if (!AccessManagement.ValidateAccessToUrl(ReturnUrl))
            {
                RedirectHelper.Redirect(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.UnauthorizedAccess));
                return;
            }

            GoToReturnUrl(RedirectDirection.Home);
        }
    }
}