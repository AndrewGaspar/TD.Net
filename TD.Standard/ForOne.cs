using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD
{
    public static class ForOne
    {
        public static U Transduce<T, U>(T input, ITransducer<T, U> transducer) where U : class
        {
            var reducer = transducer.Apply(Reducer.Make<U, U>((terminator, value) =>
            {
                return Terminator.Termination(value);
            }));

            return reducer.Invoke(null, input).Value;
        }

        public static U? ValueTransduce<T, U>(T input, ITransducer<T, U> transducer) where U : struct
        {
            var reducer = transducer.Apply(Reducer.Make<U?, U>((terminator, value) =>
            {
                return Terminator.Termination((U?)value);
            }));

            return reducer.Invoke(null, input).Value;
        }
    }
}
