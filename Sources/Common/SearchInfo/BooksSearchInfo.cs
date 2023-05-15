namespace ClubbyBook.Common.SearchInfo
{
    using System.Collections.Generic;
    using ClubbyBook.Common.Enums;

    public class BooksSearchInfo : SearchInfoBase
    {
        public int? UserId { get; set; }

        public List<int> CitiesIds { get; set; }

        public List<int> AuthorsIds { get; set; }

        public int? GenreId { get; set; }

        public UserBookOfferType? Offer { get; set; }

        public UserBookType? BookType { get; set; }

        public UserBookStatusType? Status { get; set; }

        public BookProgressType BookProgress { get; set; }

        public BookCollectionsOrNotType BookCollectionsOrNot { get; set; }

        public override void Clear()
        {
            base.Clear();

            UserId = null;
            CitiesIds = null;
            AuthorsIds = null;
            GenreId = null;
            Offer = null;
            BookType = null;
            Status = null;
            BookProgress = BookProgressType.Proven;
            BookCollectionsOrNot = BookCollectionsOrNotType.DoesnotMatter;
        }
    }
}