using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Utility
{
    /// <summary>
    /// Wrote by Paul Diac
    /// </summary>
    public sealed class TimeSlicer
    {
        private readonly int _milliseconds;
        private readonly Stopwatch _watch = Stopwatch.StartNew();
        private readonly object _lock = new object();
        private long _topStack;

        public long ElapsedMilliseconds => _watch.ElapsedMilliseconds;

        public TimeSlicer(int milliseconds)
        {
            _milliseconds = milliseconds;
        }

        public void Reset()
        {
            lock (_lock)
            {
                _topStack = 0;
            }
        }

        public async Task WaitDrifting(CancellationToken cancellationToken)
        {
            long delay;

            lock (_lock)
            {
                var elapsedMilliseconds = _watch.ElapsedMilliseconds;
                _topStack += _milliseconds;
                if (elapsedMilliseconds > _topStack)
                    _topStack = elapsedMilliseconds;
                delay = _topStack - elapsedMilliseconds;
            }

            if (delay > 0)
            {
                await Task.Delay((int)delay, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        public async Task WaitPerfect(CancellationToken cancellationToken)
        {
            long ending;

            lock (_lock)
            {
                _topStack += _milliseconds;

                var elapsedMilliseconds = _watch.ElapsedMilliseconds;
                if (elapsedMilliseconds > _topStack)
                    _topStack = elapsedMilliseconds;
                ending = _topStack;
            }

            while (_watch.ElapsedMilliseconds < ending)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}