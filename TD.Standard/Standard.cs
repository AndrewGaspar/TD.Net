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
    }
}
