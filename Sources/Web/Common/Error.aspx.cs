namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.UI;

    public partial class Error : System.Web.UI.Page
    {
        public string ErrorMessage
        {
            get
            {
                return AttributeHelper.GetEnumValueDescription(ErrorCode);
            }
        }

        private ErrorCodeType ErrorCode
        {
            get
            {
                string errorCodeStr = Request.QueryString["code"];
                if (!string.IsNullOrEmpty(errorCodeStr))
                {
                    int errorCodeInt = -1;
                    if (int.TryParse(errorCodeStr, out errorCodeInt))
                        return (ErrorCodeType)errorCodeInt;
                }

                return ErrorCodeType.None;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = string.Format("{0} - Ошибка", UIUtilities.SiteBrandName);
        }
    }
}