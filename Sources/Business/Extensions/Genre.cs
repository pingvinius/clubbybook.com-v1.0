namespace ClubbyBook.Business
{
    public partial class Genre
    {
        public string FullName
        {
            get
            {
                string fullName = Name;
                Genre parent = Parent;

                while (parent != null)
                {
                    fullName = string.Format("{0} - {1}", parent.Name, fullName);
                    parent = parent.Parent;
                }

                return fullName;
            }
        }
    }
}