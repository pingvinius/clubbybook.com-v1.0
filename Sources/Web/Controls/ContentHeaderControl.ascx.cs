namespace ClubbyBook.Web.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public partial class ContentHeaderControl : System.Web.UI.UserControl
    {
        private List<HeaderAction> actions = new List<HeaderAction>();

        #region Properties

        public string Title { get; set; }

        public string ExpandCollapseClientFunc { get; set; }

        public bool AllowExpandCollapse
        {
            get
            {
                return !string.IsNullOrEmpty(ExpandCollapseClientFunc);
            }
        }

        public bool SmallTitle { get; set; }

        public IList<HeaderAction> Actions
        {
            get
            {
                return actions;
            }
        }

        #endregion Properties

        protected void Page_Load(object sender, EventArgs e)
        {
            BindActions();
        }

        #region Control Events

        protected void rpActions_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            HeaderAction haAction = e.Item.DataItem as HeaderAction;
            if (haAction == null)
                return;

            HtmlAnchor lnkAction = e.Item.FindControl("lnkAction") as HtmlAnchor;
            if (lnkAction != null)
            {
                lnkAction.InnerText = haAction.Title;
                lnkAction.HRef = ResolveUrl(haAction.Link);
                if (!string.IsNullOrEmpty(haAction.OnClickCode))
                    lnkAction.Attributes["onclick"] = haAction.OnClickCode;
            }
        }

        #endregion Control Events

        #region Private Members

        private void BindActions()
        {
            rpActions.DataSource = Actions;
            rpActions.DataBind();
        }

        #endregion Private Members
    }

    public class HeaderAction
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public string OnClickCode { get; set; }

        public HeaderAction(string title, string link)
        {
            this.Title = title;
            this.Link = link;
            this.OnClickCode = string.Empty;
        }

        public HeaderAction(string title, ClientOnClickCode code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            this.Title = title;
            this.Link = code.HRefCode;
            this.OnClickCode = code.OnClickCode;
        }
    }

    public class ClientOnClickCode
    {
        public string OnClickCode
        {
            get
            {
                return string.Format("{0}{1} return false;", JavascriptCode, JavascriptCode.EndsWith(";") ? string.Empty : ";");
            }
        }

        public string HRefCode
        {
            get
            {
                return "javascript: void(0)";
            }
        }

        private string JavascriptCode { get; set; }

        public ClientOnClickCode(string javascriptCode)
        {
            this.JavascriptCode = javascriptCode.Trim();
        }
    }
}