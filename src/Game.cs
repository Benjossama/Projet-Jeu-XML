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

        Update();
        Show();
        RunPlayer();
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
            // Unlock door
            if (AllEnemiesKilled())
                maze.getNode(maze.getHeight() - 1, maze.getWidth() - 1).setWall(Wall.BOTTOM, false);
        }
    }

    private async void Show()
    {
        while (!GameOver)
        {
            Engine.Print(maze, enemies, player, "Playing...");
            await Task.Delay(50);
        }
    }

    public void RunPlayer()
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
        string text = Winner() ? "You won!" : "You died!"; ;
        Engine.Print(maze, enemies, player, text);

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
}
