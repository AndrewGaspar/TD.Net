﻿namespace TD
{
    /// <summary>
    /// Describes a computation as taking inputs of type To and producing results of type From.
    /// 
    /// A transducer cannot independently perform any computation - you must apply a reducer to it
    /// so that the transducer has a sink for its computations.
    /// </summary>
    /// <typeparam name="TInput">The type of the inputs to this computation.</typeparam>
    /// <typeparam name="TResult">The type of the results produced by this computation.</typeparam>
    public interface ITransducer<TInput, TResult>
    {
        /// <summary>
        /// Produces a reducer by application of another reducer. The produced reducer will take
        /// input of type From and apply results of type To to the applied reducer.
        /// </summary>
        /// <typeparam name="TReduction">
        /// The type of the reduced value that is produced as an aggregation 
        /// of the values applied to the created reducer.
        /// </typeparam>
        /// <param name="next">A computation that takes a value of type To and returns a reduction of type Reduction.</param>
        /// <returns>A new reducer that wraps the supplied reducer and takes as input type From.</returns>
        IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next);
    }
}
