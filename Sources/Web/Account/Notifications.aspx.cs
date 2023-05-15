namespace ClubbyBook.Web.Account
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;

    public partial class Notifications : EntityListPage<Notification, NotificationsController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Уведомления", UIUtilities.SiteBrandName);
            }
        }

        public bool ConversationNotificationExpandable
        {
            get
            {
                return AccessManagement.CanExploitConversationNotifications && (AccessManagement.CanExploitFeedbackNotifications || AccessManagement.CanExploitSystemNotifications);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ConversationNotificationExpandable)
                conversationNotificationsContentHeader.ExpandCollapseClientFunc = "expandCollapseConversationNotifications()";
        }
    }
}