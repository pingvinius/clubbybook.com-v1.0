namespace ClubbyBook.Common.SearchInfo
{
    using ClubbyBook.Common.Enums;

    public class SystemNotificationsSearchInfo : SearchInfoBase
    {
        public int? OwnerUserId { get; set; }

        public override void Clear()
        {
            base.Clear();

            OwnerUserId = null;
        }
    }

    public class ConversationNotificationsSearchInfo : SearchInfoBase
    {
        public NotificationDirectionType Direction { get; set; }

        public int? UserId { get; set; }

        public override void Clear()
        {
            base.Clear();

            Direction = NotificationDirectionType.NotSpecified;
            UserId = null;
        }
    }

    public class FeedbackNotificationsSearchInfo : SearchInfoBase
    {
    }
}