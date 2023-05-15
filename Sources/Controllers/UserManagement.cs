namespace ClubbyBook.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.Utilities;

    public static class UserManagement
    {
        #region Constants

        public const string AdminRoleName = "Admin";
        public const string EditorRoleName = "Editor";
        public const string AccountRoleName = "Account";

        #endregion Constants

        public static User CurrentUser
        {
            get
            {
                try
                {
                    if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        int id = 0;
                        if (int.TryParse(HttpContext.Current.User.Identity.Name, out id))
                            return ControllerFactory.UsersController.Load(id);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

                return null;
            }
        }

        public static Profile CurrentProfile
        {
            get
            {
                User currentUser = CurrentUser;
                if (currentUser != null)
                    return ControllerFactory.ProfilesController.GetProfile(currentUser);
                return null;
            }
        }

        public static Role CurrentRole
        {
            get
            {
                if (CurrentUser == null)
                    return null;
                return ControllerFactory.UsersController.GetUserRole(CurrentUser);
            }
        }

        public static bool IsAuthenticated
        {
            get
            {
                return IsAccountAuthenticated || IsEditorAuthenticated || IsAdminAuthenticated;
            }
        }

        public static bool IsAdminAuthenticated
        {
            get
            {
                return ControllerFactory.UsersController.IsUserInRole(UserManagement.CurrentUser, AdminRoleName);
            }
        }

        public static bool IsEditorAuthenticated
        {
            get
            {
                return ControllerFactory.UsersController.IsUserInRole(UserManagement.CurrentUser, EditorRoleName);
            }
        }

        public static bool IsAccountAuthenticated
        {
            get
            {
                return ControllerFactory.UsersController.IsUserInRole(UserManagement.CurrentUser, AccountRoleName);
            }
        }

        public static User RegisterUser(string email, string password)
        {
            User newUser = new User();
            newUser.EMail = email.ToLower();
            newUser.Password = MD5Helper.Calculate(password);
            newUser.CreatedDate = DateTimeHelper.Now;
            newUser.LastAccessDate = DateTimeHelper.Now;
            ContextManager.Current.AddToUsers(newUser);

            Profile newUserProfile = new Profile();
            newUserProfile.User = newUser;
            newUserProfile.Gender = GenderType.NotSpecified;
            newUserProfile.Name = string.Empty;
            newUserProfile.Nickname = string.Empty;
            newUserProfile.Surname = string.Empty;
            newUserProfile.Birthday = null;
            UrlRewriteHelper.ApplyUrlRewrite(newUserProfile);
            ContextManager.Current.AddToProfiles(newUserProfile);

            UserRole userRoleRelation = new UserRole();
            userRoleRelation.User = newUser;
            userRoleRelation.Role = ContextManager.Current.Roles.SingleOrDefault<Role>(role => role.Name == AccountRoleName);
            ContextManager.Current.AddToUserRoles(userRoleRelation);

            ContextManager.Current.SaveChanges();

            return newUser;
        }

        public static bool Login(User user, bool isPersistent)
        {
            FormsAuthentication.Initialize();

            if (user == null)
                throw new ArgumentNullException("user");

            Role role = ControllerFactory.UsersController.GetUserRole(user);
            if (role == null)
                throw new ArgumentException("The user should have a role.");

            DateTime dtNow = DateTimeHelper.Now;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                user.Id.ToString(),
                dtNow,
                isPersistent ? dtNow.AddMonths(2) : dtNow.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                isPersistent,
                role.Name,
                FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.HttpOnly = !FormsAuthentication.RequireSSL;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (isPersistent)
                cookie.Expires = ticket.Expiration;

            if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain))
                cookie.Domain = FormsAuthentication.CookieDomain;

            HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            HttpContext.Current.Response.Cookies.Add(cookie);

            // Update last access date for user
            user.LastAccessDate = DateTimeHelper.Now;
            ControllerFactory.UsersController.Update(user);

            return true;
        }

        public static bool Logout()
        {
            // Update last access date for user
            if (IsAuthenticated)
            {
                CurrentUser.LastAccessDate = DateTimeHelper.Now;
                ControllerFactory.UsersController.Update(CurrentUser);
            }

            FormsAuthentication.SignOut();

            return true;
        }
    }
}