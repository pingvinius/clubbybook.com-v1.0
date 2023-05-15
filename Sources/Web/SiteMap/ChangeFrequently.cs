namespace ClubbyBook.Web.SiteMap
{
    using System.ComponentModel;

    public enum ChangeFrequently
    {
        [Description("always")]
        Always,

        [Description("hourly")]
        Hourly,

        [Description("daily")]
        Daily,

        [Description("weekly")]
        Weekly,

        [Description("monthly")]
        Monthly,

        [Description("yearly")]
        Yearly,

        [Description("never")]
        Never
    }
}