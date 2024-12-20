class Graphics
{
    private Graphics() { }
    static public void Print(Maze maze, Enemy[]? enemies, Player? player, string text)
    {
        Console.Clear();
        Console.WriteLine(text);

        int width = maze.Width;
        int height = maze.Height;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Console.Write(maze.GetNode(x, y).TOP ? "+---" : "+   ");
            }
            Console.WriteLine("+");

            for (int y = 0; y < width; y++)
            {
                bool hasLeftWall = maze.GetNode(x, y).LEFT;
                string nodeContent = "   ";
                bool nodeContentIsEnemy = false;

                if (player != null && player.X == x && player.Y == y)
                {
                    nodeContent = $" {player} ";
                }
                else if (enemies != null)
                {
                    int enemyIndex = IsEnemyInNode(enemies, x, y);
                    if (enemyIndex > -1)
                    {
                        Enemy enemy = enemies[enemyIndex];
                        nodeContent = enemy.Alive ? $" {enemy} " : " . ";
                        nodeContentIsEnemy = true;
                    }
                }

                Console.Write(hasLeftWall ? "|" : " ");
                // Print enemies in red, the rest in white
                Console.Write(nodeContentIsEnemy ? $"\u001b[31m{nodeContent}\u001b[0m" : nodeContent);

            }
            Console.WriteLine("|");
        }

        // Affichage de la dernière ligne horizontale
        for (int y = 0; y < width; y++)
        {
            Console.Write(maze.GetNode(height - 1, y).BOTTOM ? "+---" : "+   ");
        }
        Console.WriteLine("+");
    }

    // Returns number of index of the found enemy in the coordinates (x, y)
    static private int IsEnemyInNode(Enemy[] enemies, int x, int y)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].X == x && enemies[i].Y == y)
            {
                return i;
            }
        }
        return -1; // Nothing was found
    }

    public static void PrintSavedGamesTable(FileInfo[] files)
    {
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine($"| {"Index",-5} | Filename");
        Console.WriteLine("------------------------------------------------");
        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"| {i,-5} | {files[i].Name,-25} ");
        }
        Console.WriteLine("------------------------------------------------");
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        Console.ForegroundColor = ConsoleColor.White;
    }
}