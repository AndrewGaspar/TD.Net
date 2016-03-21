namespace TD
{
    /// <summary>
    /// Contains helper factory functions for generating reduction Terminator wrappers.
    /// </summary>
    public static class Terminator
    {
        /// <summary>
        /// With just one parameter, just wraps the reduction in a non-terminated Terminator.
        /// 
        /// Whether the Terminator should be marked as terminated can be supplied as an additional parameter.
        /// </summary>
        /// <seealso cref="Terminator.Termination{Red}(Red)">
        /// Prefer Termination if terminated is known to be true.
        /// </seealso>
        /// <typeparam name="Red">The wrapped reduction type.</typeparam>
        /// <param name="value">The wrapped reduction value.</param>
        /// <param name="terminated">Whether this represents termination.</param>
        /// <returns>A wrapped reduction value that may indicate termination.</returns>
        public static Terminator<Red> Reduction<Red>(Red value, bool terminated = false) =>
            new Terminator<Red>(value, terminated: terminated);

        /// <summary>
        /// Wraps a reduction in a terminated Terminator value.
        /// </summary>
        /// <typeparam name="Red">The type of the wrapped reduction.</typeparam>
        /// <param name="value">The value of the wrapped reduction.</param>
        /// <returns>A terminated reduction.</returns>
        public static Terminator<Red> Termination<Red>(Red value) =>
            new Terminator<Red>(value, terminated: true);
    }

    /// <summary>
    /// Wraps a reduction value in order to indicate termination up the call stack of an IReducer.
    /// </summary>
    /// <typeparam name="Reduction">The type of the wrapped reduction.</typeparam>
    public struct Terminator<Reduction>
    {
        /// <summary>
        /// Indicates whether the IReducer that it was returned from should receive further input.
        /// </summary>
        public readonly bool IsTerminated;

        /// <summary>
        /// The wrapped reduction value.
        /// </summary>
        public readonly Reduction Value;

        internal Terminator(Reduction value, bool terminated)
        {
            Value = value;
            IsTerminated = terminated;
        }
    }
}
