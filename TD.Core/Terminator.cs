namespace TD
{
    public class Terminator
    {
        public static Terminator<Red> Reduction<Red>(Red value, bool terminated = false) =>
            new Terminator<Red>(value, terminated: terminated);

        public static Terminator<Red> Termination<Red>(Red value) =>
            new Terminator<Red>(value, terminated: true);
    }

    public class Terminator<Reduction>
    {
        public bool Terminated { get; private set; }
        public Reduction Value { get; private set; }

        internal Terminator(Reduction value, bool terminated)
        {
            Value = value;
            Terminated = terminated;
        }
    }
}
