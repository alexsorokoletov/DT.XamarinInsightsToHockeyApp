using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Xamarin
{
    public static class Insights
    {
        /// <summary>
        /// Data from Identify call that needs to be send after Initialized call.
        /// Difference between Hockey and Insights
        /// </summary>
        private static Dictionary<string, string> _identificationData = null;

        public const string DebugModeKey = "DebugModeKey";

        public static bool DisableCollection
        {
            get; set;
        }

        [Obsolete("this property is not supported by HockeyApp")]
        public static Insights.CollectionTypes DisableCollectionTypes
        {
            get; set;
        }

        [Obsolete("this property is not supported by HockeyApp")]
        public static bool DisableDataTransmission
        {
            get; set;
        }

        public static bool DisableExceptionCatching
        {
            get; set;
        }

        [Obsolete("this property is not supported by HockeyApp")]
        public static bool ForceDataTransmission
        {
            get; set;
        }

        public static bool IsInitialized
        {
            private set;
            get;
        }

        public static void Identify(string uid, string key, string value)
        {
            var table = new Dictionary<string, string>() { { key, value } };
            Identify(uid, table);
        }

        public static void Identify(string uid, IDictionary<string, string> table = null)
        {
#if !PORTABLE
#if __ANDROID__
#endif
#if __IOS__
            var mgr = HockeyApp.iOS.BITHockeyManager.SharedHockeyManager;
#endif
#endif
            var isGuest = uid == Traits.GuestIdentifier;
            if (table != null)
            {
#if !PORTABLE
#if __IOS__
                if (table.ContainsKey(Traits.Email))
                {
                    mgr.UserEmail = table[Traits.Email];
                }
                if (table.ContainsKey(Traits.Name))
                {
                    mgr.UserName = table[Traits.Name];
                }
#endif
#if __ANDROID__
#endif
#endif
                var properties = new Dictionary<string, string>(table);
                properties.Add("Anonymous/Guest", isGuest.ToString());
                if (IsInitialized)
                {
                    HockeyApp.MetricsManager.TrackEvent("X-Xamarin-Insights-Identify", properties, null);
                }
                else
                {
                    _identificationData = properties;
                }
            }
        }

        /// <summary>
        /// Suggesting to call it during FinishedLaunching
        /// </summary>
        public static void Initialize(string apiKey, bool blockOnStartupCrashes = false)
        {
#if !PORTABLE
#if __IOS__
            var mgr = HockeyApp.iOS.BITHockeyManager.SharedHockeyManager;
            mgr.Configure(apiKey);
            mgr.LogLevel = HockeyApp.iOS.BITLogLevel.Warning;
            ConfigureLogging();
            mgr.DisableMetricsManager = DisableCollection;
            mgr.DisableCrashManager = DisableExceptionCatching;
            mgr.StartManager();
            mgr.Authenticator.AuthenticateInstallation();
            mgr.CrashManager.CrashManagerStatus = HockeyApp.iOS.BITCrashManagerStatus.AutoSend;
            if (mgr.CrashManager.DidCrashInLastSession)
            {
                var wasStartupCrash = mgr.CrashManager.TimeIntervalCrashInLastSessionOccurred < 10;
                HasPendingCrashReport?.Invoke(mgr.CrashManager.LastSessionCrashDetails, wasStartupCrash);
            }
#endif
#if __ANDROID__
            HockeyApp.Android.Utils.HockeyLog.LogLevel = 5;
            HockeyApp.Android.CrashManager.Register(global::Android.App.Application.Context, apiKey);
            //need to get current app to register the metrics stuff
            //HockeyApp.Android.Metrics.MetricsManager.Register(
            //TODO implement Android
#endif
#endif
            if (_identificationData != null)
            {
                var data = _identificationData;
                _identificationData = null;
                HockeyApp.MetricsManager.TrackEvent("X-Xamarin-Insights-Identify", data, null);
            }
            IsInitialized = true;
        }

        [Obsolete("this property is not supported by HockeyApp")]
        public static Task PurgePendingCrashReports()
        {
            return Task.FromResult(false);
        }

        public static void Report(Exception exception = null, Insights.Severity warningLevel = Insights.Severity.Warning)
        {
            Report(exception, extraData: null, warningLevel: warningLevel);
        }

        public static void Report(Exception exception, string key, string value, Insights.Severity warningLevel = Insights.Severity.Warning)
        {
            var extraData = new Dictionary<string, string>() { { key, value } };
            Report(exception, extraData, warningLevel);
        }

        public static void Report(Exception exception, IDictionary extraData, Insights.Severity warningLevel = Insights.Severity.Warning)
        {
            var eventName = $"Error-{warningLevel}";
            var properties = new Dictionary<string, string>();
            if (extraData != null)
            {
                if (extraData is Dictionary<string, string>)
                {
                    properties = (Dictionary<string, string>)extraData;
                }
                else
                {
                    foreach (DictionaryEntry de in extraData)
                    {
                        properties.Add(de.Key?.ToString(), de.Value?.ToString());
                    }
                }
            }
            properties.Add(nameof(Insights.Severity), warningLevel.ToString());
            properties.Add("Exception", exception.ToString());
            HockeyApp.MetricsManager.TrackEvent(eventName, properties, null);
        }

        [Obsolete("this property is not supported by HockeyApp")]
        public static Task Save()
        {
            return Task.FromResult(true);
        }

        public static void Track(string trackIdentifier, IDictionary<string, string> table = null)
        {
            var properties = new Dictionary<string, string>(table);
            HockeyApp.MetricsManager.TrackEvent(trackIdentifier, properties, null);
        }

        public static void Track(string trackIdentifier, string key, string value)
        {
            var properties = new Dictionary<string, string>() { { key, value } };
            HockeyApp.MetricsManager.TrackEvent(trackIdentifier, properties, null);
        }

        public static ITrackHandle TrackTime(string identifier, string key, string value)
        {
            return TrackTime(identifier, new Dictionary<string, string>() { { key, value } });
        }

        public static ITrackHandle TrackTime(string identifier, IDictionary<string, string> table = null)
        {
            return new HockeyTrackable(identifier, table);
        }

        [Conditional("DEBUG")]
        private static void ConfigureLogging()
        {
#if !PORTABLE
#if __IOS__
            HockeyApp.iOS.BITHockeyManager.SharedHockeyManager.LogLevel = HockeyApp.iOS.BITLogLevel.Verbose;
#endif
#if __ANDROID__
            HockeyApp.Android.Utils.HockeyLog.LogLevel = 2;
#endif
#endif
        }

        public static event Insights.HasPendingCrashReportEventHandler HasPendingCrashReport;

        public delegate void HasPendingCrashReportEventHandler(object sender, bool isStartupCrash);

        [Flags]
        public enum CollectionTypes
        {
            None = 0,
            Jailbroken = 1,
            HardwareInfo = 2,
            NetworkInfo = 4,
            Locale = 8,
            OSInfo = 16
        }

        public enum Severity
        {
            Warning,
            Error,
            Critical
        }

        public static class Traits
        {
            public const string Address = "address";
            public const string Website = "website";
            public const string Phone = "phone";
            public const string Name = "name";
            public const string LastName = "lastName";
            public const string Gender = "gender";
            public const string FirstName = "firstName";
            public const string Age = "age";
            public const string Avatar = "avatar";
            public const string DateOfBirth = "dateOfBirth";
            public const string CreatedAt = "createdAt";
            public const string Description = "description";
            public const string Email = "email";
            public const string GuestIdentifier = "X-Xamarin-Identify-Guest";
        }

        internal static readonly CultureInfo EnUs = new CultureInfo("en-US");
    }
}
