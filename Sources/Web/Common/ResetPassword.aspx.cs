namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.BackgroundActions;
    using ClubbyBook.BackgroundActions.Mailing;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Services;

    public partial class ResetPassword : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Сброс пароля", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            using (ValidateService validateService = new ValidateService())
            {
                if (validateService.CheckResetPassword(tbEmail.Text) == LoginResultType.OK)
                {
                    User user = ControllerFactory.UsersController.GetUser(tbEmail.Text);
                    if (user != null)
                    {
                        var rnd = new Random((int)DateTime.Now.Ticks);
                        var minVal = 100000;
                        string newPassword = (rnd.Next(int.MaxValue - minVal) + minVal).ToString("x");

                        user.Password = MD5Helper.Calculate(newPassword);

                        ControllerFactory.UsersController.Update(user);

                        BackgroundActionManager.Instance.ExecuteAction(new SendResetPasswordMailAction(user, newPassword));
                    }
                }
            }
        }
    }
}