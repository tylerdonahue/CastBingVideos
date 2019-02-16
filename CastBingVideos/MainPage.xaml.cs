using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace CastBingVideos
{
    public sealed partial class MainPage : Page
    {
        private Window ProjectionWindow;
        private int MainViewId;

        public MainPage()
        {
            this.InitializeComponent();

            // save the id of this view for later
            MainViewId = ApplicationView.GetForCurrentView().Id;

            // add script notify event so that we can test to see if the webview can give us some information (like a Uri)
            BingVideoWebView.ScriptNotify += new NotifyEventHandler(MainWebview_ScriptNotify);
        }

        private async void Click_Button_Invoke(object sender, RoutedEventArgs e)
        {
            // warn if no external display connected
            if (!ProjectionManager.ProjectionDisplayAvailable)
            {
                var warnMessage = new MessageDialog("No external display connected. Use alt-tab or taskbar to switch between projected view and main view.");
                await warnMessage.ShowAsync();
            }

            // call ProjectionManager to start projecting a view
            await StartProjecting();

            // fire a window.external.notify event
            await BingVideoWebView.InvokeScriptAsync("eval", new string[] { "window.external.notify('https://www.bing.com/videos/search?q=microsoft&&view=detail&mid=1BD4F6F3274521F068161BD4F6F3274521F06816&&FORM=VRDGAR')" });
        }

        async void MainWebview_ScriptNotify(object sender, NotifyEventArgs e)
        {
            // cast the provided Uri
            await CastVideo(new Uri(e.Value));
        }

        private async Task StartProjecting()
        {
            // do stuff to create a new view
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int ProjectionViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(CastedVideoPage), new Uri("about:blank"), new SuppressNavigationTransitionInfo());
                Window.Current.Content = frame;
                Window.Current.Activate();
                ProjectionWindow = Window.Current;
                // save the id of this view for later
                ProjectionViewId = ApplicationView.GetForCurrentView().Id;
            });

            // use ProjectionManager to get that view on the external display (or full screen it if not connected to external display)
            await ProjectionManager.StartProjectingAsync(ProjectionViewId, MainViewId);
        }

        private async Task CastVideo(Uri uri)
        {
            if (ProjectionWindow != null)
            {
                // set the projected view's content to the Uri
                await ProjectionWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    (ProjectionWindow.Content as Frame).Navigate(typeof(CastedVideoPage), uri, new SuppressNavigationTransitionInfo());
                });
            }
        }
    }
}
