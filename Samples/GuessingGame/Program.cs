using System;
using TD;
using static TD.Core;
using static TD.Standard;

namespace GuessingGame
{
    static class Program
    {
        static void PrintUsage()
        {
            Console.Error.WriteLine($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} min_value max_value");
        }

        static bool TryGetGame(string[] args, out GuessingGame game)
        {
            game = null;

            if (args.Length < 2)
            {
                Console.Error.WriteLine($"ERROR: Insufficient arguments.");
                return false;
            }

            int min, max;
            if (!int.TryParse(args[0], out min))
            {
                Console.Error.WriteLine($"ERROR: {args[0]} is not an integer.");
                return false;
            }

            if (!int.TryParse(args[1], out max))
            {
                Console.Error.WriteLine($"ERROR: {args[0]} is not an integer.");
                return false;
            }

            game = new GuessingGame(min, max);
            return true;
        }

        static readonly ITransducer<GuessingGameResult, string> ResultFormatting =
            Mapping<GuessingGameResult, string>(result =>
            {
                switch (result.Result)
                {
                    case GuessingGameResultEnum.OutOfRange:
                        return $"Out of range";
                    default:
                        return result.Result.ToString();
                }
            });

        abstract class Result
        {
            public abstract string Display();
            public virtual bool IsEnd => false;
        }

        class GuessResult : Result
        {
            private GuessingGameResult Result;

            public GuessResult(GuessingGameResult gameResult)
            {
                Result = gameResult;
            }

            public override string Display() => $"Result: {Result.Result.ToString()}";

            public override bool IsEnd => Result.Result == GuessingGameResultEnum.Correct;
        }

        class ErrorResult : Result
        {
            private string Error;

            public ErrorResult(string error)
            {
                Error = error;
            }

            public override string Display() => $"Error: {Error}";
        }

        static void Main(string[] args)
        {
            GuessingGame game;

            if (!TryGetGame(args, out game))
            {
                PrintUsage();
                return;
            }
            
            var gamePlaying = Compose(
                TryParsing<int>(),
                Switching(
                    TransducerSwitch.Create(
                        nullable => nullable.HasValue,
                        Compose(
                            Dereferencing<int>(),
                            game.Play(),
                            Mapping<GuessingGameResult, Result>(gameResult => new GuessResult(gameResult)))),
                    TransducerSwitch.Default(Mapping<int?, Result>(_ => new ErrorResult("Not a number!")))
                ),
                Terminating<Result>(result => result.IsEnd),
                Switching(
                    TransducerSwitch.Create(
                        result => result.IsEnd,
                        Mapping<Result, string>(result => $"Congratulations! You did it!\nPress any key to continue...")),
                    TransducerSwitch.Default(
                        Mapping<Result, string>(result => result.Display())
                            .Compose(Formatting<string>("{0}\nGuess: ")))
                ));

            new[] {
                "Welcome to the Guessing Game!\n",
                $"We've picked a value between {game.Min} and {game.Max}. Your job is to guess it.\n",
                "We'll let you know if you're getting warmer or colder. Let's start!\n",
                "Guess: "
            }.Reduce(Console.Out, Relaxing<string, object>().Apply(TextIO.WriteReducer()));

            Console.In.Transduce(Console.Out, gamePlaying);

            Console.ReadKey();
        }
    }
}
