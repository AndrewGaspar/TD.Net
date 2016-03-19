using System;
using TD;

namespace GuessingGame
{
    public enum GuessingGameResultEnum
    {
        OutOfRange,
        Warm,
        Warmer,
        Cold,
        Colder,
        Correct,
        Failure
    };

    public struct GuessingGameResult
    {
        public int Guess;
        public GuessingGameResultEnum Result;
    }

    internal class GuessingGameInstance : ITransducer<int, GuessingGameResult>
    {
        private class Reducer<Reduction> : IReducer<Reduction, int>
        {
            private GuessingGameResult? lastGuess = null;
            
            int? LastDistance => lastGuess.HasValue ? (int?)Distance(lastGuess.Value.Guess) : null;
            GuessingGameResultEnum LastResult => lastGuess.HasValue ? lastGuess.Value.Result : GuessingGameResultEnum.Cold;
            
            int Distance(int value) => Math.Abs(instance.answer - value);
            int Range => instance.max - instance.min;
            
            GuessingGameResultEnum ColdOrWarm(int value) => 
                Distance(value) < Range / 4 ?
                GuessingGameResultEnum.Warm : 
                GuessingGameResultEnum.Cold;

            private GuessingGameInstance instance;
            private IReducer<Reduction, GuessingGameResult> next;

            public Reducer(
                GuessingGameInstance instance,
                IReducer<Reduction, GuessingGameResult> next)
            {
                this.instance = instance;
                this.next = next;
            }

            public Terminator<Reduction> Complete(Reduction reduction) => next.Complete(reduction);

            private GuessingGameResultEnum GetResult(int value)
            {
                if (value > instance.max || value < instance.min)
                {
                    return GuessingGameResultEnum.OutOfRange;
                }

                if (value == instance.answer)
                {
                    return GuessingGameResultEnum.Correct;
                }

                var coldOrWarm = ColdOrWarm(value);
                
                if (LastResult == GuessingGameResultEnum.Cold || coldOrWarm == GuessingGameResultEnum.Cold)
                {
                    return coldOrWarm;
                }
                
                if (Distance(value) < LastDistance.Value)
                {
                    return GuessingGameResultEnum.Warmer;
                }
                else
                {
                    return GuessingGameResultEnum.Colder;
                }
            }

            public Terminator<Reduction> Invoke(Reduction reduction, int value)
            {
                var result = new GuessingGameResult();
                result.Guess = value;
                result.Result = GetResult(value);
                
                lastGuess = result;

                return next.Invoke(reduction, result);
            }
        }

        private int min, max, answer;

        public GuessingGameInstance(int min, int max)
        {
            var rand = new Random();

            this.min = min;
            this.max = max;
            this.answer = rand.Next(this.min, this.max);
        }

        public IReducer<Reduction, int> Apply<Reduction>(IReducer<Reduction, GuessingGameResult> next)
        {
            return new Reducer<Reduction>(this, next);
        }
    }

    public class GuessingGame
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public GuessingGame(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public ITransducer<int, GuessingGameResult> Play()
        {
            return new GuessingGameInstance(Min, Max);
        }
    }
}
