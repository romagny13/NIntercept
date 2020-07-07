using System;

namespace NetConsoleSample
{
    public class TestWatcher
    {
        DateTime dateTime;

        public double Seconds { get; private set; }
        public double Milliseconds { get; private set; }

        public void Start()
        {
            dateTime = DateTime.Now;
        }

        public void Stop()
        {
            this.Seconds = Math.Round(DateTime.Now.Subtract(dateTime).TotalSeconds, MidpointRounding.AwayFromZero);
            this.Milliseconds = Math.Round(DateTime.Now.Subtract(dateTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
        }
    }
}
