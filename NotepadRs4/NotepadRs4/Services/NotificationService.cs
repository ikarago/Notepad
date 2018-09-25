using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Notifications;

using Microsoft.Toolkit.Uwp.Notifications;

namespace NotepadRs4.Services
{
    // #LEGACY CODE: It's still here so you might learn something from it, but it's not used in the app anymore
    //      Reason to remove this code is that it has been superseded by the in-app Notifications which keep the experience inside the app and doesn't destract from it with outside influences.

    /*public static class NotificationService
    {
        // Show Notifications (TEMP)        
        // Save Successful
        public static void ShowNotificationSaveSuccessful()
        {
            // #TODO: Get strings from Resource
            string title = "Save successful!";
            string content = ("File saved to your device");

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Phone stuff
                ShowStatusBarNotification(title);
            }
            else
            {
                // Toast Stuff 
                var toast = CreateToastNotification(title, content);

                // Disable Notification Mirroring so it won't show up on other devices 
                //    (which would be kinda weird, as the notification is only intended for this device) 
                toast.NotificationMirroring = NotificationMirroring.Disabled;

                // Show the Notification
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }

        }

        // Save failed
        public static void ShowNotificationSaveFailed()
        {
            // #TODO: Get strings from Resource
            string title = "Save failed";
            string content = ("Please try again");

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // Phone stuff
                ShowStatusBarNotification(title);
            }
            else
            {
                // Toast Stuff 
                var toast = CreateToastNotification(title, content);

                // Disable Notification Mirroring so it won't show up on other devices 
                //    (which would be kinda weird, as the notification is only intended for this device) 
                toast.NotificationMirroring = NotificationMirroring.Disabled;

                // Show the Notification
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }


        // Methods
        /// <summary>
        /// Create a basic Toast Notification
        /// </summary>
        /// <param name="title">Title of the toast</param>
        /// <param name="content">Subtitle of the toast</param>
        /// <returns>Returns ToastNotification-variable to be used by the ToastNotificationManager</returns>
        private static ToastNotification CreateToastNotification(string title, string content = "")
        {

            // First; create the toast itself
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveText()
                            {
                                Text = content
                            }
                        }
                    }
                }
            };
            // Now convert it to XML
            ToastNotification toast = new ToastNotification(toastContent.GetXml());

            return toast;
        }

        /// <summary>
        /// Show Notification in the StatusBar of a phone
        /// </summary>
        /// <param name="title">Message that needs to be displayed</param>
        private static async void ShowStatusBarNotification(string title)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusbar.ProgressIndicator.Text = title;

                // Show the Statusbar message
                await statusbar.ProgressIndicator.ShowAsync();

                // Hide the message after 3 seconds
                await Task.Delay(TimeSpan.FromSeconds(3));
                await statusbar.ProgressIndicator.HideAsync();
            }
        }
    }*/
}
