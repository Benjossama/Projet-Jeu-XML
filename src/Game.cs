using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;

[Serializable]
public class Game
{
    volatile private Maze maze;
    [XmlElement("Maze")]
    public Maze Maze
    {
        get => maze;
        set { }

    }

    volatile private Player player;
    [XmlElement("Player")]
    public Player Player
    {
        get => player;
        set { }
    }

    volatile private Enemy[] enemies;
    [XmlArray("Enemies")]
    [XmlArrayItem("Enemy")]
    public Enemy[] Enemies
    {
        get => enemies;
        set { enemies = value; }
    }

    volatile private bool _GameOver;
    [XmlAttribute("GameOver")]
    public bool GameOver
    {
        get => _GameOver;
        set { }
    }

    private Game() { }
    public Game(int width, int height, int NbEnemies)
    {
        player = new Player(0, 0);
        maze = new Maze(height, width);
        enemies = new Enemy[NbEnemies];

        Random randomNumberGenerator = new Random();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new Enemy(randomNumberGenerator.Next(height / 3, height), randomNumberGenerator.Next(width / 3, width));

        }
        _GameOver = false;

        Update();
        Show();
        RunPlayer();
    }

    public async void Update()
    {
        while (!_GameOver)
        {
            if (Winner() || PlayerDetected())
            {
                _GameOver = true;
            }
            else
            {
                MoveEnemies();
                await Task.Delay(250);
            }
            // Unlock door
            if (AllEnemiesKilled())
                maze.GetNode(maze.Height - 1, maze.Width - 1).BOTTOM = false;

            XMLUtils.Save(this);
        }
    }

    public async void Show()
    {
        while (!_GameOver)
        {
            Engine.Print(maze, enemies, player, "Playing...");
            await Task.Delay(50);
        }
    }

    public void RunPlayer()
    {
        // Boucle principale
        while (!_GameOver)
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
            if (maze.NodeAVisibleFromB(PlayerNode, EnemyNode) && ACorrectlyOrientedTowardsB(player, enemy))
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
                    maze.NodeAVisibleFromB(playerNode, enemyNode) && // No wall between enemy and player
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
