using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.UI.Popups;
using System.Diagnostics;

namespace backtestthree.BackgroundTasks
{
    public sealed class testback : BackgroundTask
    {
        public static string Message { get; set; }

        private volatile bool _cancelRequested = false;
        private IBackgroundTaskInstance _taskInstance;
        private BackgroundTaskDeferral _deferral;

        private ApplicationTrigger trigger = null;  //Seems like I want an application trigger for Posting to AWS and a deviceusetrigger for getting data from radio
        //https://docs.microsoft.com/en-us/previous-versions/windows/apps/dn630194(v=win.10)  //link needs ) comment breaks it somehow

        public override void Register()
        {
            var taskName = GetType().Name;
            var taskRegistration = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == taskName).Value;

            if (taskRegistration == null)  //if task was already registered, this wont let it register again (can have multiple) BUT since IM manually triggering I need a ref to the current instance (see workaround in BackgroundTaskService)
            {
                var builder = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };

                trigger = new ApplicationTrigger();  //AJS

                // TODO WTS: Define the trigger for your background task and set any (optional) conditions
                // More details at https://docs.microsoft.com/windows/uwp/launch-resume/create-and-register-an-inproc-background-task
                builder.SetTrigger(trigger); //new TimeTrigger(1, false));

                builder.Register();
            }
        }

        public override Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                return null;
            }

            _deferral = taskInstance.GetDeferral();

            return Task.Run(() =>
            {
                //// TODO WTS: Insert the code that should be executed in the background task here.
                //// This sample initializes a timer that counts to 100 in steps of 10.  It updates Message each time.

                //// Documentation:
                ////      * General: https://docs.microsoft.com/windows/uwp/launch-resume/support-your-app-with-background-tasks
                ////      * Debug: https://docs.microsoft.com/windows/uwp/launch-resume/debug-a-background-task
                ////      * Monitoring: https://docs.microsoft.com/windows/uwp/launch-resume/monitor-background-task-progress-and-completion

                //// To show the background progress and message on any page in the application,
                //// subscribe to the Progress and Completed events.
                //// You can do this via "BackgroundTaskService.GetBackgroundTasksRegistration"

                _taskInstance = taskInstance;
                ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(SampleTimerCallback), TimeSpan.FromSeconds(1));
            });
        }

        public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
            // TODO WTS: Insert code to handle the cancelation request here.
            // Documentation: https://docs.microsoft.com/windows/uwp/launch-resume/handle-a-cancelled-background-task
        }

        public override async void Trigger()  //AJS
        {
            _cancelRequested = false;
            await trigger.RequestAsync();
       // https://stackoverflow.com/questions/51914667/uwp-start-background-task-at-startup
        }

        public override void Kill()  //AJS
        {
            this._cancelRequested = true;
        }


        private void SampleTimerCallback(ThreadPoolTimer timer)
        {
            if (_cancelRequested == true)
            {
                Message = $" {_taskInstance.Task.Name} Canceled";
                timer.Cancel();
            }
            else if ((_taskInstance.Progress < 100))
            {
                _taskInstance.Progress += 1;
                Message = $"Background Task {_taskInstance.Task.Name} running";
                Debug.WriteLine("Stupid task is running");
            }
            else
            {
                timer.Cancel();
                Message = $"Background Task {_taskInstance.Task.Name} finished";
                _deferral?.Complete();
            }
        }
    }
}
