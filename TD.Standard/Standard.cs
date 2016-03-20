using System;
using System.Collections.Generic;
using static TD.Transducer;

namespace TD
{
    public static class Standard
    {
        public static ITransducer<string, T?> TryParsing<T>() where T : struct => new TryParsing<T>();

        public static ITransducer<T?, T> Dereferencing<T>() where T : struct => Mapping<T?, T>(x => x.Value);

        public static ITransducer<T?, T> FilteringNonNull<T>() where T : struct =>
            Filtering<T?>(x => x.HasValue).Compose(Dereferencing<T>());

        public static ITransducer<From, To> Switching<From, To>(params TransducerSwitch<From, To>[] switches) =>
            new Switching<From, To>(switches);

        public static ITransducer<From, To> Switching<From, To>(IList<TransducerSwitch<From, To>> switches) =>
            new Switching<From, To>(switches);

        public static ITransducer<T, string> Formatting<T>(string formatString) =>
            Mapping<T, string>(val => string.Format(formatString, val));

        public static ITransducer<T, T> Terminating<T>(Predicate<T> test) => new Terminating<T>(test);

        public static ITransducer<From, To> Casting<From, To>() where From : To =>
            Mapping<From, To>(a => a);

        public static ITransducer<T, To> Casting<T, From, To>(this ITransducer<T, From> transducer) 
            where From : To => 
            transducer.Compose(Casting<From, To>());

        public static ITransducer<T, object> Relaxing<T>() => Casting<T, object>();

        public static ITransducer<T, object> Relaxing<T, U>(this ITransducer<T, U> transducer) =>
            transducer.Compose(Casting<U, object>());
    }
}
