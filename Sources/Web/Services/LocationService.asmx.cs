namespace ClubbyBook.Web.Services
{
    using System.Collections.Generic;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class LocationService : WebService
    {
        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetCountries()
        {
            List<SimpleListItem> values = new List<SimpleListItem>();

            foreach (Country entity in LocationManagement.GetCountries())
                values.Add(new SimpleListItem(entity.Name, entity.Id));

            return values;
        }

        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetCountryDistricts(int countryId)
        {
            List<SimpleListItem> values = new List<SimpleListItem>();

            if (countryId < 0)
                countryId = LocationManagement.DefaultCountry.Id;

            foreach (District entity in LocationManagement.GetCountryDistricts(countryId))
                values.Add(new SimpleListItem(entity.Name, entity.Id));

            return values;
        }

        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetDistrictCities(int districtId)
        {
            List<SimpleListItem> values = new List<SimpleListItem>();

            if (districtId != -1)
                foreach (City entity in LocationManagement.GetDistrictCities(districtId))
                    values.Add(new SimpleListItem(entity.Name, entity.Id));

            return values;
        }

        [WebMethod]
        [ScriptMethod]
        public List<SimpleListItem> GetAutoCompleteCities(string prefixText)
        {
            List<SimpleListItem> resultList = new List<SimpleListItem>();

            foreach (City city in ControllerFactory.CitiesController.GetAutoCompleteList(prefixText))
                resultList.Add(new SimpleListItem(string.Format("{0} ({1})", city.Name, city.District.Name), city.Id));

            return resultList;
        }
    }
}