namespace ClubbyBook.Common.Enums
{
    using System.ComponentModel;

    public enum BookCollectionsOrNotType : int
    {
        [Description("Неважно")]
        DoesnotMatter = 0,

        [Description("Только сбоники")]
        CollectionsOnly = 1,

        [Description("Исключить сборники")]
        ExcludeCollections = 2
    }
}