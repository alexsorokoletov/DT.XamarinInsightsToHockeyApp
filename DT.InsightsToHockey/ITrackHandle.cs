using System;
using System.Collections.Generic;

namespace Xamarin
{
    public interface ITrackHandle : IDisposable
    {
        IDictionary<string, string> Data
        {
            get;
        }

        void Start();
        void Stop();
    }
    
}
