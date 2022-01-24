using Microsoft.Xna.Framework;
using System;

namespace Rosie.Misc
{
    class Timer
    {
        private double _TimerVal = 0.0;
        private double _DelayTime = 0.0;
        public bool Enabled { get; set; } = true;

        public Timer(double DelayTime)
        {
            _DelayTime = DelayTime;
        }

        public void Wait(GameTime gt, Action Action)
        {
            if (Enabled && _TimerVal <= gt.TotalGameTime.TotalMilliseconds)
            {
                _TimerVal = gt.TotalGameTime.TotalMilliseconds + _DelayTime;
                Action.Invoke();
            }
        }
    }
}
