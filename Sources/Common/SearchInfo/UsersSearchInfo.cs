namespace ClubbyBook.Common.SearchInfo
{
    using System.Collections.Generic;
    using ClubbyBook.Common.Enums;

    public class UsersSearchInfo : SearchInfoBase
    {
        public int? BookId { get; set; }

        public int? CityId { get; set; }

        public UserBookOfferType? Offer { get; set; }

        public UserBookType? BookType { get; set; }

        public List<string> Roles { get; set; }

        public override void Clear()
        {
            base.Clear();

            BookId = null;
            CityId = null;
            Offer = null;
            BookType = null;
            Roles = null;
        }
    }
}