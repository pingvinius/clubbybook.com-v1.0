namespace ClubbyBook.Common.SearchInfo
{
    public class CommentSearchInfo : SearchInfoBase
    {
        public int? BookId { get; set; }

        public int? AuthorId { get; set; }

        public override void Clear()
        {
            base.Clear();

            this.BookId = null;
            this.AuthorId = null;
        }
    }
}