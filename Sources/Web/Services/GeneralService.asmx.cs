namespace ClubbyBook.Web.Services
{
    using System;
    using System.Text;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.BackgroundActions;
    using ClubbyBook.BackgroundActions.Mailing;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class GeneralService : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod]
        public ServiceResultType LeaveFeedback(int userId, string message)
        {
            // TODO: validate strings and trim them

            if (!string.IsNullOrEmpty(message))
            {
                User user = null;
                if (userId != -1)
                    user = ControllerFactory.UsersController.Load(userId);

                ControllerFactory.NotificationsController.AddNewFeedbackNotification(user, message);

                BackgroundActionManager.Instance.ExecuteAction(new SendFeedbackNotificationMailAction(user, message));
            }

            return ServiceResultType.OK;
        }

        [WebMethod]
        [ScriptMethod]
        public string ValidateTextBeforePost(string text)
        {
            if (!UserManagement.IsEditorAuthenticated && !UserManagement.IsAdminAuthenticated)
                throw new UnauthorizedAccessException();

            while (text.IndexOf("  ") != -1)
                text = text.Replace("  ", " ");

            while (text.IndexOf('…') != -1)
                text = text.Replace("…", "...");

            while (text.IndexOf('–') != -1)
                text = text.Replace("–", "-");

            while (text.IndexOf('—') != -1)
                text = text.Replace("—", "-");

            while (text.IndexOf('”') != -1)
                text = text.Replace("”", "\"");

            while (text.IndexOf('“') != -1)
                text = text.Replace("“", "\"");

            while (text.IndexOf('«') != -1)
                text = text.Replace("«", "\"");

            while (text.IndexOf('»') != -1)
                text = text.Replace("»", "\"");

            const int russianStart = 0x0400;
            const int russianEnd = 0x04FF;

            StringBuilder sb = new StringBuilder();
            foreach (char ch in text)
                if ((int)ch <= 255 || ((int)ch >= russianStart && (int)ch <= russianEnd))
                    sb.Append(ch);

            return sb.ToString();
        }
    }
}