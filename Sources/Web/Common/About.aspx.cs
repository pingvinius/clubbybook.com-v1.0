namespace ClubbyBook.Web.Common
{
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;

    public partial class About : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Первая в Украине социальная сеть любителей книг", UIUtilities.SiteBrandName);
            }
        }
    }
}