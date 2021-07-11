using System;
using System.Threading;

namespace LoongEgg.Timer
{
    /// <summary>
    /// timer resolution is us
    /// </summary>
    public class MicroTimer
    {
        public event MicroTimerElapsedEventHandler Elapsed;

        private Thread _TimerThread = null;
        private bool _StopTimer = true;

        /// <summary>
        /// interval between two timer ticks, in [us]
        /// </summary>
        public long IntervalInMicroSeconds
        {
            get
            {
                return Interlocked.Read(ref _IntervalInMicroSeconds);
            }
            set
            {
                Interlocked.Exchange(ref _IntervalInMicroSeconds, value);
            }
        }
        private long _IntervalInMicroSeconds;

        /// <summary>
        /// most late time to ignore event raising.
        /// [default]=long.MaxValue(not ignore any event)
        /// </summary>
        public long IgnoreEventIfLateBy
        {
            get
            {
                return Interlocked.Read(ref _IgnoreEventIfLateBy);
            }
            set
            {
                Interlocked.Exchange(ref _IgnoreEventIfLateBy, value <= 0 ? long.MaxValue : value);
            }
        }
        private long _IgnoreEventIfLateBy = long.MaxValue;

        /// <summary>
        /// start or stop the timer
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _TimerThread != null && _TimerThread.IsAlive;
            }
            set
            {
                if (value == true)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// create a new instance of <see cref="MicroTimer"/>
        /// </summary>
        /// <param name="intervalInMicroSeconds">interval in [us]</param>
        public MicroTimer(long intervalInMicroSeconds)
        {
            this.IntervalInMicroSeconds = intervalInMicroSeconds;
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        private void Stop() => _StopTimer = true;

        /// <summary>
        /// 开始计时
        /// </summary>
        private void Start()
        {
            if (Enabled || IntervalInMicroSeconds <= 0)
            {
                return;
            }

            _StopTimer = false;
            ThreadStart threadStart = () =>
            {
                BeginRunningTimer(ref _IntervalInMicroSeconds,
                                  ref _IgnoreEventIfLateBy,
                                  ref _StopTimer);
            };

            _TimerThread = new Thread(threadStart) { Priority = ThreadPriority.Highest };
            _TimerThread.Start();
        }

        private void Abort()
        {
            _StopTimer = true;
            if (Enabled == true)
            {
                _TimerThread.Abort();
            }
        }

        private void BeginRunningTimer(ref long intervalInMicroSeconds,
                                       ref long ignoreEventIfLateBy,
                                       ref bool stopTimer)
        {
            int timerCount = 0;
            long nextNotificationTime = 0;
            long callbackFunctionExecutionTime;
            long elapsedMicroSeconds = 0;
            long timerLateBy = 0;
            MicroTimerEventArgs args;
            MicroStopwatch stopwatch = new MicroStopwatch();

            // warm up
            double lastSeconds = DateTime.Now.Second + 1;
            do
            {
                Thread.SpinWait(10);
            } while (lastSeconds > DateTime.Now.Second);

            stopwatch.Start();

            while (stopTimer == false)
            {
                callbackFunctionExecutionTime = stopwatch.ElapsedMicroseconds - nextNotificationTime;
                nextNotificationTime += intervalInMicroSeconds;
                timerCount++;

                elapsedMicroSeconds = 0;
                while ((elapsedMicroSeconds = stopwatch.ElapsedMicroseconds) < nextNotificationTime)
                {
                    Thread.SpinWait(10);
                }

                timerLateBy = elapsedMicroSeconds - nextNotificationTime;
                if (timerLateBy > ignoreEventIfLateBy)
                {
                    continue;
                }

                args = new MicroTimerEventArgs(timerCount,
                                               elapsedMicroSeconds,
                                               timerLateBy,
                                               timerLateBy + intervalInMicroSeconds,
                                               callbackFunctionExecutionTime);

                Elapsed?.Invoke(this, args);
            }// end of  while (stopTimer == false)
            stopwatch.Stop();
        } 
         
    }
}
