using System;

namespace TD
{
    public static class Transducer
    {
        public static ITransducer<From, To> Mapping<From, To>(Func<From, To> map) =>
            new MappingTransducer<From, To>(map);

        public static ITransducer<Input, Input> Filtering<Input>(Predicate<Input> test) =>
            new FilteringTransducer<Input>(test);

        public static ITransducer<Input, Input> Passing<Input>() => new PassingTransducer<Input>();

        public static ITransducer<A, C> Compose<A, B, C>(
            this ITransducer<A, B> first,
            ITransducer<B, C> second) =>
                new ComposingTransducer<A, B, C>(first, second);

        public static ITransducer<A, D> Compose<A, B, C, D>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third) => Compose(first, second).Compose(third);

        public static ITransducer<A, E> Compose<A, B, C, D, E>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third,
            ITransducer<D, E> fourth) => Compose(first, second, third).Compose(fourth);

        public static ITransducer<A, F> Compose<A, B, C, D, E, F>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third,
            ITransducer<D, E> fourth,
            ITransducer<E, F> fifth) => Compose(first, second, third, fourth).Compose(fifth);

        public static ITransducer<A, G> Compose<A, B, C, D, E, F, G>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third,
            ITransducer<D, E> fourth,
            ITransducer<E, F> fifth,
            ITransducer<F, G> sixth) => Compose(first, second, third, fourth, fifth).Compose(sixth);
    }
}
