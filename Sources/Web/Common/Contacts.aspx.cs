namespace ClubbyBook.Web.Common
{
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;

    public partial class Contacts : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Контакты социальной сети любителей книг", UIUtilities.SiteBrandName);
            }
        }
    }
}