namespace ClubbyBook.Web.Controls
{
    using System;
    using System.Web.UI.WebControls;
    using ClubbyBook.Common.Utilities;

    public partial class EnumDropDownList : System.Web.UI.UserControl
    {
        public event EventHandler SelectedIndexChanged;

        public Type EnumType
        {
            get
            {
                return ViewState["EnumType"] as Type;
            }
            set
            {
                if (value.IsEnum)
                {
                    ViewState["EnumType"] = value;

                    ddlItems.Items.Clear();
                    foreach (object obj in Enum.GetValues(value))
                        ddlItems.Items.Add(new ListItem(AttributeHelper.GetEnumValueDescription(obj), obj.ToString()));
                }
            }
        }

        public bool AutoPostBack
        {
            get
            {
                return ddlItems.AutoPostBack;
            }
            set
            {
                ddlItems.AutoPostBack = value;
            }
        }

        public ListItem SelectedItem
        {
            get
            {
                return ddlItems.SelectedItem;
            }
        }

        public string SelectedValue
        {
            get
            {
                return ddlItems.SelectedValue;
            }
            set
            {
                ddlItems.SelectedValue = value;
            }
        }

        public object SelectedEnumValue
        {
            get
            {
                if (!string.IsNullOrEmpty(SelectedValue))
                    return Enum.Parse(EnumType, SelectedValue, true);

                return null;
            }
            set
            {
                if (value != null)
                    SelectedValue = value.ToString();
                else
                    SelectedValue = string.Empty;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return ddlItems.SelectedIndex;
            }
            set
            {
                ddlItems.SelectedIndex = value;
            }
        }

        public Unit Width
        {
            get
            {
                return ddlItems.Width;
            }
            set
            {
                ddlItems.Width = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ddlItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, EventArgs.Empty);
        }
    }
}