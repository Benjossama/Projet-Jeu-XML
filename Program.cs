using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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
                Console.Write("Enter game height (< 40): ");
                int height = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                Console.Write("Enter game width (< 40): ");
                int width = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                Console.Write("Enter number of enemies (< 40): ");
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
                    Graphics.PrintError("No saved games found.");
                    return;
                }

                Graphics.PrintSavedGamesTable(files);

                Console.Write("Enter the number of the game to load: ");
                int index = int.Parse(Console.ReadLine() ?? throw new Exception("Could not read input."));

                if (index < 0 || index >= files.Length)
                {
                    throw new ArgumentException("Invalid arguments.");
                }
                // Load the game
                XMLUtils.Load(files[index].Name);
            }
            else
            {
                throw new ArgumentException("Invalid arguments.");
            }

            // At the end update the menus
            XMLUtils.UpdateSavedGamesMenu("data/saved_games", "data/menu.xml");
            XMLUtils.RunXSLT("data/xslt/convertMenuToHTML.xslt", "data/menu.xml", "data/menu.html");

        }
        catch (ArgumentException e)
        {
            Graphics.PrintError($"Invalid arguments.\n{e.Message}\nPlease restart the game and try again.");
        }
        catch (Exception e)
        {
            Graphics.PrintError($"Error occured: {e.Message}\nPlease restart the game and try again.");
        }

    }
}
