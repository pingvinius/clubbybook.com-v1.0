namespace ClubbyBook.Controllers
{
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;

    public interface IAutoCompletableController<EntityType> where EntityType : EntityObject
    {
        IList<EntityType> GetAutoCompleteList(string prefixText);
    }
}