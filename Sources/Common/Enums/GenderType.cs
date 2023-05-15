namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum GenderType : sbyte
    {
        [Description("Не указано")]
        NotSpecified = 0,

        [Description("Мужской")]
        Male = 1,

        [Description("Женский")]
        Female = 2
    }
}