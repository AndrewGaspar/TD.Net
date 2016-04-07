using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD
{
    public static class Net
    {
        /// <summary>
        /// Converts values from host order to network order.
        /// </summary>
        /// <typeparam name="T">The type of the numeric value.</typeparam>
        /// <returns>A transducer that converts host order to network order.</returns>
        public static ITransducer<T, T> NetworkOrder<T>() where T : struct => new NetworkOrder<T>();

        /// <summary>
        /// Creates a transducer that produces numeric results in network order.
        /// </summary>
        /// <typeparam name="TInput">Input to original transducer</typeparam>
        /// <typeparam name="TResult">The numeric type to be converted from host to network order.</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <returns>The new transducer</returns>
        public static ITransducer<TInput, TResult> NetworkOrder<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer) where TResult : struct => 
                transducer.Compose(NetworkOrder<TResult>());

        /// <summary>
        /// Converts a numeric from network order to host order
        /// </summary>
        /// <typeparam name="TNumeric">The numeric type</typeparam>
        /// <returns>A transducer that converts network order to host order</returns>
        public static ITransducer<TNumeric, TNumeric> HostOrder<TNumeric>() where TNumeric : struct =>
            new HostOrder<TNumeric>();

        /// <summary>
        /// Produces a transducer that outputs host order numerics
        /// </summary>
        /// <typeparam name="TInput">Input type to the original transducer</typeparam>
        /// <typeparam name="TNumeric">The numeric type to be converted</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <returns>The new transducer</returns>
        public static ITransducer<TInput, TNumeric> HostOrder<TInput, TNumeric>(
            this ITransducer<TInput, TNumeric> transducer) where TNumeric : struct =>
                transducer.Compose(HostOrder<TNumeric>());
    }
}
