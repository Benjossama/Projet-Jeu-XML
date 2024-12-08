class Program
{
    // public static Engine engine;
    public static void Main(string[] args)
    {
        Game game = new Game(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
        // Maze maze = new Maze(5, 5);
    }

}
