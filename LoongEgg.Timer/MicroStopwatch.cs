using System;
using System.Diagnostics;

namespace LoongEgg.Timer
{
    public class MicroStopwatch : Stopwatch
    {
        public double MicroSecondsPerTick { get; }

        public long ElapsedMicroseconds => (long)(ElapsedTicks * MicroSecondsPerTick);

        public MicroStopwatch()
        {
            if (Stopwatch.IsHighResolution == false)
            {
                throw new Exception("MicroStopwatch init error: high resolution is not available on this system");
            }
            else
            {
                MicroSecondsPerTick = 1000000d / Stopwatch.Frequency;
                Debug.WriteLine($"MicroStopwatch info: micro seconds per ticks={MicroSecondsPerTick}");
            }
        }
    }
}
