using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using backtestthree.Activation;
using backtestthree.BackgroundTasks;
using backtestthree.Core.Helpers;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;

namespace backtestthree.Services
{
    internal class BackgroundTaskService : ActivationHandler<BackgroundActivatedEventArgs>
    {
        public static IEnumerable<BackgroundTask> BackgroundTasks => BackgroundTaskInstances.Value;

         private static readonly Lazy<IEnumerable<BackgroundTask>> BackgroundTaskInstances =
             new Lazy<IEnumerable<BackgroundTask>>(() => CreateInstances());

        public async Task RegisterBackgroundTasksAsync()
        {
            BackgroundExecutionManager.RemoveAccess();
            var result = await BackgroundExecutionManager.RequestAccessAsync();

            if (result == BackgroundAccessStatus.DeniedBySystemPolicy
                || result == BackgroundAccessStatus.DeniedByUser)
            {
                return;
            }

            //Avery additions //because for some reason if I run this multiple times in a row there is a chance the task it still registered and we crash
            //some weird shit was happening with this
            //BackgroundTask testTask  = BackgroundTasks.FirstOrDefault();

            //if (testTask !=null)    //makesure we got a task from the list
            //    if (BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == testTask.GetType().Name))//see if for some reason its already registered
            //    {
            //        //if already registered, its because we left shit running and registered and we need to register new instances we can interact with
            //        UnregisterAll();
            //    }

            //just dont even fool
            UnregisterAll();  //since I dont know how to get currently registered instances and I need to interact with them to trigger

            foreach (var task in BackgroundTasks)
            {
                    task.Register();        //register my instances that I have a ref to.
            }
        }

        public static BackgroundTaskRegistration GetBackgroundTasksRegistration<T>()
            where T : BackgroundTask
        {
            if (!BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == typeof(T).Name))
            {
                // This condition should not be met. If it is it means the background task was not registered correctly.
                // Please check CreateInstances to see if the background task was properly added to the BackgroundTasks property.
                return null;
            }

            return (BackgroundTaskRegistration)BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == typeof(T).Name).Value;
        }

        public void Start(IBackgroundTaskInstance taskInstance)
        {
            var task = BackgroundTasks.FirstOrDefault(b => b.Match(taskInstance?.Task?.Name)); 

            if (task == null)
            {
                // This condition should not be met. It is it it means the background task to start was not found in the background tasks managed by this service.
                // Please check CreateInstances to see if the background task was properly added to the BackgroundTasks property.
                return;
            }

            task.RunAsync(taskInstance).FireAndForget();
        }

        protected override async Task HandleInternalAsync(BackgroundActivatedEventArgs args)
        {
            Start(args.TaskInstance);

            await Task.CompletedTask;
        }

        private static IEnumerable<BackgroundTask> CreateInstances()
        {
            var backgroundTasks = new List<BackgroundTask>();

            backgroundTasks.Add(new testback());
            return backgroundTasks;
        }


        public void TriggerBackTask(string taskName)
        {
            var task = BackgroundTasks.FirstOrDefault(b => b.Match(taskName));
            task?.Trigger();
        }

        public void StopBackTask(string taskName)
        {
            var task = BackgroundTasks.FirstOrDefault(b => b.Match(taskName));
            task?.Kill();
        }

        public List<string> getTaskList()
        {
            List<string> tlist = new List<string>();
            foreach (BackgroundTask task in BackgroundTasks)
                tlist.Add(task.GetType().Name);

            return tlist;
        }

        public void UnregisterAll()
        {
            var tasks = BackgroundTaskRegistration.AllTasks;
            foreach (var task in tasks)
            {
                // You can check here for the name
                string name = task.Value.Name;
                task.Value.Unregister(true);
            }
        }

        public void CancelAll()
        {
            foreach (BackgroundTask task in BackgroundTasks)
                task.Kill();
        }

    }
}
