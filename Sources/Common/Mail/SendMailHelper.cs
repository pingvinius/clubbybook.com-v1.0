namespace ClubbyBook.Common.Mail
{
    using System;
    using System.Net.Mail;
    using System.Text;
    using ClubbyBook.Common.Logging;

    public static class SendMailHelper
    {
        public static Encoding MailEncoding = Encoding.UTF8;
        public static MailAddress From = new MailAddress("admin@clubbybook.com", "ClubbyBook");

        public static bool Send(SendItem sendItem)
        {
            return Send(new SendItem[] { sendItem });
        }

        public static bool Send(SendItem[] sendItems)
        {
            bool bResult = true;

            using (var smtpServer = new SmtpClient())
            {
                MailMessage mail = null;

                foreach (var sendItem in sendItems)
                {
                    mail = null;
                    try
                    {
                        mail = new MailMessage();
                        mail.From = From;
#if DEBUG
                        mail.To.Add(new MailAddress("pingvinius@gmail.com", "Instead of " + sendItem.To));
#else
                        mail.To.Add(new MailAddress(sendItem.To));
#endif
                        mail.Subject = sendItem.Subject;
                        mail.SubjectEncoding = MailEncoding;
                        mail.Body = sendItem.Body;
                        mail.BodyEncoding = MailEncoding;
                        mail.IsBodyHtml = false;
                        mail.Priority = MailPriority.Normal;

                        smtpServer.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        bResult = false;
                        Logger.Write("The sending mail operation failed.", ex);
                    }
                    finally
                    {
                        if (mail != null)
                        {
                            mail.Dispose();
                        }
                    }
                }
            }

            return bResult;
        }
    }
}