namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum NotificationDirectionType : sbyte
    {
        [Description("Без направления")]
        NotSpecified = 0,

        [Description("Входящие")]
        Input = 1,

        [Description("Исходящие")]
        Output = 2,
    }
}