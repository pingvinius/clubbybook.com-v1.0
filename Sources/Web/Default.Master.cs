namespace ClubbyBook.Web
{
    using System;

    public partial class DefaultMaster : System.Web.UI.MasterPage
    {
        public bool IncludeGoogleAnalitics
        {
            get
            {
#if DEBUG
                return false;
#else
                return true;
#endif
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: paste verifying code here
        }
    }
}