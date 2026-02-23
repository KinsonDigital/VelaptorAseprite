// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests.Helpers;

using Velaptor;

/// <summary>
/// Provides extension methods for various types.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Sets the milliseconds portion of the <see cref="FrameTime"/> to the specified value.
    /// </summary>
    /// <param name="value">The original <see cref="FrameTime"/> value.</param>
    /// <param name="ms">The milliseconds to set.</param>
    /// <returns>A new <see cref="FrameTime"/> with the updated milliseconds.</returns>
    public static FrameTime SetMs(this FrameTime value, int ms)
    {
        return new FrameTime
        {
            ElapsedTime = new TimeSpan(
                value.TotalTime.Days,
                value.TotalTime.Hours,
                value.TotalTime.Minutes,
                value.TotalTime.Seconds,
                ms),
        };
    }

    /// <summary>
    /// Generates a random number between the <paramref name="min"/> and <paramref name="max"/> values.
    /// </summary>
    /// <param name="value">The random number generator.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The random result.</returns>
    public static float NextRange(this Random value, float min, float max) => (value.NextSingle() * (max - min)) + min;

    /// <summary>
    /// Runs the given <paramref name="action"/> until the <paramref name="predicate"/> returns true.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="predicate">The predicate to return true to stop executing the <paramref name="action"/>.</param>
    /// <param name="totalAttempts">The total number of attempts to execute the action until an exception is thrown.</param>
    /// <exception cref="Exception">Thrown if the total number of attempts has been exceeded.</exception>
    public static void RunUntil(this Action action, Func<bool> predicate, int totalAttempts = 1_000)
    {
        var currentAttempt = 0;

        while (!predicate())
        {
            action();
            currentAttempt++;

            if (currentAttempt >= totalAttempts)
            {
                throw new Exception($"The action did not complete within '{totalAttempts}' executions.");
            }
        }
    }
}
