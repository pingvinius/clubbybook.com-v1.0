namespace ClubbyBook.Web.Services
{
    using System.Collections.Generic;

    #region General

    public class SimpleListItem
    {
        public string text;
        public int value;

        public SimpleListItem() : this(string.Empty, -1) { }

        public SimpleListItem(string text, int value)
        {
            this.text = text;
            this.value = value;
        }
    }

    public class SearchItem
    {
        public string paramName { get; set; }

        public object paramValue { get; set; }
    }

    #endregion General

    #region Responses

    public enum ServiceResultType : int
    {
        OK = 0,
        Fail = 1
    }

    public enum ImageUploadResultType : byte
    {
        OK = 0,
        NoFile = 1,
        InvalidSize = 2,
        InvalidExtension = 3,
        Error = 4
    }

    public enum LoginResultType : int
    {
        OK = 0,
        Error = 1,
        AlreadyExist = 2,
        WasDeleted = 3
    }

    public enum AddBookResultType : int
    {
        OK = 0,
        AlreadyExists = 1,
        Fail = 2
    }

    public class ListResponse<T>
    {
        public List<T> items;
        public List<int> ids;
        public string searchParametersString;
        public string totalItemCountString;
        public bool moreItems;
    }

    #endregion Responses

    #region Books && Authors

    public class AuthorItem
    {
        public int id;
        public string fullName;
        public string seoImageAlt;
        public string yearsString;
        public string photoPath;
        public bool isPublishingHouse;
        public string typeString;
        public int booksCount;
        public string viewAuthorLink;
        public string editAuthorLink;
        public string viewAuthorBooksLink;
        public string restrictedDescription;
    }

    public class GenreItem
    {
        public int id;
        public string name;
        public string viewGenreBooksLink;
    }

    public class OfferItem
    {
        public int partialCount;
        public string partialLink;
        public int totalCount;
        public string totalLink;

        public OfferItem(int partialCount, string partialLink, int totalCount, string totalLink)
        {
            this.partialCount = partialCount;
            this.partialLink = partialLink;
            this.totalCount = totalCount;
            this.totalLink = totalLink;
        }
    }

    public class BookTypeItem
    {
        public int partialCount;
        public string partialLink;
        public int totalCount;
        public string totalLink;

        public BookTypeItem(int partialCount, string partialLink, int totalCount, string totalLink)
        {
            this.partialCount = partialCount;
            this.partialLink = partialLink;
            this.totalCount = totalCount;
            this.totalLink = totalLink;
        }
    }

    public class BookItem
    {
        public int id;
        public string title;
        public string originalTitle;
        public string description;
        public string restrictedDescription;
        public string coverPath;
        public bool confirmed;
        public bool collection;
        public string seoImageAlt;
        public int status;
        public int offer;
        public int type;
        public string viewBookLink;
        public string editBookLink;
        public bool containsInUserLibrary;
        public bool containsInOtherUserLibraryFromSameCity;
        public BookTypeItem paperBookType;
        public BookTypeItem eBookType;
        public BookTypeItem audiobookType;
        public OfferItem anyOffer;
        public OfferItem sellOffer;
        public OfferItem buyOffer;
        public OfferItem barterOffer;
        public OfferItem willGiveReadOffer;
        public OfferItem willGrantGratisOffer;
        public string userComment;
        public string authorsAndTitle;
        public List<AuthorItem> authors;
        public List<GenreItem> genres;
    }

    public class CommentItem
    {
        public int id;
        public string message;
        public string userFullName;
        public string userPhotoPath;
        public string viewUserLink;
        public string seoImageAlt;
        public string createdDate;
        public bool isUserComment;
    }

    #endregion Books && Authors

    #region Profiles

    public class ProfileItem
    {
        public int userId;
        public string fullName;
        public string nickname;
        public string genderName;
        public string cityName;
        public string photoPath;
        public int userBookCount;
        public string viewProfileLink;
        public string seoImageAlt;
    }

    #endregion Profiles

    #region Notifications

    public class SystemNotificationItem
    {
        public int id;
        public string typeString;
        public bool isNew;
        public string content;
        public string createdDate;

        public string ownerUserFullName;
        public string ownerUserPhotoPath;
        public string ownerUserLink;
    }

    public class FeedbackNotificationItem
    {
        public int id;
        public bool isNew;
        public string content;
        public string createdDate;

        public bool isAnonymous;
        public string fromUserFullName;
        public string fromUserPhotoPath;
        public string fromUserLink;
    }

    public class ConversationNotificationItem
    {
        public int id;
        public string directionString;
        public bool isNew;
        public string content;
        public bool canReply;
        public string createdDate;

        public string fromUserFullName;
        public string fromUserPhotoPath;
        public string fromUserLink;
        public int fromUserId;

        public string toUserLink;
        public string toUserFullName;
        public string toUserPhotoPath;
        public int toUserId;
    }

    #endregion Notifications

    #region News

    public class NewsItem
    {
        public int id;
        public string title;
        public string restrictedMessage;
        public string createdDateFull;
        public string createdDateShort;
        public string viewNewsLink;
        public string editNewsLink;
    }

    #endregion News
}