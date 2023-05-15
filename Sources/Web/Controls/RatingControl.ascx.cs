namespace ClubbyBook.Web.Controls
{
    using System;
    using ClubbyBook.Controllers;

    public partial class RatingControl : System.Web.UI.UserControl
    {
        public string GetCommonRatingServiceMethod
        {
            get
            {
                string sValue = ViewState["rcGetCommonRatingServiceMethod"] as string;
                if (sValue == null)
                {
                    sValue = string.Empty;
                    ViewState["rcGetCommonRatingServiceMethod"] = sValue;
                }

                return sValue;
            }
            set
            {
                ViewState["rcGetCommonRatingServiceMethod"] = value;
            }
        }

        public string GetUserRatingServiceMethod
        {
            get
            {
                string sValue = ViewState["rcGetUserRatingServiceMethod"] as string;
                if (sValue == null)
                {
                    sValue = string.Empty;
                    ViewState["rcGetUserRatingServiceMethod"] = sValue;
                }

                return sValue;
            }
            set
            {
                ViewState["rcGetUserRatingServiceMethod"] = value;
            }
        }

        public string SetUserRatingServiceMethod
        {
            get
            {
                string sValue = ViewState["rcSetUserRatingServiceMethod"] as string;
                if (sValue == null)
                {
                    sValue = string.Empty;
                    ViewState["rcSetUserRatingServiceMethod"] = sValue;
                }

                return sValue;
            }
            set
            {
                ViewState["rcSetUserRatingServiceMethod"] = value;
            }
        }

        public string EntityId
        {
            get
            {
                string sValue = ViewState["rcEntityId"] as string;
                if (sValue == null)
                {
                    sValue = string.Empty;
                    ViewState["rcEntityId"] = sValue;
                }

                return sValue;
            }
            set
            {
                ViewState["rcEntityId"] = value;
            }
        }

        public string InfoMessageControlIdToRemove { get; set; }

        protected int EntityIdInt
        {
            get
            {
                int idInt = -1;
                if (int.TryParse(EntityId, out idInt) && idInt >= 0)
                    return idInt;
                return -1;
            }
        }

        protected bool IsReadOnly
        {
            get
            {
                return !UserManagement.IsAuthenticated || string.IsNullOrEmpty(SetUserRatingServiceMethod);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}