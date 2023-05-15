namespace ClubbyBook.Web.Account
{
    using System;
    using System.IO;
    using ClubbyBook.Business;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Pages;
    using ClubbyBook.Web.Utilities;

    public partial class EditProfile : EntityEditPage<Profile, ProfilesController>
    {
        public override string PageTitle
        {
            get
            {
                return string.Format("{0} - Редактирование профайла {1}", UIUtilities.SiteBrandName,
                  UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(Entity)));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            if (Entity.UserId != UserManagement.CurrentUser.Id)
                RedirectHelper.Redirect(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.UnauthorizedAccess));

            contentHeader.Title = string.Format("Редактирование профайла: {0}", UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(Entity)));

            if (!IsPostBack)
            {
                ddlGender.EnumType = typeof(GenderType);

                if (Entity != null)
                {
                    tbName.Text = Entity.Name.Trim();
                    tbSurname.Text = Entity.Surname.Trim();
                    tbNickname.Text = Entity.Nickname.Trim();
                    ddlGender.SelectedEnumValue = Entity.Gender;
                    hfCity.Value = Entity.CityId.ToString();
                }
            }
        }

        #region Override Members

        #endregion Override Members

        #region Page Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Entity == null)
                throw new ArgumentNullException("Entity");

            Entity.Name = tbName.Text;
            Entity.Surname = tbSurname.Text;
            Entity.Nickname = tbNickname.Text;
            Entity.Gender = (GenderType)ddlGender.SelectedEnumValue;

            int cityId = -1;
            if (int.TryParse(hfCity.Value, out cityId) && cityId != -1)
                Entity.CityId = cityId;
            else
                Entity.CityId = null;

            string fileTempPath = imageUploader.ImagePath;
            if (!string.IsNullOrEmpty(fileTempPath))
            {
                try
                {
                    string profileFileName = string.Format(Settings.ProfileAvatarFileName, Entity.Id);
                    string profilePath = Path.Combine(Settings.ImagesProfilesPath, profileFileName);

                    Entity.ImagePath = profilePath;

                    File.Copy(Server.MapPath(fileTempPath), Server.MapPath(profilePath), true);
                    File.Delete(Server.MapPath(fileTempPath));
                }
                catch (Exception ex)
                {
                    Entity.ImagePath = null;
                    Logger.Write(ex);
                }
            }

            EntityController.Update(Entity);

            RedirectHelper.Redirect(RedirectDirection.ViewProfile, Entity.UrlRewrite);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectHelper.Redirect(RedirectDirection.ViewProfile, Entity.UrlRewrite);
        }

        #endregion Page Events
    }
}