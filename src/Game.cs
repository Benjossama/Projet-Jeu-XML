public class Game
{
    Maze maze;
    Player player = new Player(0, 0);
    Enemy[] enemies = new Enemy[2];

    public Game(int width, int height)
    {
        maze = new Maze(height, width);
        Random randomNumberGenerator = new Random();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new Enemy(randomNumberGenerator.Next(height / 3, height), randomNumberGenerator.Next(width / 3, width));
        }
    }

    public async void Run()
    {
        bool GameOver = false;

        // The code below in a function that runs asynchronously, it modifies the GameOver varibale
        // in other words it checks if the game is over or not, it checks every 10ms, it is important
        // so that we wouldn't have lags in the game
        _ = Task.Run(async () =>
       {
           while (!GameOver)
           {
               GameOver = Winner() || PlayerDetected();
               if (AllEnemiesKilled())
                   maze.getNode(maze.getHeight() - 1, maze.getWidth() - 1).setWall(Wall.BOTTOM, false);
               PrintGrid();
               await Task.Delay(10);
           }

           PrintGrid();

       });

        // This function is responsible for moving the enemies, regardless of the position
        // of the player or what he is doing, the enemies will move one spot every 500ms
        _ = Task.Run(async () =>
        {
            while (!GameOver)
            {

                foreach (var enemy in enemies)
                {
                    if (!enemy.IsAlive())
                        continue;
                    enemy.Move(maze.getNode(enemy.getX(), enemy.getY()));
                }
                await Task.Delay(500);
            }
        });

        // Boucle principale
        while (!GameOver)
        {
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

            if (!char.IsLower(input) && input != 'P')
            {
                player.Move(maze.getNode(player.getX(), player.getY()));
            }
        }

        if (Winner())
        {
            Console.WriteLine("You won!");
        }
        else
        {
            Console.WriteLine("You died!");
        }
    }

    private void Shoot()
    {
        foreach (var enemy in enemies)
        {
            Node PlayerNode = maze.getNode(player.getX(), player.getY());
            Node EnemyNode = maze.getNode(enemy.getX(), enemy.getY());
            if (maze.NodeAVisibleFromB(PlayerNode, EnemyNode) && ACorrectlyOrientedTowardsB(player, enemy))
            {
                enemy.Kill();
            }
        }

    }

    private bool Winner()
    {
        bool isInLastNode = (player.getX() == maze.getHeight() - 1) && (player.getY() == maze.getWidth() - 1);
        return isInLastNode && AllEnemiesKilled();
    }

    private bool AllEnemiesKilled()
    {
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
        return !foundAliveEnemy;
    }
    private bool PlayerDetected()
    {
        bool detected = false;
        foreach (var enemy in enemies)
        {
            Node playerNode = maze.getNode(player.getX(), player.getY());
            Node enemyNode = maze.getNode(enemy.getX(), enemy.getY());

            // Vérifie si le joueur est dans la ligne de vue de l'ennemi
            if (
                    ACorrectlyOrientedTowardsB(enemy, player) && // Enemy is facing the direction
                                                                 // on which the player is found
                    maze.NodeAVisibleFromB(playerNode, enemyNode) && // No wall between enemy and player
                    enemy.IsAlive()
                )
            {
                detected = true;
            }
        }

        return detected; // Aucun ennemi n'a détecté le joueur
    }

    private bool ACorrectlyOrientedTowardsB(Person A, Person B)
    {
        return A.getOrientation() switch
        {
            Orientation.NORTH => A.getY() == B.getY() && B.getX() <= A.getX(),
            Orientation.SOUTH => A.getY() == B.getY() && B.getX() >= A.getX(),
            Orientation.EAST => A.getX() == B.getX() && B.getY() >= A.getY(),
            Orientation.WEST => A.getX() == B.getX() && B.getY() <= A.getY(),
            _ => false
        };
    }


    public void PrintGrid()
    {
        Console.Clear();

        // Affichage de la ligne supérieure des indices Y
        Console.Write("    "); // Espacement pour aligner avec les indices X
        for (int y = 0; y < maze.getWidth(); y++)
        {
            Console.Write($" {y,3}"); // Espacement formaté
        }
        Console.WriteLine();

        for (int x = 0; x < maze.getHeight(); x++)
        {
            // Ligne horizontale supérieure avec les murs
            Console.Write("    "); // Espacement pour aligner avec les indices X
            for (int y = 0; y < maze.getWidth(); y++)
            {
                Console.Write(maze.getNode(x, y).GetTopWall() ? "+---" : "+   ");
            }
            Console.WriteLine("+");

            // Ligne de contenu (joueurs, ennemis, espaces) avec l'indice vertical
            for (int y = 0; y < maze.getWidth(); y++)
            {
                if (y == 0) // Affichage de l'indice vertical sur la première colonne
                {
                    Console.Write($" {x,2} "); // Affichage de l'indice vertical
                }

                if (player.getX() == x && player.getY() == y)
                    Console.Write(maze.getNode(x, y).GetLeftWall() ? "| {0} " : "  {0} ", player);
                else if (EnnemyHere(x, y) > -1)
                {
                    string EnemyForm = enemies[EnnemyHere(x, y)].IsAlive() ? enemies[EnnemyHere(x, y)].ToString() : ".";
                    if (maze.getNode(x, y).GetLeftWall())
                    {
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($" {EnemyForm,1} ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"  {EnemyForm,1} ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                    Console.Write(maze.getNode(x, y).GetLeftWall() ? "|   " : "    ");
            }
            Console.WriteLine("|");
        }

        // Dernière ligne horizontale
        Console.Write("    "); // Espacement pour aligner avec les indices X
        for (int y = 0; y < maze.getWidth(); y++)
        {
            Console.Write(maze.getNode(maze.getHeight() - 1, y).GetBottomWall() ? "+---" : "+   ");
        }
        Console.WriteLine("+");
    }

    private int EnnemyHere(int x, int y)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].getX() == x && enemies[i].getY() == y)
            {
                return i; // Retourne l'indice de l'ennemi trouvé
            }
        }
        return -1; // Aucun ennemi trouvé
    }


}