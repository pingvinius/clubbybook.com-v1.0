namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;

    public sealed class CitiesController : IEntityController<City>, IRewritableController<City>, IAutoCompletableController<City>
    {
        #region IEntityController<City> Members

        public IList<City> Load()
        {
            var result = from City city in ContextManager.Current.Cities
                         orderby city.Name
                         select city;

            return result.ToList<City>();
        }

        public City Load(int id)
        {
            return ContextManager.Current.Cities.SingleOrDefault<City>(city => city.Id == id);
        }

        public void Update(City entity)
        {
            throw new InvalidOperationException();
        }

        public void Delete(City entity)
        {
            throw new InvalidOperationException();
        }

        #endregion IEntityController<City> Members

        #region IRewritableController<City> Members

        public City FindByUrlRewrite(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                return ContextManager.Current.Cities.SingleOrDefault<City>(city => city.UrlRewrite == alias);
            return null;
        }

        #endregion IRewritableController<City> Members

        #region IAutoCompletableController<City> Members

        public IList<City> GetAutoCompleteList(string prefixText)
        {
            if (prefixText != null)
                prefixText = prefixText.ToLower();
            else
                prefixText = string.Empty;

            var result = from City city in ContextManager.Current.Cities
                         where city.Name.ToLower().Contains(prefixText)
                         orderby city.Name, city.District.Name
                         select city;

            return result.ToList<City>();
        }

        #endregion IAutoCompletableController<City> Members
    }
}