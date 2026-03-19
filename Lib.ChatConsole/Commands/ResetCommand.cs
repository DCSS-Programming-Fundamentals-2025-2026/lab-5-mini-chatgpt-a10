public class ResetCommand : IReplCommand
{
    public string Name 
    { 
        get { return "/reset"; } 
    }

    public string Description 
    { 
        get { return "Скинути контекст чату (очищує консоль)"; } 
    }

    public void Execute(string[] args, CommandExecutionContext context)
    {
        Console.Clear();
        Console.WriteLine("Контекст скинуто. Можете почати нову розмову.");
    }
}