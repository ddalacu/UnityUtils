using System;
using UnityEngine;

namespace Framework.Utility
{
    [Serializable]
    public struct Ticker
    {
        private float _tick;

        public float TickInterval;

        public void Reset()
        {
            _tick = 0;
        }

        public uint UpdateTicks(float deltaTime)
        {
            Debug.Assert(TickInterval > 0);

            _tick += deltaTime;

            uint ticksCount = 0;

            while (_tick > TickInterval)
            {
                _tick -= TickInterval;
                ticksCount++;
            }

            return ticksCount;
        }

    }
}
