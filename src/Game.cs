using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;

public class Game
{
    Maze maze;
    Player player = new Player(1, 2);
    Enemy[] enemies = new Enemy[3];

    public Game(int width, int height)
    {
        maze = new Maze(height, width);
        Random randomNumberGenerator = new Random();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new Enemy(randomNumberGenerator.Next(height / 3, height), randomNumberGenerator.Next(width / 3, width));
        }
    }

    private bool Winner()
    {
        // A player wins if he is in the most south-east node, and all enemiers are dead
        bool foundAliveEnemy = false;
        int i = 0;
        while (i < enemies.Length && !foundAliveEnemy)
        {
            if (enemies[i].IsAlive())
            {
                foundAliveEnemy = true;
            }
            i++;
        }
        bool isInLastNode = (player.getX() == maze.getHeight() - 1) && (player.getY() == maze.getWidth() - 1);

        return !foundAliveEnemy && isInLastNode;
    }

    public async void Run()
    {
        bool GameOver = false;

        _ = Task.Run(async () =>
       {
           while (!GameOver)
           {

               GameOver = Winner() || PlayerDetected();
               await Task.Delay(50);
           }
       });
        // Tâche pour déplacer les ennemis périodiquement
        _ = Task.Run(async () =>
        {
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            while (!GameOver)
            {

                foreach (var enemy in enemies)
                {
                    enemy.Move(maze.getNode(enemy.getX(), enemy.getY()));
                }
                PrintGrid();
                milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                await Task.Delay(500);
            }
        });

        // Boucle principale
        while (!GameOver)
        {
            PrintGrid();
            char input = Console.ReadKey().KeyChar;
            switch (input)
            {
                case 'w':
                case 'W':
                    player.setOrientation(Orientation.NORTH);
                    break;
                case 'd':
                case 'D':
                    player.setOrientation(Orientation.EAST);
                    break;
                case 's':
                case 'S':
                    player.setOrientation(Orientation.SOUTH);
                    break;
                case 'a':
                case 'A':
                    player.setOrientation(Orientation.WEST);
                    break;
                case 'P':
                case 'p':
                    Shoot();
                    break;
                default:
                    break;
            }

            if (!char.IsLower(input))
            {
                player.Move(maze.getNode(player.getX(), player.getY()));
            }
        }
        PrintGrid();

    }

    private void Shoot()
    {
        foreach (var enemy in enemies)
        {
            if (player.getX() == enemy.getX() && !isHorizontalWallBetweenNodes(player.getX(), player.getY(), enemy.getY()))
            {
                enemy.Kill();

            }
            else if (player.getY() == enemy.getY() && !isVerticalWallBetweenNodes(player.getY(), player.getX(), enemy.getX()))
            {
                enemy.Kill();
            }

        }

    }

    private bool PlayerDetected()
    {
        return false;
    }

    private bool isHorizontalWallBetweenNodes(int x, int y1, int y2)
    {
        bool foundWall = false;
        int start = Math.Min(y1, y2);
        int end = Math.Max(y1, y2);
        // Does the first node have a right wall, or the second node have a left wall, if yes, return true
        if (maze.getNode(x, start).GetRightWall() || maze.getNode(x, end).GetLeftWall())
            return true;

        // We search for the nodes in between that have right or left wall
        int i = start + 1;
        while (!(i == end) && !foundWall)
        {

            foundWall = maze.getNode(x, i).GetRightWall() || maze.getNode(x, i).GetLeftWall();
            i++;
        }

        return foundWall;
    }

    private bool isVerticalWallBetweenNodes(int y, int x1, int x2)
    {
        bool foundWall = false;
        int start = Math.Min(x1, x2);
        int end = Math.Max(x1, x2);

        // Does the first node have a right wall, or the second node have a left wall, if yes, return true
        if (maze.getNode(start, y).GetBottomWall() || maze.getNode(end, y).GetTopWall())
            return true;

        // We search for the nodes in between that have right or left wall
        int i = start + 1;
        while (!(i == end) && !foundWall)
        {
            foundWall = maze.getNode(i, y).GetBottomWall() || maze.getNode(i, y).GetTopWall();
            i++;
        }

        return foundWall;
    }
    public void PrintGrid()
    {
        Console.Clear();
        for (int x = 0; x < maze.getHeight(); x++)
        {
            for (int y = 0; y < maze.getWidth(); y++)
            {
                Console.Write(maze.getNode(x, y).GetTopWall() ? "+---" : "+   ");
            }
            Console.WriteLine("+");

            for (int y = 0; y < maze.getWidth(); y++)
            {
                if (player.getX() == x && player.getY() == y)
                    Console.Write(maze.getNode(x, y).GetLeftWall() ? "| {0} " : "  {0} ", player);
                else if (ennemyHere(x, y) > -1)
                {
                    if (maze.getNode(x, y).GetLeftWall())
                    {
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" {0} ", enemies[ennemyHere(x, y)]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("  {0} ", enemies[ennemyHere(x, y)]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                    Console.Write(maze.getNode(x, y).GetLeftWall() ? "|   " : "    ");
            }
            Console.WriteLine("|");
        }

        for (int y = 0; y < maze.getWidth(); y++)
        {
            Console.Write(maze.getNode(maze.getHeight() - 1, y).GetBottomWall() ? "+---" : "+   ");
        }
        Console.WriteLine("+");

    }

    private int ennemyHere(int x, int y)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!enemies[i].IsAlive())
                continue;
            if (enemies[i].getX() == x && enemies[i].getY() == y)
            {
                return i; // Retourne l'indice de l'ennemi trouvé
            }
        }
        return -1; // Aucun ennemi trouvé
    }


}