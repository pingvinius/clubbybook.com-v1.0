namespace ClubbyBook.Common.SearchInfo
{
    using System.Collections.Generic;

    public class SearchInfoBase
    {
        public const int DefaultReturnCount = 10;

        public string SearchText { get; set; }

        public List<int> ExistentIds { get; set; }

        public int ReturnCount { get; set; }

        public SearchInfoBase()
        {
            Clear();
        }

        public virtual void Clear()
        {
            SearchText = string.Empty;
            ExistentIds = new List<int>();
            ReturnCount = DefaultReturnCount;
        }
    }
}