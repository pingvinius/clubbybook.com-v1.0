namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;
    using LinqKit;

    public sealed class UsersController : IEntityController<User>
    {
        #region Users Routine

        public User GetUser(string email)
        {
            email = email.ToLower();
            return ContextManager.Current.Users.SingleOrDefault<User>(user => user.EMail.ToLower() == email);
        }

        public User GetUser(string email, string password)
        {
            email = email.ToLower();
            password = MD5Helper.Calculate(password);
            return ContextManager.Current.Users.SingleOrDefault<User>(user => user.EMail.ToLower() == email && user.Password.ToLower() == password && !user.IsDeleted);
        }

        public IList<User> GetUsersByRoles(params string[] rolesNames)
        {
            if (rolesNames.Length > 0)
            {
                var result = from userRole in ContextManager.Current.UserRoles
                             where rolesNames.Contains(userRole.Role.Name)
                             select userRole.User;
                return result.ToList<User>();
            }

            return new List<User>();
        }

        #endregion Users Routine

        #region Roles Routine

        public IList<Role> GetRoles()
        {
            return new List<Role>(ContextManager.Current.Roles.AsQueryable<Role>());
        }

        public Role GetUserRole(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return ContextManager.Current.UserRoles.SingleOrDefault<UserRole>(userRole => userRole.UserId == user.Id).Role;
        }

        public bool IsUserInRole(User user, string roleName)
        {
            if (user == null)
                return false;

            Role userRole = GetUserRole(user);
            if (userRole != null && userRole.Name == roleName)
                return true;

            return false;
        }

        #endregion Roles Routine

        #region Search Routine

        public IList<User> Search(UsersSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
                throw new ArgumentNullException("searchInfo");

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from User user in ContextManager.Current.Users
                         join profile in ContextManager.Current.Profiles on user.Id equals profile.UserId
                         where !searchInfo.ExistentIds.Contains(user.Id) && !user.IsDeleted &&
                               ((profile.Name ?? string.Empty).ToLower().Contains(searchInfo.SearchText) ||
                               (profile.Surname ?? string.Empty).ToLower().Contains(searchInfo.SearchText) ||
                               (profile.Nickname ?? string.Empty).ToLower().Contains(searchInfo.SearchText))
                         select user;

            if (UserManagement.IsAuthenticated)
            {
                result = from User user in result
                         where user.Id != UserManagement.CurrentUser.Id
                         select user;
            }

            if (searchInfo.BookId.HasValue || searchInfo.Offer.HasValue || searchInfo.BookType.HasValue)
            {
                var userBookPredicate = PredicateBuilder.True<UserBook>();

                if (searchInfo.BookId.HasValue)
                    userBookPredicate = userBookPredicate.And(ub => ub.BookId == searchInfo.BookId);

                if (searchInfo.Offer.HasValue && searchInfo.Offer.Value != UserBookOfferType.None)
                    userBookPredicate = userBookPredicate.And(ub => (ub.iOffer & (int)searchInfo.Offer) != (int)UserBookOfferType.None);

                if (searchInfo.BookType.HasValue)
                    userBookPredicate = userBookPredicate.And(ub => (ub.iBookType & (int)searchInfo.BookType) != (int)UserBookType.None);

                result = result.Join(
                  ContextManager.Current.UserBooks.AsExpandable().Where(userBookPredicate),
                  user => user.Id,
                  userBook => userBook.UserId,
                  (user, userBook) => user);
            }

            if (searchInfo.CityId.HasValue)
            {
                result = from User user in result
                         join profile in ContextManager.Current.Profiles on user.Id equals profile.UserId
                         where profile.CityId.HasValue && profile.CityId == searchInfo.CityId
                         select user;
            }

            if (searchInfo.Roles != null)
            {
                result = from User user in result
                         join ur in ContextManager.Current.UserRoles on user.Id equals ur.UserId
                         join r in ContextManager.Current.Roles on ur.RoleId equals r.Id
                         where searchInfo.Roles.Contains(r.Name)
                         select user;
            }

            if (searchInfo.ExistentIds.Count == 0)
                totalCount = result.Count();
            else
                totalCount = -1;

            return result
              .Take(searchInfo.ReturnCount)
              .ToList();
        }

        #endregion Search Routine

        #region IEntityController<User> Members

        public IList<User> Load()
        {
            return new List<User>(ContextManager.Current.Users.AsQueryable<User>());
        }

        public User Load(int id)
        {
            return ContextManager.Current.Users.SingleOrDefault<User>(user => user.Id == id);
        }

        public void Update(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ContextManager.Current.SaveChanges();
        }

        public void Delete(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (UserManagement.CurrentUser == entity)
                UserManagement.Logout();

            entity.IsDeleted = true;

            ContextManager.Current.SaveChanges();
        }

        #endregion IEntityController<User> Members
    }
}