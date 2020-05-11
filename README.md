# WindowsTemplateStudio-BackgroundTask
My hack of the Windows Template Studio BackgroundService that allows the use of an application trigger.

Also See my Post on Stack OverFlow
https://stackoverflow.com/questions/51914667/uwp-start-background-task-at-startup

## Used the most plain setup I could in the Template Studio:

![WTS Setup](https://github.com/PillarOfSociety/WindowsTemplateStudio-BackgroundTask/blob/master/WTS_Image.PNG)


## Running App:
 
![WTS Setup](https://github.com/PillarOfSociety/WindowsTemplateStudio-BackgroundTask/blob/master/App_Run_Image.PNG)

## Buttons Do the following

- List Registered:  Shows the background tasks associated with this App.  Will also show status if they are started.
- List Service Instances: Shows the background tasks in the list in "BackgroundTaskService"
- Register back: Reruns the register function in "BackgroundTaskService"
- Add events:  adds event handlers to task to update UI.
- Unregister all:  will unregister all background tasks associated with app
- Start Back:  Will trigger Application trigger (makes the task start doing something)
- Stop back: Cancels background task.
