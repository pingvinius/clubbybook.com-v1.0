namespace ClubbyBook.Web.Pages
{
    using System.Data.Objects.DataClasses;
    using ClubbyBook.Controllers;

    public abstract class EntityViewPage<EntityType, EntityControllerType> : SimplePage
        where EntityType : EntityObject
        where EntityControllerType : IEntityController<EntityType>
    {
        #region Fields

        private EntityType entity;

        #endregion Fields

        #region Properties

        protected EntityType Entity
        {
            get
            {
                if (entity == null && ViewId.HasValue)
                    entity = EntityController.Load(ViewId.Value);

                return entity;
            }
        }

        protected EntityControllerType EntityController
        {
            get
            {
                return ControllerFactory.GetInstance<EntityType, EntityControllerType>();
            }
        }

        protected int? ViewId
        {
            get
            {
                int id = 0;
                if (int.TryParse(Request.Params["Id"], out id))
                    return id;
                return null;
            }
        }

        #endregion Properties
    }
}