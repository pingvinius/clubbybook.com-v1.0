using System;
using System.Collections.Generic;
using System.Linq;
using ClubbyBook.Business;

namespace ClubbyBook.Controllers
{
    public sealed class ProfilesController : IEntityController<Profile>, IRewritableController<Profile>
    {
        public Profile GetProfile(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return ContextManager.Current.Profiles.SingleOrDefault<Profile>(profile => profile.UserId == user.Id);
        }

        #region IEntityController<Profile> Members

        public IList<Profile> Load()
        {
            return new List<Profile>(ContextManager.Current.Profiles.AsQueryable<Profile>());
        }

        public Profile Load(int id)
        {
            return ContextManager.Current.Profiles.SingleOrDefault<Profile>(profile => profile.Id == id);
        }

        public void Update(Profile entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            UrlRewriteHelper.ApplyUrlRewrite(entity);

            ContextManager.Current.SaveChanges();
        }

        public void Delete(Profile entity)
        {
            throw new NotImplementedException();
        }

        #endregion IEntityController<Profile> Members

        #region IRewritableController<Profile> Members

        public Profile FindByUrlRewrite(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                return ContextManager.Current.Profiles.SingleOrDefault<Profile>(profile => profile.UrlRewrite == alias);
            return null;
        }

        #endregion IRewritableController<Profile> Members
    }
}