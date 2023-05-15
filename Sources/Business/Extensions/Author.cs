namespace ClubbyBook.Business
{
    using ClubbyBook.Common.Enums;

    public partial class Author
    {
        public AuthorType Type
        {
            get
            {
                return (AuthorType)sbType;
            }
            set
            {
                sbType = (sbyte)value;
            }
        }

        public GenderType Gender
        {
            get
            {
                return (GenderType)sbGender;
            }
            set
            {
                sbGender = (sbyte)value;
            }
        }
    }
}