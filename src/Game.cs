using System.Diagnostics;
using System.Net;
using Microsoft.VisualBasic;

public class Game
{
    volatile Maze maze;
    volatile Player player = new Player(0, 0);
    volatile Enemy[] enemies;
    private volatile bool GameOver = false;



    public Game(int width, int height, int NbEnemies)
    {
        maze = new Maze(height, width);
        enemies = new Enemy[NbEnemies];

        Random randomNumberGenerator = new Random();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new Enemy(randomNumberGenerator.Next(height / 3, height), randomNumberGenerator.Next(width / 3, width));
            // enemies[i] = new Enemy((height - 1), (width - 1));
        }

        // maze.getNode(0, 0).setWall(Wall.RIGHT, false);
        // for (int i = 1; i < width - 1; i++)
        // {
        //     maze.getNode(0, i).setWall(Wall.RIGHT, false);
        //     maze.getNode(0, i).setWall(Wall.LEFT, false);
        //     maze.getNode(i, 0).setWall(Wall.TOP, false);
        //     maze.getNode(i, 0).setWall(Wall.BOTTOM, false);
        // }

        Update();
        Show();
        Run();
    }

    public async void Update()
    {
        while (!GameOver)
        {
            if (Winner() || PlayerDetected())
            {
                GameOver = true;
            }
            else
            {
                MoveEnemies();
                await Task.Delay(250);
            }
        }
    }

    private async void Show()
    {
        while (!GameOver)
        {
            PrintGrid();
            await Task.Delay(50);
        }
    }

    public void Run()
    {
        // Boucle principale
        while (!GameOver)
        {
            HandlePlayerInput();
        }
        // When game is over, it has to be handled
        HandleEndOfGame();
    }


    private void MoveEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive())
                continue;
            enemy.Move(maze.getNode(enemy.getX(), enemy.getY()));
        }
    }

    // Existence questionable, has to be improved, may even be removed later
    private void HandleEndOfGame()
    {
        PrintGrid();
        if (Winner())
        {
            Console.WriteLine("You won!");
        }
        else
        {
            Console.WriteLine("You died!");
        }
    }

    private void HandlePlayerInput()
    {
        char input = Console.ReadKey().KeyChar;
        bool CanMove = true;
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
                CanMove = false;
                break;
        }

        if (!char.IsLower(input) && CanMove)
        {
            player.Move(maze.getNode(player.getX(), player.getY()));
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
                // System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"c:\mywavfile.wav");
                // player.Play();
            }
        }

    }

    private bool Winner()
    {
        bool isInLastNode = (player.getX() == maze.getHeight() - 1) && (player.getY() == maze.getWidth() - 1) && player.GetOrientation() == Orientation.SOUTH;
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
        return A.GetOrientation() switch
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
                int enemyhere = FoundEnnemy(x, y);
                string EnemyForm = "";
                if (enemyhere > -1)
                    EnemyForm = enemies[FoundEnnemy(x, y)].IsAlive() ? enemies[FoundEnnemy(x, y)].ToString() : ".";

                if (y == 0) // Affichage de l'indice vertical sur la première colonne
                {
                    Console.Write($" {x,2} "); // Affichage de l'indice vertical
                }
                if (player.getX() == x && player.getY() == y)
                    Console.Write(maze.getNode(x, y).GetLeftWall() ? "| {0} " : "  {0} ", player);
                else if (enemyhere > -1)
                {
                    // Console.Write("\n the value is {0} {1}\n", FoundEnnemy(x, y), enemies.Length);
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

    private int FoundEnnemy(int x, int y)
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
