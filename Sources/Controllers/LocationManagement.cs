namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;

    public static class LocationManagement
    {
        public static Country DefaultCountry
        {
            get
            {
                // By default we have Ukraine with id = 1
                return ContextManager.Current.Countries.SingleOrDefault<Country>(country => country.Id == 1);
            }
        }

        public static IList<Country> GetCountries()
        {
            return new List<Country>(ContextManager.Current.Countries.AsQueryable<Country>());
        }

        public static IList<District> GetCountryDistricts(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            return GetCountryDistricts(country.Id);
        }

        public static IList<District> GetCountryDistricts(int countryId)
        {
            var result = from District district in ContextManager.Current.Districts
                         where district.CountryId == countryId
                         orderby district.Name ascending
                         select district;

            return result.ToList<District>();
        }

        public static IList<City> GetCountryCities(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            return GetCountryCities(country.Id);
        }

        public static IList<City> GetCountryCities(int countryId)
        {
            var result = from City city in ContextManager.Current.Cities
                         where city.CountryId == countryId
                         orderby city.Default descending, city.Name ascending
                         select city;

            return result.ToList<City>();
        }

        public static IList<City> GetDistrictCities(District district)
        {
            if (district == null)
                throw new ArgumentNullException("district");

            return GetDistrictCities(district.Id);
        }

        public static IList<City> GetDistrictCities(int districtId)
        {
            var result = from City city in ContextManager.Current.Cities
                         where city.DistrictId == districtId
                         orderby city.Default descending, city.Name ascending
                         select city;

            return result.ToList<City>();
        }
    }
}