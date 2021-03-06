# DT.XamarinInsightsToHockeyApp
[![NuGet Badge](https://buildstats.info/nuget/DT.InsightsToHockey?includePreReleases=true)](https://www.nuget.org/packages/DT.InsightsToHockey/)

Drop-in library to migrate from Xamarin.Insights to HockeyApp with as little changes as possible

Since Xamarin.Insights is coming to end of life, we all need to migrate to HockeyApp.

Most of the API available in Xamarin.Insights is supported by HockeyApp SDK in one form or another.

Not supported APIs are marked as `[Obsolete]`.

## How to use?
1. Remove NuGet Package `Xamarin.Insights` from your project and other references to `Xamarin.Insights` SDK.
2. Add NuGet package: `Install-Package DT.InsightsToHockey`.
3. Change Insights' API key to HockeyApp API key.
4. Some platforms might require additional steps (for iOS you have to add NSPhotoLibraryUsageDescription key, etc).
[Read details here.](https://support.hockeyapp.net/kb/client-integration-cross-platform/how-to-integrate-hockeyapp-with-xamarin)
5. That's it.

### Disclaimer

This is a work in progress. iOS and PCL (bait and switch) is supported, Android is supported partially


### Collaboration

Will be glad to get any help with testing, samples, CI and other platforms support. Ping me on Twitter [@AlexSorokoletov](https://twitter.com/AlexSorokoletov)