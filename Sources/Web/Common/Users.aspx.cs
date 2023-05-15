namespace ClubbyBook.Web.Common
{
    using System;
    using ClubbyBook.Business;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Controls;
    using ClubbyBook.Web.Pages;

    public partial class Users : EntityListPage<User, UsersController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Присоеденись к сообществу любителей книг сегодня", UIUtilities.SiteBrandName);
            }
        }

        public int BookId
        {
            get
            {
                int id = -1;
                if (int.TryParse(Request.Params["bookId"], out id))
                    return id;
                return -1;
            }
        }

        public int Offer
        {
            get
            {
                int offer = -1;
                if (int.TryParse(Request.Params["offer"], out offer))
                    return offer;
                return -1;
            }
        }

        public int BookType
        {
            get
            {
                int bookType = -1;
                if (int.TryParse(Request.Params["bookType"], out bookType))
                    return bookType;
                return -1;
            }
        }

        public int CityId
        {
            get
            {
                int id = -1;
                if (int.TryParse(Request.Params["cityId"], out id))
                    return id;
                return -1;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AccessManagement.CanAdvancedUsersSearch)
                contentHeader.Actions.Add(new HeaderAction("Расширенный поиск", new ClientOnClickCode("expandAdvancedSearchBlock()")));
        }
    }
}