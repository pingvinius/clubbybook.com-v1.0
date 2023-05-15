namespace ClubbyBook.Business
{
    using System;
    using ClubbyBook.Common.Logging;
    using EFCachingProvider;
    using EFCachingProvider.Caching;
    using EFProviderWrapperToolkit;
    using EFTracingProvider;

    public partial class DbContext : Entities
    {
        private static ICache contextCacheObject = new InMemoryCache();
#if DEBUG
        private static CachingPolicy contextCachingPolicy = CachingPolicy.NoCaching;
#else
        private static CachingPolicy contextCachingPolicy = CachingPolicy.CacheAll;
#endif

        private bool isDisposed;

        public DbContext(string connectionString)
            : base(EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(connectionString, "EFTracingProvider", "EFCachingProvider"))
        {
            this.CommandExecuting += OnSqlCommandExecuting;
            this.ContextOptions.LazyLoadingEnabled = true;
            this.Cache = DbContext.contextCacheObject;
            this.CachingPolicy = DbContext.contextCachingPolicy;
            this.isDisposed = false;
        }

        #region Logging Routine

        private void OnSqlCommandExecuting(object sender, CommandExecutionEventArgs e)
        {
            Logger.WriteSql(e.ToTraceString().TrimEnd());
        }

        #endregion Logging Routine

        #region Tracing Extensions

        internal event EventHandler<CommandExecutionEventArgs> CommandExecuting
        {
            add
            {
                this.TracingConnection.CommandExecuting += value;
            }
            remove
            {
                this.TracingConnection.CommandExecuting -= value;
            }
        }

        internal event EventHandler<CommandExecutionEventArgs> CommandFinished
        {
            add
            {
                this.TracingConnection.CommandFinished += value;
            }
            remove
            {
                this.TracingConnection.CommandFinished -= value;
            }
        }

        internal event EventHandler<CommandExecutionEventArgs> CommandFailed
        {
            add
            {
                this.TracingConnection.CommandFailed += value;
            }
            remove
            {
                this.TracingConnection.CommandFailed -= value;
            }
        }

        private EFTracingConnection TracingConnection
        {
            get
            {
                return this.UnwrapConnection<EFTracingConnection>();
            }
        }

        #endregion Tracing Extensions

        #region Caching Extensions

        internal ICache Cache
        {
            get
            {
                return CachingConnection.Cache;
            }
            set
            {
                CachingConnection.Cache = value;
            }
        }

        internal CachingPolicy CachingPolicy
        {
            get
            {
                return CachingConnection.CachingPolicy;
            }
            set
            {
                CachingConnection.CachingPolicy = value;
            }
        }

        private EFCachingConnection CachingConnection
        {
            get
            {
                return this.UnwrapConnection<EFCachingConnection>();
            }
        }

        #endregion Caching Extensions

        #region IDisposable implementation

        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                // Dispose managed resources
                if (disposing)
                {
                    this.CommandExecuting -= OnSqlCommandExecuting;
                }

                // Disposing unmanaged resources
            }

            this.isDisposed = true;

            try
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }
            catch
            {
            }

            try
            {
                base.Dispose(disposing);
            }
            catch
            {
            }
        }

        #endregion IDisposable implementation
    }
}