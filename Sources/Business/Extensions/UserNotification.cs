namespace ClubbyBook.Business
{
    using ClubbyBook.Common.Enums;

    public partial class ConversationNotification
    {
        public NotificationDirectionType Direction
        {
            get
            {
                return (NotificationDirectionType)sbDirection;
            }
            set
            {
                sbDirection = (sbyte)value;
            }
        }
    }
}