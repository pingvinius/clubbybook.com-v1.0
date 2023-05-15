namespace ClubbyBook.Controllers
{
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;

    public interface IEntityController<EntityType> where EntityType : EntityObject
    {
        IList<EntityType> Load();

        EntityType Load(int id);

        void Update(EntityType entity);

        void Delete(EntityType entity);
    }
}