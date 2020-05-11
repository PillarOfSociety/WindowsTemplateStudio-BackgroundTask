using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;  //for dispatcher stuff
using Windows.UI.Popups;
using System.Threading.Tasks;

using backtestthree.Services;
using backtestthree.Core.Helpers;

namespace backtestthree.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {

        private string testText = "start";
        public string TestText { get { return testText; } set { Set(ref testText, value); } }

        private double prog = 40;
        public double Prog { get { return prog; } set { Set(ref prog, value); } }

        private BackgroundTaskService backgroundcrap => Singleton<BackgroundTaskService>.Instance;

        public MainPage()
        {
            InitializeComponent();
            //SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += this.OnCloseRequest;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void testBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TestText = "";
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                TestText += task.Value.Name + " " + BackgroundTasks.testback.Message;
            }
            await Task.CompletedTask;
        }

        private async void addEventsToBackgroundTask()
        {
            await Task.CompletedTask;

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name.Equals("testback"))
                {
                    task.Value.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
                    task.Value.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                    break;
                }
            }
        }

        private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Prog = args.Progress;
            });
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var msgbox = new MessageDialog("Completed?");
                var ignore = msgbox.ShowAsync();
            });

        }

        private void startBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            backgroundcrap.TriggerBackTask("testback");
        }

        private async void registerBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //backtestthree.BackgroundTasks.testback mytask = new BackgroundTasks.testback();
            //backgroundcrap.addTask(mytask);
            await backgroundcrap.RegisterBackgroundTasksAsync();
            await Task.CompletedTask;
        }

        private void listBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TestText = "";
            var mylist = backgroundcrap.getTaskList();
            foreach(string name in mylist)
            {
                TestText += name;
            }
        }

        private void unRegisterBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            backgroundcrap.UnregisterAll();
        }

        private void addeventsBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            addEventsToBackgroundTask();
        }

        private void stopBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            backgroundcrap.StopBackTask("testback");
        }

        //private void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        //{
        //https://stackoverflow.com/questions/35056464/uwp-on-desktop-closed-by-top-x-button-no-event
        //      backgroundcrap.CancelAll();
        //    backgroundcrap.UnregisterAll();
        //}

    }
}
