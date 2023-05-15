namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum BookProgressType : int
    {
        [Description("Проверенные")]
        Proven = 0,

        [Description("Все")]
        All = 1,

        [Description("Непроверенные")]
        Unproven = 2
    }
}