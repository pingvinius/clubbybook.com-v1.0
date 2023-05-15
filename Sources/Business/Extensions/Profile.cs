namespace ClubbyBook.Business
{
    using ClubbyBook.Common.Enums;

    public partial class Profile
    {
        public GenderType Gender
        {
            get
            {
                if (sbGender.HasValue)
                    return (GenderType)sbGender.Value;
                return GenderType.NotSpecified;
            }
            set
            {
                sbGender = (sbyte)value;
            }
        }
    }
}