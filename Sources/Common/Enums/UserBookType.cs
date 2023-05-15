namespace ClubbyBook.Common.Enums
{
    using System;
    using System.ComponentModel;

    [Flags]
    public enum UserBookType : int
    {
        [Description("Не указано")]
        None = 0x0000,

        [UrlRewrite("paper-book")]
        [Description("Печатная книга")]
        PaperBook = 0x0001,

        [UrlRewrite("e-book")]
        [Description("Электронная книга")]
        EBook = 0x0010,

        [UrlRewrite("audio-book")]
        [Description("Аудиокнига")]
        Audiobook = 0x0100,
    }
}