namespace ClubbyBook.Business
{
    public interface IContextHolder
    {
        bool IsAvailable();

        bool Contains();

        object Get();

        void Set(object context);

        void Remove();
    }
}