namespace ClubbyBook.Common.Logging
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using ClubbyBook.Common.Utilities;

    internal class LogItem : IDisposable
    {
        #region Constants

        private const long maxFileSizeBeforeBackup = 1024 * 1024 * 20; // 20 MB
        private const int frequencyOfBackupChecking = 500;

        #endregion Constants

        #region Fields

        private readonly object lockObject = new object();
        private readonly string logFilePath = string.Empty;

        private readonly bool isTraceAllowed = false;
        private int currentLogRecord = 0;

        #endregion Fields

        public LogItem(string logFilePath, bool isTraceAllowed)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                throw new ArgumentNullException("logFilePath");
            }

            this.logFilePath = logFilePath;
            this.isTraceAllowed = isTraceAllowed;
        }

        public void WriteLog(string text)
        {
            lock (this.lockObject)
            {
                if (this.isTraceAllowed)
                {
                    Trace.WriteLine(text);
                    Trace.WriteLine(string.Empty);
                }

                try
                {
                    var directoryPath = Path.GetDirectoryName(this.logFilePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (StreamWriter sw = new StreamWriter(this.logFilePath, true, Encoding.UTF8))
                    {
                        sw.WriteLine(text);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Couldn't write log to file. Details: " + ex.Message);
                }

                if (this.currentLogRecord == LogItem.frequencyOfBackupChecking)
                {
                    ValidateBackup();
                    this.currentLogRecord = 0;
                }

                this.currentLogRecord++;
            }
        }

        private void ValidateBackup()
        {
            bool isNeedToBackup = false;
            try
            {
                isNeedToBackup = (new FileInfo(logFilePath)).Length >= LogItem.maxFileSizeBeforeBackup;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Couldn't access to log file. Details: " + ex.Message);
            }

            if (isNeedToBackup)
            {
                string logFileName = Path.GetFileNameWithoutExtension(this.logFilePath);
                string backupFileName = string.Format("{0}_backup_{1}.log", logFileName, DateTimeHelper.Now.ToString("yyyy-MM-dd HH.mm.ss"));
                string backupFilePath = Path.Combine(Path.GetDirectoryName(this.logFilePath), backupFileName);

                try
                {
                    File.Copy(this.logFilePath, backupFilePath, true);
                    File.Delete(this.logFilePath);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Couldn't backup log file. Details: " + ex.Message);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion IDisposable Members
    }
}