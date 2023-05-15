namespace ClubbyBook.Common.Enums
{
    using System;
    using System.ComponentModel;

    [Flags]
    public enum UserBookOfferType : int
    {
        [UrlRewrite("any-offer")]
        [Description("Любое")]
        None = 0x0000,

        [UrlRewrite("sell")]
        [Description("Продам")]
        Sell = 0x00000001,

        [UrlRewrite("buy")]
        [Description("Куплю")]
        Buy = 0x00000010,

        [UrlRewrite("barter")]
        [Description("Обменяю")]
        Barter = 0x00000100,

        [UrlRewrite("will-give-read")]
        [Description("Дам прочесть")]
        WillGiveRead = 0x00001000,

        [UrlRewrite("will-grant-gratis")]
        [Description("Отдам безвозмездно")]
        WillGrantGratis = 0x00010000
    }
}