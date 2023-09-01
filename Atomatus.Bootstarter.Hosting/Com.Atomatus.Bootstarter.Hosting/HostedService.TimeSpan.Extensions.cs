using System;

namespace Com.Atomatus.Bootstarter.Hosting
{
    internal static class HostedServiceTimeSpanExtensions
	{
        static readonly long TOO_LARGE_TICKS = TimeSpan.FromMilliseconds(int.MaxValue).Ticks;

        internal static TimeSpan ThrowsIfZeroLessOrTooLarge(this TimeSpan time, string paramName)
		{
            if (time.Ticks <= TimeSpan.Zero.Ticks)
			{
                throw new ArgumentOutOfRangeException(
                    $"The argument {paramName} must be a positive value!");
            }
            else if (time.Ticks >= TOO_LARGE_TICKS)
            {
                throw new ArgumentOutOfRangeException(
                    $"The argument {paramName} is too large!");
            }

            return time;
		}

        internal static TimeSpan ThrowsIfNegativeOrTooLarge(this TimeSpan time, string paramName)
        {
            if (time.Ticks < TimeSpan.Zero.Ticks)
            {
                throw new ArgumentOutOfRangeException(
                    $"The argument {paramName} must be a non negative value!");
            }
            else if (time.Ticks >= TOO_LARGE_TICKS)
            {
                throw new ArgumentOutOfRangeException(
                    $"The argument {paramName} is too large!");
            }

            return time;
        }

    }
}

