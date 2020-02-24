using System;

using NotepadRs4.Services;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace NotepadRs4
{
    public sealed partial class App : Application
    {
        // Set a global variable for the App_CloseRequest so it can keep an eye on whether to show the confirm close-dialog.
        public static bool UnsavedChanges { get; set; }
        public static bool IsNotClosedProperly { get; set; } //#TODO: Save to the Settings when done
        public static AutoRecoveryService RecoveryService = new AutoRecoveryService();


        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            // Xbox stuff (Disabled, only here for testing)
            //this.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
            //if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            //{
            //    this.FocusVisualKind = FocusVisualKind.Reveal;
            //}

            UnsavedChanges = false; // Setting it by default to false, because the app will open the files unedited anyway. :)

            EnteredBackground += App_EnteredBackground;


            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage));
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }
    }
}
