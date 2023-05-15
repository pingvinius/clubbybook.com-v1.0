namespace ClubbyBook.Common.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClubbyBook.Common.Utilities;

    public static class Logger
    {
        private static Dictionary<LogType, LogItem> logItems = new Dictionary<LogType, LogItem>();

        public static void AttachLog(LogType logType, string logFilePath, bool isTraceAllowed)
        {
            if (Logger.logItems.ContainsKey(logType))
            {
                Logger.logItems[logType].Dispose();
                Logger.logItems.Remove(logType);
            }

            Logger.logItems.Add(logType, new LogItem(logFilePath, isTraceAllowed));
        }

        public static void DetachAllLogs()
        {
            foreach (var pair in Logger.logItems)
            {
                pair.Value.Dispose();
            }

            Logger.logItems.Clear();
        }

        public static void Write(Exception ex)
        {
            WriteInternal(LogType.General, LogCategoryType.Exception, ex.ToString());
        }

        public static void Write(string message, Exception ex)
        {
            WriteInternal(LogType.General, LogCategoryType.Message, message);
            WriteInternal(LogType.General, LogCategoryType.Exception, ex.ToString());
        }

        public static void Write(string message)
        {
            WriteInternal(LogType.General, LogCategoryType.Message, message);
        }

        public static void WriteSql(string sql)
        {
            WriteInternal(LogType.Sql, LogCategoryType.None, sql);
        }

        private static void WriteInternal(LogType logType, LogCategoryType category, string text)
        {
            if (Logger.logItems.ContainsKey(logType))
            {
                string timeMark = string.Format("{0} {1}", DateTimeHelper.Now.ToShortDateString(), DateTimeHelper.Now.ToLongTimeString());

                StringBuilder sb = new StringBuilder();
                if (category == LogCategoryType.None)
                {
                    sb.AppendFormat(string.Format("{0}:", timeMark));
                }
                else
                {
                    sb.AppendFormat(string.Format("{0}: {1}", category.ToString(), timeMark));
                }

                sb.AppendLine();
                sb.AppendLine(text);

                logItems[logType].WriteLog(sb.ToString());
            }
        }
    }
}