namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum NotificationSystemType : sbyte
    {
        [Description("Нет типа")]
        NotSpecified = 0,

        [Description("Необходимо добавить новую книгу")]
        NewBook = 1,
    }
}