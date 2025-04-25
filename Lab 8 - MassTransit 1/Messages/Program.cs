namespace Messages;

public interface IMessage1
{
    public string? messageIM1 { get; set; }
}

public interface IMessage2
{
    public string? messageIM2 { get; set; }
}

public interface IMessage3 : IMessage1, IMessage2 { }

public class Program
{
    public static void Main(string[] args)
    {

    }
}
