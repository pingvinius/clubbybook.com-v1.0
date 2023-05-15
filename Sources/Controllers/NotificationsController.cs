namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ClubbyBook.Business;
    using ClubbyBook.Common.Enums;
    using ClubbyBook.Common.SearchInfo;
    using ClubbyBook.Common.Utilities;

    public sealed class NotificationsController : IEntityController<Notification>
    {
        public int GetNewNotificationCount(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            int newNotificationsCount = 0;

            bool calcFeedbackNotifications = ControllerFactory.UsersController.IsUserInRole(user, UserManagement.AdminRoleName);
            bool calcSystemNotifications = ControllerFactory.UsersController.IsUserInRole(user, UserManagement.AdminRoleName) ||
              ControllerFactory.UsersController.IsUserInRole(user, UserManagement.EditorRoleName);

            if (calcSystemNotifications)
            {
                newNotificationsCount +=
                    (from SystemNotification sn in ContextManager.Current.SystemNotifications
                     where sn.IsNew
                     select sn).Count();
            }

            if (calcFeedbackNotifications)
            {
                newNotificationsCount +=
                    (from FeedbackNotification fn in ContextManager.Current.FeedbackNotifications
                     where fn.IsNew
                     select fn).Count();
            }

            newNotificationsCount +=
                (from ConversationNotification cn in ContextManager.Current.ConversationNotifications
                 where cn.ToUserId == user.Id && cn.IsNew && cn.sbDirection == (sbyte)NotificationDirectionType.Input
                 select cn).Count();

            return newNotificationsCount;
        }

        public SystemNotification LoadSystem(int id)
        {
            return ContextManager.Current.SystemNotifications.SingleOrDefault<SystemNotification>(sn => sn.Id == id);
        }

        public FeedbackNotification LoadFeedback(int id)
        {
            return ContextManager.Current.FeedbackNotifications.SingleOrDefault<FeedbackNotification>(fn => fn.Id == id);
        }

        public ConversationNotification LoadConversation(int id)
        {
            return ContextManager.Current.ConversationNotifications.SingleOrDefault<ConversationNotification>(cn => cn.Id == id);
        }

        public void AddNewBookNotification(User ownerUser, Book book, string message)
        {
            if (ownerUser == null)
            {
                throw new ArgumentNullException("ownerUser");
            }

            if (book == null)
            {
                throw new ArgumentNullException("book");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            Notification notification = new Notification();
            notification.Message = message;
            notification.CreatedDate = DateTimeHelper.Now;
            ContextManager.Current.AddToNotifications(notification);

            SystemNotification systemNotification = new SystemNotification();
            systemNotification.IsNew = true;
            systemNotification.Notification = notification;
            systemNotification.Type = NotificationSystemType.NewBook;
            systemNotification.OwnerUser = ownerUser;
            ContextManager.Current.AddToSystemNotifications(systemNotification);

            ContextManager.Current.SaveChanges();
        }

        public void AddNewFeedbackNotification(User ownerUser, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            Notification notification = new Notification();
            notification.Message = message;
            notification.CreatedDate = DateTimeHelper.Now;
            ContextManager.Current.AddToNotifications(notification);

            FeedbackNotification feedbackNotification = new FeedbackNotification();
            feedbackNotification.IsNew = true;
            feedbackNotification.Notification = notification;
            feedbackNotification.OwnerUser = ownerUser;
            ContextManager.Current.AddToFeedbackNotifications(feedbackNotification);

            ContextManager.Current.SaveChanges();
        }

        public void AddConversationNotification(User fromUser, User toUser, string message)
        {
            if (fromUser == null)
            {
                throw new ArgumentNullException("fromUser");
            }

            if (toUser == null)
            {
                throw new ArgumentNullException("toUser");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            Notification notification = new Notification();
            notification.Message = message;
            notification.CreatedDate = DateTimeHelper.Now;
            ContextManager.Current.AddToNotifications(notification);

            ConversationNotification ownerConversationNotification = new ConversationNotification();
            ownerConversationNotification.IsNew = false;
            ownerConversationNotification.Notification = notification;
            ownerConversationNotification.Direction = NotificationDirectionType.Output;
            ownerConversationNotification.FromUser = fromUser;
            ownerConversationNotification.ToUser = toUser;
            ContextManager.Current.AddToConversationNotifications(ownerConversationNotification);

            ConversationNotification targetConversationNotification = new ConversationNotification();
            targetConversationNotification.IsNew = true;
            targetConversationNotification.Notification = notification;
            targetConversationNotification.Direction = NotificationDirectionType.Input;
            targetConversationNotification.FromUser = fromUser;
            targetConversationNotification.ToUser = toUser;
            ContextManager.Current.AddToConversationNotifications(targetConversationNotification);

            ContextManager.Current.SaveChanges();
        }

        public void MarkAsReadNotification(SystemNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            notification.IsNew = false;

            ContextManager.Current.SaveChanges();
        }

        public void MarkAsReadNotification(FeedbackNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            notification.IsNew = false;

            ContextManager.Current.SaveChanges();
        }

        public void MarkAsReadNotification(ConversationNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            notification.IsNew = false;

            ContextManager.Current.SaveChanges();
        }

        public void DeleteNotification(SystemNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            ContextManager.Current.DeleteObject(notification);
            ContextManager.Current.SaveChanges();
        }

        public void DeleteNotification(FeedbackNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            ContextManager.Current.DeleteObject(notification);
            ContextManager.Current.SaveChanges();
        }

        public void DeleteNotification(ConversationNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            ContextManager.Current.DeleteObject(notification);
            ContextManager.Current.SaveChanges();
        }

        #region Search Routine

        public IList<SystemNotification> SearchSystem(SystemNotificationsSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
            {
                throw new ArgumentNullException("searchInfo");
            }

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from SystemNotification sn in ContextManager.Current.SystemNotifications
                         where !searchInfo.ExistentIds.Contains(sn.Id) &&
                               sn.Notification.Message.ToLower().Contains(searchInfo.SearchText)
                         select sn;

            if (searchInfo.OwnerUserId.HasValue)
            {
                result = from SystemNotification sn in result
                         where sn.OwnerUserId == searchInfo.OwnerUserId
                         select sn;
            }

            totalCount = searchInfo.ExistentIds.Count == 0 ? result.Count() : -1;

            return result
              .OrderByDescending(sn => sn.IsNew)
              .ThenByDescending(sn => sn.Notification.CreatedDate)
              .Take(searchInfo.ReturnCount)
              .ToList();
        }

        public IList<FeedbackNotification> SearchFeedback(FeedbackNotificationsSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
            {
                throw new ArgumentNullException("searchInfo");
            }

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from FeedbackNotification fn in ContextManager.Current.FeedbackNotifications
                         where !searchInfo.ExistentIds.Contains(fn.Id) && fn.Notification.Message.ToLower().Contains(searchInfo.SearchText)
                         orderby fn.IsNew descending, fn.Notification.CreatedDate descending
                         select fn;

            totalCount = searchInfo.ExistentIds.Count == 0 ? result.Count() : -1;

            return result
              .Take(searchInfo.ReturnCount)
              .ToList();
        }

        public IList<ConversationNotification> SearchConversation(ConversationNotificationsSearchInfo searchInfo, out int totalCount)
        {
            if (searchInfo == null)
            {
                throw new ArgumentNullException("searchInfo");
            }

            searchInfo.SearchText = searchInfo.SearchText.Trim().ToLower();

            var result = from ConversationNotification cn in ContextManager.Current.ConversationNotifications
                         where !searchInfo.ExistentIds.Contains(cn.Id) &&
                               cn.Notification.Message.ToLower().Contains(searchInfo.SearchText)
                         select cn;

            if (searchInfo.Direction != NotificationDirectionType.NotSpecified && !searchInfo.UserId.HasValue)
            {
                result = from ConversationNotification cn in result
                         where cn.sbDirection == (sbyte)searchInfo.Direction
                         select cn;
            }
            else if (searchInfo.Direction != NotificationDirectionType.NotSpecified && searchInfo.UserId.HasValue)
            {
                if (searchInfo.Direction == NotificationDirectionType.Input)
                {
                    result = from ConversationNotification cn in result
                             where (cn.ToUserId == searchInfo.UserId && cn.sbDirection == (sbyte)NotificationDirectionType.Input)
                             select cn;
                }
                else if (searchInfo.Direction == NotificationDirectionType.Output)
                {
                    result = from ConversationNotification cn in result
                             where (cn.FromUserId == searchInfo.UserId && cn.sbDirection == (sbyte)NotificationDirectionType.Output)
                             select cn;
                }
            }
            else if (searchInfo.UserId.HasValue)
            {
                result = from ConversationNotification cn in result
                         where (cn.FromUserId == searchInfo.UserId && cn.sbDirection == (sbyte)NotificationDirectionType.Output) ||
                               (cn.ToUserId == searchInfo.UserId && cn.sbDirection == (sbyte)NotificationDirectionType.Input)
                         select cn;
            }

            totalCount = searchInfo.ExistentIds.Count == 0 ? result.Count() : -1;

            return result
              .OrderByDescending(cn => cn.IsNew)
              .ThenByDescending(cn => cn.Notification.CreatedDate)
              .Take(searchInfo.ReturnCount)
              .ToList();
        }

        #endregion Search Routine

        #region IEntityController<Notification> Members

        public IList<Notification> Load()
        {
            var result = from Notification notification in ContextManager.Current.Notifications
                         select notification;

            return result.ToList<Notification>();
        }

        public Notification Load(int id)
        {
            return ContextManager.Current.Notifications.SingleOrDefault<Notification>(notification => notification.Id == id);
        }

        public void Update(Notification entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ContextManager.Current.SaveChanges();
        }

        public void Delete(Notification entity)
        {
            // UserNotifications and SystemNotifications should be removed here too
            ContextManager.Current.DeleteObject(entity);
            ContextManager.Current.SaveChanges();
        }

        #endregion IEntityController<Notification> Members
    }
}