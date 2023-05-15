namespace ClubbyBook.Business
{
    using ClubbyBook.Common.Enums;

    public partial class SystemNotification
    {
        public NotificationSystemType Type
        {
            get
            {
                return (NotificationSystemType)sbType;
            }
            set
            {
                sbType = (sbyte)value;
            }
        }
    }
}