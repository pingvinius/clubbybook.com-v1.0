namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum UserBookStatusType : int
    {
        [Description("Не указано")]
        None = 0x0000,

        [Description("Читал(а)")]
        AlreadyRead = 0x0001,

        [Description("Читаю")]
        ReadingNow = 0x0010,

        [Description("Хочу прочесть")]
        WantToRead = 0x0100
    }
}