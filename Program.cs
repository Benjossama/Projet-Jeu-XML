using System;
using System.IO;
using System.Linq;
class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1 - New Game");
            Console.WriteLine("2 - Load Game");
            Console.Write("Enter your choice (1 or 2): ");

            string choice = Console.ReadLine() ?? throw new Exception("Could not read input.");

            if (choice == "1")
            {
                Console.Write("Enter game height: ");
                int height = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                Console.Write("Enter game width: ");
                int width = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                Console.Write("Enter number of enemies: ");
                int enemies = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                Console.Write("Enter a name for the game file: ");
                string filename = Console.ReadLine() + ".xml";

                // Creating a new game with the provided parameters
                Game game = new Game(height, width, enemies, filename);
            }
            else if (choice == "2")
            {
                FileInfo[] files = new DirectoryInfo(@"./data/saved_games/").GetFiles()
                    .OrderBy(f => f.LastWriteTime)
                    .ToArray();

                // We do not throw an exception here because no games being saved is not an error.
                if (files.Length == 0)
                {
                    Console.WriteLine("No saved games found.");
                    return;
                }

                Engine.PrintSavedGamesTable(files);

                Console.Write("Enter the number of the game to load: ");
                int index = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                if (index < 0 || index >= files.Length)
                {
                    throw new ArgumentException("Invalid arguments.");
                }

                XMLUtils.Load(files[index].Name);
            }
            else
            {
                throw new ArgumentException("Invalid arguments.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            Console.WriteLine("Please restart the program and try again.");
        }
    }
}
