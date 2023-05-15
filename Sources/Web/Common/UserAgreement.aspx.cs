namespace ClubbyBook.Web.Common
{
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;

    public partial class UserAgreement : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Пользовательское соглашение социальной сети любителей книг", UIUtilities.SiteBrandName);
            }
        }
    }
}