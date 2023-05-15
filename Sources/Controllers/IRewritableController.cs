namespace ClubbyBook.Controllers
{
    using System.Data.Objects.DataClasses;

    public interface IRewritableController<EntityType> where EntityType : EntityObject
    {
        EntityType FindByUrlRewrite(string alias);
    }
}