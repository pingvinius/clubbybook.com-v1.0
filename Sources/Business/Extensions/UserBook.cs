namespace ClubbyBook.Business
{
    using ClubbyBook.Common.Enums;

    public partial class UserBook
    {
        public UserBookStatusType Status
        {
            get
            {
                return (UserBookStatusType)iStatus;
            }
            set
            {
                iStatus = (int)value;
            }
        }

        public UserBookOfferType Offer
        {
            get
            {
                return (UserBookOfferType)iOffer;
            }
            set
            {
                iOffer = (int)value;
            }
        }

        public UserBookType BookType
        {
            get
            {
                return (UserBookType)iBookType;
            }
            set
            {
                iBookType = (int)value;
            }
        }
    }
}