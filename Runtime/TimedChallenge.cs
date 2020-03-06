using System;
using System.Globalization;

namespace Framework.Utility
{
    public class TimedChallenge : ISaveState
    {
        public string SaveIdentifier { get; private set; }

        public DateTime LastChallenge;

        public TimeSpan Cooldown = new TimeSpan(24, 0, 0);

        public TimeSpan RemainingTime => Cooldown - (DateTime.Now - LastChallenge);

        public TimedChallenge(string uniqueId)
        {
            SaveIdentifier = uniqueId;
        }

        public void CompleteChallenge()
        {
            LastChallenge = DateTime.Now;
        }

        public string GetState()
        {
            return LastChallenge.ToString(CultureInfo.InvariantCulture);
        }

        public void LoadState(string state)
        {
            LastChallenge = DateTime.Parse(state, CultureInfo.InvariantCulture);
        }

        public void LoadDefaultState()
        {

        }
    }
}