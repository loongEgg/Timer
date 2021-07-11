using System;

namespace LoongEgg.Timer
{
    /// <summary>
    /// event args of <see cref="MicroTimer"/>
    /// </summary>
    public class MicroTimerEventArgs : EventArgs
    {
        /// <summary>
        /// event raised counter, set by creator only
        /// </summary>
        public int TimerCount { get; private set; }

        /// <summary>
        /// Time when timed event was walled since timer started
        /// </summary>
        public long ElapsedMicroSeconds { get; private set; }

        /// <summary>
        /// How late the timer was compared to when it should have been called
        /// </summary>
        public long TimerLateBy { get; private set; }

        /// <summary>
        /// 实际间隔时间
        /// </summary>
        public long ActualInterval { get; private set; }

        /// <summary>
        /// Time it took to execute previous call to callback function
        /// </summary>
        public long CallbackFunctionExecutionTime { get; private set; }

        /// <summary>
        /// create a new instance of <see cref="MicroTimerEventArgs"/>
        /// </summary>
        /// <param name="timeCount">触发计数</param>
        /// <param name="elapsedMicroSeconds">实际间隔时间[us]</param>
        /// <param name="timerLateBy">延迟时间[us]</param>
        /// <param name="callbackFunctionExecutionTime">上一次回调函数的执行时间[us]</param>
        public MicroTimerEventArgs(int timeCount,
                                   long elapsedMicroSeconds,
                                   long timerLateBy,
                                   long actualInterval,
                                   long callbackFunctionExecutionTime)
        {
            this.TimerCount = timeCount;
            this.ElapsedMicroSeconds = elapsedMicroSeconds;
            this.TimerLateBy = timerLateBy;
            this.ActualInterval = actualInterval;
            this.CallbackFunctionExecutionTime = callbackFunctionExecutionTime;
        }
    }
}