namespace ClubbyBook.Business
{
    using System;
    using System.Collections;
    using System.Data;
    using ClubbyBook.Common;
    using ClubbyBook.Common.Logging;
    using MySql.Data.MySqlClient;

    public static class ContextManager
    {
        private static IContextHolder contextHolder = null;
        private static object syncDbContext = new object();

        public static DbContext Current
        {
            get
            {
                if (ContextManager.contextHolder == null)
                {
                    throw new InvalidOperationException("The context holder should be initialized.");
                }

                if (!ContextManager.contextHolder.IsAvailable())
                {
                    throw new InvalidOperationException("The context holder should be available.");
                }

                if (!ContextManager.contextHolder.Contains())
                {
                    lock (ContextManager.syncDbContext)
                    {
                        if (!ContextManager.contextHolder.Contains())
                        {
                            ContextManager.contextHolder.Set(new DbContext(Settings.ConnectionString));
                        }
                    }
                }

                var dbContext = ContextManager.contextHolder.Get() as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException("The context holder holds wrong type of context.");
                }

                return dbContext;
            }
        }

        public static void Initialize(IContextHolder contextHolder)
        {
            ContextManager.contextHolder = contextHolder;
        }

        public static void Dispose()
        {
            try
            {
                if (ContextManager.contextHolder != null && ContextManager.contextHolder.IsAvailable() && ContextManager.contextHolder.Contains())
                {
                    var dbContext = ContextManager.contextHolder.Get() as DbContext;
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }

                    ContextManager.contextHolder.Remove();
                }
            }
            catch (Exception ex)
            {
                Logger.Write("An error has occurred while disposing context manager.", ex);
            }
        }

        /// <summary>
        /// This function checks for any sleeping connections beyond a reasonable time and kills them.
        /// Since .NET appears to have a bug with how pooling MySQL connections are handled and leaves
        /// too many sleeping connections without closing them, we will kill them here.
        /// </summary>
        /// iMinSecondsToExpire - all connections sleeping more than this amount in seconds will be killed.
        /// <returns>integer - number of connections killed</returns>
        public static int KillSleepingConnections(int iMinSecondsToExpire)
        {
            string strSQL = "show processlist";
            System.Collections.ArrayList m_ProcessesToKill = new ArrayList();

            using (MySqlConnection myConnection = new MySqlConnection(Settings.SimpleConnectionString))
            {
                using (MySqlCommand myCommand = new MySqlCommand(strSQL, myConnection))
                {
                    MySqlDataReader myReader = null;

                    try
                    {
                        myConnection.Open();

                        // Get a list of processes to kill.
                        myReader = myCommand.ExecuteReader();
                        while (myReader.Read())
                        {
                            // Find all processes sleeping with a timeout value higher than our threshold.
                            int iPID = Convert.ToInt32(myReader["Id"].ToString());
                            string strState = myReader["Command"].ToString();
                            int iTime = Convert.ToInt32(myReader["Time"].ToString());

                            if (strState == "Sleep" && iTime >= iMinSecondsToExpire && iPID > 0)
                            {
                                // This connection is sitting around doing nothing. Kill it.
                                m_ProcessesToKill.Add(iPID);
                            }
                        }

                        myReader.Close();

                        foreach (int aPID in m_ProcessesToKill)
                        {
                            strSQL = "kill " + aPID;
                            myCommand.CommandText = strSQL;
                            myCommand.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (myReader != null && !myReader.IsClosed)
                        {
                            myReader.Close();
                        }

                        if (myConnection != null && myConnection.State == ConnectionState.Open)
                        {
                            myConnection.Close();
                        }
                    }
                }
            }

            Logger.Write(string.Format("The {0} sleeped connections killed.", m_ProcessesToKill.Count));

            return m_ProcessesToKill.Count;
        }
    }
}