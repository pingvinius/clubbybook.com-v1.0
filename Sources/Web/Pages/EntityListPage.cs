namespace ClubbyBook.Web.Pages
{
    using System.Data.Objects.DataClasses;
    using ClubbyBook.Controllers;

    public class EntityListPage<EntityType, EntityControllerType> : SimplePage
        where EntityType : EntityObject
        where EntityControllerType : IEntityController<EntityType>
    {
        #region Fields

        #endregion Fields

        #region Properties

        protected EntityControllerType EntityController
        {
            get
            {
                return ControllerFactory.GetInstance<EntityType, EntityControllerType>();
            }
        }

        #endregion Properties

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}