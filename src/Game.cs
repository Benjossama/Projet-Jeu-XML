using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Game", Namespace = "http://www.silentstrike.com", IsNullable = false)]
[Serializable]
public class Game
{
    volatile public Maze maze;
    volatile public Player player;
    volatile public Enemy[] enemies;
    volatile public bool GameOver;
    public string? Filename;

    private Game() { }
    public Game(int height, int width, int NbEnemies, string filename)
    {
        this.Filename = filename;
        player = new Player(0, 0);
        maze = new Maze(height, width);
        enemies = new Enemy[NbEnemies];

        Random randomNumberGenerator = new Random();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new Enemy(randomNumberGenerator.Next(height / 3, height), randomNumberGenerator.Next(width / 3, width));

        }
        GameOver = false;

        Update();
        Show();
        RunPlayer();
    }

    public async void Update()
    {
        // if (maze == null) throw new ArgumentNullException("maze
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
                maze.GetNode(maze.Height - 1, maze.Width - 1).BOTTOM = false;
            // Saving after every iteration
            XMLUtils.Save(this, Filename);
        }
    }

    public async void Show()
    {
        while (!GameOver)
        {
            Engine.Print(maze, enemies, player, $"Playing: {Filename}");
            await Task.Delay(10);
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
            if (!enemy.Alive)
                continue;
            enemy.Move(maze.GetNode(enemy.X, enemy.Y));
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
                player.Orientation = Orientation.NORTH;
                break;
            case 'd':
            case 'D':
                player.Orientation = Orientation.EAST;
                break;
            case 's':
            case 'S':
                player.Orientation = Orientation.SOUTH;
                break;
            case 'a':
            case 'A':
                player.Orientation = Orientation.WEST;
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
            player.Move(maze.GetNode(player.X, player.Y));
        }
    }


    private void Shoot()
    {
        foreach (var enemy in enemies)
        {
            Node PlayerNode = maze.GetNode(player.X, player.Y);
            Node EnemyNode = maze.GetNode(enemy.X, enemy.Y);
            if (maze.NoWallBetweenNodes(PlayerNode, EnemyNode) && ACorrectlyOrientedTowardsB(player, enemy))
            {
                enemy.Kill();
            }
        }

    }


    private bool Winner()
    {
        bool isOutOfMaze = (player.X > maze.Height - 1) && (player.Y >= maze.Width - 1);
        return isOutOfMaze && AllEnemiesKilled();
    }

    private bool AllEnemiesKilled()
    {
        bool foundAliveEnemy = false;
        int i = 0;
        while (i < enemies.Length && !foundAliveEnemy)
        {
            if (enemies[i].Alive)
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
            Node playerNode = maze.GetNode(player.X, player.Y);
            Node enemyNode = maze.GetNode(enemy.X, enemy.Y);

            // Vérifie si le joueur est dans la ligne de vue de l'ennemi
            if (
                    ACorrectlyOrientedTowardsB(enemy, player) && // Enemy is facing the direction
                                                                 // on which the player is found
                    maze.NoWallBetweenNodes(playerNode, enemyNode) && // No wall between enemy and player
                    enemy.Alive
                )
            {
                detected = true;
            }
        }

        return detected; // Aucun ennemi n'a détecté le joueur
    }


    private bool ACorrectlyOrientedTowardsB(Person A, Person B)
    {
        return A.Orientation switch
        {
            Orientation.NORTH => A.Y == B.Y && B.X <= A.X,
            Orientation.SOUTH => A.Y == B.Y && B.X >= A.X,
            Orientation.EAST => A.X == B.X && B.Y >= A.Y,
            Orientation.WEST => A.X == B.X && B.Y <= A.Y,
            _ => false
        };
    }
}
