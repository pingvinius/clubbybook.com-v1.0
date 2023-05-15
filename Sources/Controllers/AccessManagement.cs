namespace ClubbyBook.Controllers
{
    using System.Collections.Generic;
    using ClubbyBook.Business;

    public static class AccessManagement
    {
        private static Dictionary<string, string[]> urlToAccessRoles = new Dictionary<string, string[]>()
        {
          { "notifications", new string[] { UserManagement.AccountRoleName, UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "editprofile", new string[] { UserManagement.AccountRoleName, UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "editaccount", new string[] { UserManagement.AccountRoleName, UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "editbook", new string[] { UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "editauthor", new string[] { UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "editnews", new string[] { UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "resolveduplicatebook", new string[] { UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "editortools", new string[] { UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
          { "prepostvalidation", new string[] { UserManagement.EditorRoleName, UserManagement.AdminRoleName } },
        };

        public static bool CanSeeEmails
        {
            get
            {
                return UserManagement.IsAdminAuthenticated;
            }
        }

        public static bool CanExploitFeedbackNotifications
        {
            get
            {
                return UserManagement.IsAdminAuthenticated;
            }
        }

        public static bool CanExploitSystemNotifications
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanExploitConversationNotifications
        {
            get
            {
                return UserManagement.IsAuthenticated;
            }
        }

        public static bool CanAddAuthor
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanEditAuthor
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanRemoveAuthor
        {
            get
            {
                return UserManagement.IsAdminAuthenticated;
            }
        }

        public static bool CanAddBook
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanEditBook
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanResolveDuplicateBook
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanRemoveBook
        {
            get
            {
                return UserManagement.IsAdminAuthenticated;
            }
        }

        public static bool CanAddBookComment
        {
            get
            {
                return UserManagement.IsAuthenticated;
            }
        }

        public static bool CanRemoveBookComment(Comment comment)
        {
            return UserManagement.IsAdminAuthenticated || (UserManagement.IsAuthenticated && comment.User.Id == UserManagement.CurrentUser.Id);
        }

        public static bool CanAddAuthorComment
        {
            get
            {
                return UserManagement.IsAuthenticated;
            }
        }

        public static bool CanRemoveAuthorComment(Comment comment)
        {
            return UserManagement.IsAdminAuthenticated || (UserManagement.IsAuthenticated && comment.User.Id == UserManagement.CurrentUser.Id);
        }

        public static bool CanAddNews
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanEditNews
        {
            get
            {
                return UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated;
            }
        }

        public static bool CanRemoveNews
        {
            get
            {
                return UserManagement.IsAdminAuthenticated;
            }
        }

        public static bool CanAdvancedUsersSearch
        {
            get
            {
                return UserManagement.IsAdminAuthenticated;
            }
        }

        public static bool CanEditUser(User user)
        {
            return UserManagement.IsAuthenticated && user == UserManagement.CurrentUser && user != null;
        }

        public static bool CanRemoveUser(User user)
        {
            return UserManagement.IsAdminAuthenticated || (UserManagement.IsAuthenticated && user == UserManagement.CurrentUser && user != null);
        }

        public static bool ValidateAccessToUrl(string url)
        {
            url = url.ToLower();

            foreach (var pair in urlToAccessRoles)
            {
                if (url.Contains(pair.Key))
                {
                    bool allowAccess = false;
                    foreach (var roleName in pair.Value)
                    {
                        allowAccess |= ControllerFactory.UsersController.IsUserInRole(UserManagement.CurrentUser, roleName);
                    }

                    return allowAccess;
                }
            }

            return true;
        }
    }
}