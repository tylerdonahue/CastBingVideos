using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CastBingVideos
{
    public sealed partial class CastedVideoPage : Page
    {
        public CastedVideoPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            string fullscreenVideo = ((Uri)e.Parameter).AbsoluteUri;
            CastedVideoWebView.Source = new Uri(fullscreenVideo);
        }
    }
}
