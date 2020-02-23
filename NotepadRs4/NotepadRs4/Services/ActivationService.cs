using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NotepadRs4.Activation;
using NotepadRs4.Helpers;
using NotepadRs4.ViewModels;
using NotepadRs4.Views.Dialogs;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NotepadRs4.Services
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        private readonly App _app;
        private readonly Lazy<UIElement> _shell;
        private readonly Type _defaultNavItem;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize things like registering background task before the app is loaded
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Window.Current.Content = _shell?.Value ?? new Frame();
                    NavigationService.NavigationFailed += (sender, e) =>
                    {
                        throw e.Exception;
                    };
                    NavigationService.Navigated += Frame_Navigated;
                    if (SystemNavigationManager.GetForCurrentView() != null)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested += ActivationService_BackRequested;
                        
                        // Stuff for popping a confirmation dialog before closing
                        SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
                    }
                }
            }

            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }

                // Set custom stuff
                SetTitlebar();
                SetMinimalWindowSize();
                // Maximize UI on Xbox (Disabled, only here for testing)
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            // CREDIT: Made possible by the really helpful blog post by Martin Zikmund, go check it out! https://blog.mzikmund.com/2018/09/app-close-confirmation-in-uwp/

            // #TODO: Give the user the option to save from this dialog
            if (App.UnsavedChanges == true)
            {
                var deferral = e.GetDeferral();
                var dialog = new ExitConfirmationDialog();

                // #TODO Catch the rare case when another dialog is already open by cancelling that dialog and prioritizing this dialog over it instead
                await dialog.ShowAsync();
                // Check the answer; if no then cancel closing the app
                if (dialog.Result == ExitConfirmationDialogResult.Cancel || dialog.Result == ExitConfirmationDialogResult.DialogClosed)
                {
                    // Cancel the closure by setting the Handled-status to true
                    e.Handled = true;
                }
                deferral.Complete();
            }

        }

        private async Task InitializeAsync()
        {
            await ThemeSelectorService.InitializeAsync();
            await Singleton<SettingsViewModel>.Instance.EnsureInstanceInitializedAsync();
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            ThemeSelectorService.SetRequestedTheme();
            await WhatsNewDisplayService.ShowIfAppropriateAsync();
            await Task.CompletedTask;
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<FileAssociationService>.Instance;
            yield return Singleton<SuspendAndResumeService>.Instance;
            yield break;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationService.CanGoBack ?
            //    AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void ActivationService_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
        }

        private void SetTitlebar()
        {
            // Make the buttons transparent
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                titlebar.ButtonForegroundColor = Colors.White;
            }
            else if (App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                titlebar.ButtonForegroundColor = Colors.Black;
            }

            // Extend the normal window to the Titlebar for the blur to reach there too
            //CoreApplicationViewTitleBar coreTitlebar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitlebar.ExtendViewIntoTitleBar = true;
        }

        private void SetMinimalWindowSize()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(360, 300));
        }
    }
}
