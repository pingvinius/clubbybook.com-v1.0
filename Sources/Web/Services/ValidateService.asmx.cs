namespace ClubbyBook.Web.Services
{
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.Controllers;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class ValidateService : WebService
    {
        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public LoginResultType CheckLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return LoginResultType.Error;

            return ControllerFactory.UsersController.GetUser(email, password) != null ? LoginResultType.OK : LoginResultType.Error;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public LoginResultType CheckResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return LoginResultType.Error;

            return ControllerFactory.UsersController.GetUser(email) != null ? LoginResultType.OK : LoginResultType.Error;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public LoginResultType CheckRegistration(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return LoginResultType.Error;

            Business.User user = ControllerFactory.UsersController.GetUser(email);
            if (user != null)
            {
                if (user.IsDeleted)
                    return LoginResultType.WasDeleted;
                else
                    return LoginResultType.AlreadyExist;
            }

            return LoginResultType.OK;
        }
    }
}