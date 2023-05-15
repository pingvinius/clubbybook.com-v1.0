namespace ClubbyBook.BackgroundActions.Mailing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using ClubbyBook.Common.Mail;

    public abstract class MailingAction : BackgroundAction
    {
        protected abstract IEnumerable<SendItem> CreateSendItems();

        #region BackgroundAction Implementation

        public override bool Execute()
        {
            var sendItems = CreateSendItems().ToArray();

            if (sendItems != null && sendItems.Length > 0)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    SendMailHelper.Send(sendItems);
                });
            }

            return true;
        }

        #endregion BackgroundAction Implementation
    }
}