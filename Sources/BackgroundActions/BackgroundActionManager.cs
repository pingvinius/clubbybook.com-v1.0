namespace ClubbyBook.BackgroundActions
{
    using System.Collections.Generic;

    public sealed class BackgroundActionManager
    {
        #region Singleton implementation

        private static BackgroundActionManager instance = null;

        private static object syncObject = new object();

        public static BackgroundActionManager Instance
        {
            get
            {
                if (BackgroundActionManager.instance == null)
                {
                    lock (BackgroundActionManager.syncObject)
                    {
                        if (BackgroundActionManager.instance == null)
                        {
                            BackgroundActionManager.instance = new BackgroundActionManager();
                        }
                    }
                }
                return BackgroundActionManager.instance;
            }
        }

        #endregion Singleton implementation

        private List<BackgroundAction> actions;

        private BackgroundActionManager()
        {
            this.actions = new List<BackgroundAction>();
        }

        public void ExecuteAction(BackgroundAction action)
        {
            this.actions.Add(action);
            ExecuteActionInternal();
        }

        private void ExecuteActionInternal()
        {
            // TODO:
            BackgroundAction[] executableAction = new BackgroundAction[0];

            lock (this.actions)
            {
                executableAction = new BackgroundAction[this.actions.Count];
                this.actions.CopyTo(executableAction);
                this.actions.Clear();
            }

            foreach (var action in executableAction)
            {
                action.Execute();
            }

            // TODO:
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //});
        }
    }
}