namespace ClubbyBook.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Services;
    using ClubbyBook.BackgroundActions;
    using ClubbyBook.BackgroundActions.Mailing;
    using ClubbyBook.Business;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.Logging;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;
    using ClubbyBook.Controllers;
    using ClubbyBook.UI;
    using ClubbyBook.Web.Utilities;

    [WebService(Namespace = "http://clubbybook.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class NotificationsService : WebService
    {
        [WebMethod]
        [ScriptMethod]
        public int GetNewNotificationCount(int userId)
        {
            User user = ControllerFactory.UsersController.Load(userId);
            if (user == null)
                throw new ArgumentException("userId");

            if (user == UserManagement.CurrentUser)
                return ControllerFactory.NotificationsController.GetNewNotificationCount(user);
            return 0;
        }

        #region System Notifications Routine

        [WebMethod]
        [ScriptMethod]
        public ListResponse<SystemNotificationItem> GetSystemNotifications(IList<SearchItem> searchParameters)
        {
            if (!AccessManagement.CanExploitSystemNotifications)
                throw new UnauthorizedAccessException();

            SystemNotificationsSearchInfo searchInfo;

            if (ParseSystemNotificationsSearchParameters(searchParameters, out searchInfo))
                return GetSystemNotificationsInternal(searchInfo);

            return GetSystemNotificationsInternal(new SystemNotificationsSearchInfo());
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType MarkAsReadSystemNotification(int systemNotificationId)
        {
            if (!AccessManagement.CanExploitSystemNotifications)
                throw new UnauthorizedAccessException();

            SystemNotification sn = ControllerFactory.NotificationsController.LoadSystem(systemNotificationId);
            if (sn != null)
            {
                ControllerFactory.NotificationsController.MarkAsReadNotification(sn);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveSystemNotification(int systemNotificationId)
        {
            if (!AccessManagement.CanExploitSystemNotifications)
                throw new UnauthorizedAccessException();

            SystemNotification sn = ControllerFactory.NotificationsController.LoadSystem(systemNotificationId);
            if (sn != null)
            {
                ControllerFactory.NotificationsController.DeleteNotification(sn);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        private ListResponse<SystemNotificationItem> GetSystemNotificationsInternal(SystemNotificationsSearchInfo searchInfo)
        {
            ListResponse<SystemNotificationItem> response = new ListResponse<SystemNotificationItem>();
            response.items = new List<SystemNotificationItem>();
            response.ids = new List<int>();
            response.searchParametersString = string.Empty;

            int totalCount = 0;
            foreach (SystemNotification sn in ControllerFactory.NotificationsController.SearchSystem(searchInfo, out totalCount))
            {
                response.items.Add(CreateSystemNotificationItem(sn));
                response.ids.Add(sn.Id);
            }

            response.totalItemCountString = PrepareItemCountString(totalCount);
            response.moreItems = response.items.Count == searchInfo.ReturnCount;

            return response;
        }

        private bool ParseSystemNotificationsSearchParameters(IList<SearchItem> searchParameters, out SystemNotificationsSearchInfo info)
        {
            info = new SystemNotificationsSearchInfo();

            try
            {
                foreach (var item in searchParameters)
                {
                    switch (item.paramName.ToLower())
                    {
                        case "existentid":
                            info.ExistentIds.Add((int)item.paramValue);
                            break;

                        case "searchtext":
                            info.SearchText = item.paramValue as string;
                            break;

                        case "owneruserid":
                            info.OwnerUserId = (int)item.paramValue;
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private SystemNotificationItem CreateSystemNotificationItem(SystemNotification item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            SystemNotificationItem sni = new SystemNotificationItem();

            sni.id = item.Id;
            sni.content = UIUtilities.PrepareTextContent(item.Notification.Message);
            sni.createdDate = UIUtilities.GetFullDateString(item.Notification.CreatedDate);
            sni.isNew = item.IsNew;
            sni.typeString = AttributeHelper.GetEnumValueDescription(item.Type);

            Profile ownerUserProfile = ControllerFactory.ProfilesController.GetProfile(item.OwnerUser);
            if (ownerUserProfile != null)
            {
                sni.ownerUserFullName = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(ownerUserProfile));
                sni.ownerUserLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewProfile, ownerUserProfile.UrlRewrite);
                sni.ownerUserPhotoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(ownerUserProfile.ImagePath, Settings.EmptyProfileAvatarPath));
            }

            return sni;
        }

        #endregion System Notifications Routine

        #region Feedback Notifications Routine

        [WebMethod]
        [ScriptMethod]
        public ListResponse<FeedbackNotificationItem> GetFeedbackNotifications(IList<SearchItem> searchParameters)
        {
            if (!AccessManagement.CanExploitFeedbackNotifications)
                throw new UnauthorizedAccessException();

            FeedbackNotificationsSearchInfo searchInfo;

            if (ParseFeedbackNotificationsSearchParameters(searchParameters, out searchInfo))
                return GetFeedbackNotificationsInternal(searchInfo);

            return GetFeedbackNotificationsInternal(new FeedbackNotificationsSearchInfo());
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType MarkAsReadFeedbackNotification(int feedbackNotificationId)
        {
            if (!AccessManagement.CanExploitFeedbackNotifications)
                throw new UnauthorizedAccessException();

            FeedbackNotification fn = ControllerFactory.NotificationsController.LoadFeedback(feedbackNotificationId);
            if (fn != null)
            {
                ControllerFactory.NotificationsController.MarkAsReadNotification(fn);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveFeedbackNotification(int feedbackNotificationId)
        {
            if (!AccessManagement.CanExploitFeedbackNotifications)
                throw new UnauthorizedAccessException();

            FeedbackNotification fn = ControllerFactory.NotificationsController.LoadFeedback(feedbackNotificationId);
            if (fn != null)
            {
                ControllerFactory.NotificationsController.DeleteNotification(fn);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        private ListResponse<FeedbackNotificationItem> GetFeedbackNotificationsInternal(FeedbackNotificationsSearchInfo searchInfo)
        {
            ListResponse<FeedbackNotificationItem> response = new ListResponse<FeedbackNotificationItem>();
            response.items = new List<FeedbackNotificationItem>();
            response.ids = new List<int>();
            response.searchParametersString = string.Empty;

            int totalCount = 0;
            foreach (FeedbackNotification fn in ControllerFactory.NotificationsController.SearchFeedback(searchInfo, out totalCount))
            {
                response.items.Add(CreateFeedbackNotificationItem(fn));
                response.ids.Add(fn.Id);
            }

            response.totalItemCountString = PrepareItemCountString(totalCount);
            response.moreItems = response.items.Count == searchInfo.ReturnCount;

            return response;
        }

        private bool ParseFeedbackNotificationsSearchParameters(IList<SearchItem> searchParameters, out FeedbackNotificationsSearchInfo info)
        {
            info = new FeedbackNotificationsSearchInfo();

            try
            {
                foreach (var item in searchParameters)
                {
                    switch (item.paramName.ToLower())
                    {
                        case "existentid":
                            info.ExistentIds.Add((int)item.paramValue);
                            break;

                        case "searchtext":
                            info.SearchText = item.paramValue as string;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private FeedbackNotificationItem CreateFeedbackNotificationItem(FeedbackNotification item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            FeedbackNotificationItem fni = new FeedbackNotificationItem();

            fni.id = item.Id;
            fni.content = UIUtilities.PrepareTextContent(item.Notification.Message);
            fni.createdDate = UIUtilities.GetFullDateString(item.Notification.CreatedDate);
            fni.isNew = item.IsNew;
            fni.isAnonymous = item.OwnerUser == null;

            Profile ownerUserProfile = null;
            if (!fni.isAnonymous)
                ownerUserProfile = ControllerFactory.ProfilesController.GetProfile(item.OwnerUser);

            if (ownerUserProfile != null)
            {
                fni.fromUserFullName = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(ownerUserProfile));
                fni.fromUserLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewProfile, ownerUserProfile.UrlRewrite);
                fni.fromUserPhotoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(ownerUserProfile.ImagePath, Settings.EmptyProfileAvatarPath));
            }
            else
            {
                fni.fromUserFullName = "Анонимус";
                fni.fromUserLink = string.Empty;
                fni.fromUserPhotoPath = VirtualPathUtility.ToAbsolute(Settings.AnonymousProfileAvatarPath);
            }

            return fni;
        }

        #endregion Feedback Notifications Routine

        #region Conversation Notifications Routine

        [WebMethod]
        [ScriptMethod]
        public ListResponse<ConversationNotificationItem> GetConversationNotifications(IList<SearchItem> searchParameters)
        {
            if (!AccessManagement.CanExploitConversationNotifications)
                throw new UnauthorizedAccessException();

            ConversationNotificationsSearchInfo searchInfo;

            if (ParseConversationNotificationsSearchParameters(searchParameters, out searchInfo))
            {
                if (searchInfo.UserId != UserManagement.CurrentUser.Id)
                    throw new UnauthorizedAccessException();

                return GetConversationNotificationsInternal(searchInfo);
            }

            return GetConversationNotificationsInternal(new ConversationNotificationsSearchInfo());
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType MarkAsReadConversationNotification(int conversationNotificationId)
        {
            if (!AccessManagement.CanExploitConversationNotifications)
                throw new UnauthorizedAccessException();

            ConversationNotification cn = ControllerFactory.NotificationsController.LoadConversation(conversationNotificationId);
            if (cn != null)
            {
                if ((cn.Direction == NotificationDirectionType.Input && cn.ToUser != UserManagement.CurrentUser) ||
                  (cn.Direction == NotificationDirectionType.Output && cn.FromUser != UserManagement.CurrentUser))
                    throw new UnauthorizedAccessException();

                ControllerFactory.NotificationsController.MarkAsReadNotification(cn);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType RemoveConversationNotification(int conversationNotificationId)
        {
            if (!AccessManagement.CanExploitConversationNotifications)
                throw new UnauthorizedAccessException();

            ConversationNotification cn = ControllerFactory.NotificationsController.LoadConversation(conversationNotificationId);
            if (cn != null)
            {
                if ((cn.Direction == NotificationDirectionType.Input && cn.ToUser != UserManagement.CurrentUser) ||
                  (cn.Direction == NotificationDirectionType.Output && cn.FromUser != UserManagement.CurrentUser))
                    throw new UnauthorizedAccessException();

                ControllerFactory.NotificationsController.DeleteNotification(cn);
                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        [WebMethod]
        [ScriptMethod]
        public ServiceResultType SendConversationMessage(int fromUserId, int toUserId, string message)
        {
            // TODO: validate strings and trim it

            User fromUser = ControllerFactory.UsersController.Load(fromUserId);
            User toUser = ControllerFactory.UsersController.Load(toUserId);

            if (fromUser != null && toUser != null)
            {
                if (fromUser != UserManagement.CurrentUser)
                    throw new UnauthorizedAccessException();

                ControllerFactory.NotificationsController.AddConversationNotification(fromUser, toUser, message);

                BackgroundActionManager.Instance.ExecuteAction(new SendConversationNotificationMailAction(fromUser, toUser, message));

                return ServiceResultType.OK;
            }

            return ServiceResultType.Fail;
        }

        private ListResponse<ConversationNotificationItem> GetConversationNotificationsInternal(ConversationNotificationsSearchInfo searchInfo)
        {
            if (!searchInfo.UserId.HasValue || searchInfo.UserId != UserManagement.CurrentUser.Id)
                throw new UnauthorizedAccessException();

            ListResponse<ConversationNotificationItem> response = new ListResponse<ConversationNotificationItem>();
            response.items = new List<ConversationNotificationItem>();
            response.ids = new List<int>();
            response.searchParametersString = string.Empty;

            int totalCount = 0;
            foreach (ConversationNotification sn in ControllerFactory.NotificationsController.SearchConversation(searchInfo, out totalCount))
            {
                response.items.Add(CreateConversationNotificationItem(sn));
                response.ids.Add(sn.Id);
            }

            response.totalItemCountString = PrepareItemCountString(totalCount);
            response.moreItems = response.items.Count == searchInfo.ReturnCount;

            return response;
        }

        private bool ParseConversationNotificationsSearchParameters(IList<SearchItem> searchParameters, out ConversationNotificationsSearchInfo info)
        {
            info = new ConversationNotificationsSearchInfo();

            try
            {
                foreach (var item in searchParameters)
                {
                    switch (item.paramName.ToLower())
                    {
                        case "existentid":
                            info.ExistentIds.Add((int)item.paramValue);
                            break;

                        case "searchtext":
                            info.SearchText = item.paramValue as string;
                            break;

                        case "userid":
                            info.UserId = (int)item.paramValue;
                            break;

                        case "direction":
                            info.Direction = (NotificationDirectionType)((sbyte)item.paramValue);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private ConversationNotificationItem CreateConversationNotificationItem(ConversationNotification item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            ConversationNotificationItem cni = new ConversationNotificationItem();

            cni.id = item.Id;
            cni.content = UIUtilities.PrepareTextContent(item.Notification.Message);
            cni.createdDate = UIUtilities.GetFullDateString(item.Notification.CreatedDate);
            cni.directionString = AttributeHelper.GetEnumValueDescription(item.Direction);
            cni.canReply = item.Direction == NotificationDirectionType.Input;
            cni.isNew = item.IsNew;

            Profile fromUserProfile = ControllerFactory.ProfilesController.GetProfile(item.FromUser);
            if (fromUserProfile != null)
            {
                cni.fromUserId = item.FromUserId;
                cni.fromUserFullName = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(fromUserProfile));
                cni.fromUserLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewProfile, fromUserProfile.UrlRewrite);
                cni.fromUserPhotoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(fromUserProfile.ImagePath, Settings.EmptyProfileAvatarPath));
            }

            Profile toUserProfile = ControllerFactory.ProfilesController.GetProfile(item.ToUser);
            if (toUserProfile != null)
            {
                cni.toUserId = toUserProfile.UserId;
                cni.toUserFullName = UIUtilities.ValidateStringValue(UIUtilities.GetProfileFullName(toUserProfile));
                cni.toUserLink = RedirectHelper.ResolveUrl(RedirectDirection.ViewProfile, toUserProfile.UrlRewrite);
                cni.toUserPhotoPath = VirtualPathUtility.ToAbsolute(UIUtilities.ValidateImagePath(toUserProfile.ImagePath, Settings.EmptyProfileAvatarPath));
            }

            return cni;
        }

        #endregion Conversation Notifications Routine

        #region Common Routine

        private string PrepareItemCountString(int totalCount)
        {
            if (totalCount <= 0)
                return string.Empty;

            var totalCountToCheck = totalCount % 100;

            if (totalCountToCheck >= 5 && totalCountToCheck <= 20)
                return string.Format("В результате поиска найдено {0} сообщений.", totalCount);
            else
            {
                totalCountToCheck = totalCountToCheck % 10;

                if (totalCountToCheck == 1)
                    return string.Format("В результате поиска найдено {0} сообщение.", totalCount);
                else if (totalCountToCheck >= 2 && totalCountToCheck <= 4)
                    return string.Format("В результате поиска найдено {0} сообщения.", totalCount);
                else
                    return string.Format("В результате поиска найдено {0} сообщений.", totalCount);
            }
        }

        #endregion Common Routine
    }
}