namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum AuthorType : int
    {
        [UrlRewrite("type-none")]
        [Description("Не указано")]
        NotSpecified = 0,

        [UrlRewrite("type-man")]
        [Description("Человек")]
        Man = 1,

        [UrlRewrite("type-publishing-house")]
        [Description("Издательство")]
        PublishingHouse = 2,
    }
}