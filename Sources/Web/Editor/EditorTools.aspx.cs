namespace ClubbyBook.Web.Editor
{
    using System;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;

    public partial class EditorTools : SimplePage
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Инструменты редактора", UIUtilities.SiteBrandName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}