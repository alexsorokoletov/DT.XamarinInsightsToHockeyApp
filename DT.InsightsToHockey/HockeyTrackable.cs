using System.Collections.Generic;
using System.Diagnostics;

namespace Xamarin
{
    internal class HockeyTrackable : ITrackHandle
    {
        private readonly string _eventName;
        private Dictionary<string, string> _data;
#if !PORTABLE
        private readonly Stopwatch _sw = new Stopwatch();
#endif

        public HockeyTrackable(string eventName, IDictionary<string, string> data)
        {
#if !PORTABLE
            _eventName = eventName;
            _data = (data != null) ? new Dictionary<string, string>(data) : new Dictionary<string, string>();
#endif
            Start();
        }

        public IDictionary<string, string> Data
        {
            get { return _data; }
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
#if !PORTABLE
            _sw.Start();
#endif
        }

        public void Stop()
        {
#if !PORTABLE
            _sw.Stop();
            var timeElapsedSeconds = _sw.Elapsed.TotalSeconds;
            Data.Add("X-Time", timeElapsedSeconds.ToString(Insights.EnUs));
            var measurements = new Dictionary<string, double>() { { _eventName, timeElapsedSeconds } };
            HockeyApp.MetricsManager.TrackEvent(_eventName, _data, measurements);
#endif
        }
    }
    
}
