namespace ClubbyBook.Web.Controls
{
    using System;

    public partial class InfoMessageControl : System.Web.UI.UserControl
    {
        public InfoMessageType Type { get; set; }

        public string CustomText { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialization
            Type = InfoMessageType.None;
            CustomText = string.Empty;
        }
    }

    public enum InfoMessageType
    {
        None,
        ViewBookRegistrationMessage,
        ViewAuthorRegistrationMessage,
        ViewProfileFillFieldsMessage,
        ViewBookPleaseRateBook,
        ViewAuthorPleaseRateAuthor,
        AddCommentRequirements,
        CustomText,
    }
}