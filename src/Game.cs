using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public enum GameState
{
    Over,
    Paused,
    Running,
    Quitting
}

[XmlRoot("Game", Namespace = "http://www.silentstrike.com/game", IsNullable = false)]
[Serializable]

public class Game
{

    volatile public Maze maze;
    volatile public Player player;
    volatile public Enemy[] enemies;
    volatile public GameState State;
    public string filename;

    // Used for serialization and deserialization.
#pragma warning disable
    private Game()
    {
    }
#pragma warning restore

    public Game(int height, int width, int NbEnemies, string filename)
    {
        // Check that NbEnemies is valid
        if (NbEnemies >= 40 || NbEnemies < 1)
            throw new ArgumentOutOfRangeException("Number of enemies must be between 1 and 40.");

        // Copying variables & settings up variables
        this.filename = filename;
        player = new Player(0, 0);
        maze = new Maze(height, width);
        enemies = new Enemy[NbEnemies];
        State = GameState.Running;

        // Setting up enemies
        Random Rand = new Random();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = new Enemy(Rand.Next(height / 3, height), Rand.Next(width / 3, width));

        }
        // Create the file and save the initial state of the game
        XMLUtils.Save(this, this.filename);
        // Start running the game
        Update();
        Show();
        Run();
    }

    // Update the game and moves the enemies.
    // Checks that the game is not over.
    public async void Update()
    {
        while (State != GameState.Over && State != GameState.Quitting)
        {
            if (Winner() || PlayerDetected())
            {
                State = GameState.Over;
            }
            else if (State == GameState.Running)
            {
                MoveEnemies();
                // Unlock door
                if (AllEnemiesKilled())
                    maze.GetNode(maze.Height - 1, maze.Width - 1).BOTTOM = false;
            }
            await Task.Delay(250);
        }
    }

    // Calls the printing function while the game is not over 60 times per second (60 fps)
    public async void Show()
    {
        while (State != GameState.Over && State != GameState.Quitting)
        {
            string TextToPrint = State == GameState.Paused
                                        ? $"Game Paused. \nPress escape to continue\nCNTRL+S to save and quit."
                                        : $"Playing: {filename}";
            Graphics.Print(maze, enemies, player, TextToPrint);
            await Task.Delay(1000 / 60);
        }
    }

    // Gets the input from the user whule the game is not over
    public void Run()
    {
        while (State != GameState.Over && State != GameState.Quitting)
        {
            HandlePlayerInput();
        }

        // When game is over, it has to be handled
        if (State == GameState.Over)
        {
            XMLUtils.Save(this, this.filename);
            Graphics.Print(maze, enemies, player, Winner() ? "You won." : "You lost.");
        }
    }

    // Loops through the enemies and call move on each one of them
    private void MoveEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.Alive)
                continue;
            enemy.Move(maze.GetNode(enemy.X, enemy.Y));
        }
    }

    public void SaveAndQuit()
    {
        XMLUtils.Save(this, filename); // Save the game
        State = GameState.Quitting;
    }

    private void HandlePlayerInput()
    {
        // Read the input from the player
        ConsoleKeyInfo input = Console.ReadKey();
        // If the game is paused, we have these two scenarios
        // Escape is pressed, then we unpause the game
        // S is pressed, we save the game and quit
        if (State == GameState.Paused)
        {
            if (input.Key == ConsoleKey.Escape)
                State = GameState.Running;
            else if (input.Key == ConsoleKey.S && input.Modifiers.HasFlag(ConsoleModifiers.Control))
                SaveAndQuit();
            return;
        }

        switch (input.Key)
        {
            case ConsoleKey.W:
                player.Orientation = Orientation.NORTH;
                break;
            case ConsoleKey.D:
                player.Orientation = Orientation.EAST;
                break;
            case ConsoleKey.S:
                player.Orientation = Orientation.SOUTH;
                break;
            case ConsoleKey.A:
                player.Orientation = Orientation.WEST;
                break;
            case ConsoleKey.P:
                if (State == GameState.Running)
                    Shoot();
                break;
            case ConsoleKey.Escape:
                State = (State == GameState.Running) ? GameState.Paused : GameState.Running;
                break;
            default:
                return;
        }

        //  Move if the typed character is upper case
        //  otherwise just change orientation
        if (char.IsUpper(input.KeyChar))
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

    // Returns true if all enemies are dead
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

    // Returns true when the player is detected
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
                    enemy.Alive // The enemy has to be alive for us to be detected
                )
            {
                detected = true;
            }
        }

        return detected;
    }

    // Si la personne A (player or enemy) regarde dans la direction ou se trouve la personne B

    //                                                                          A      B
    // Dans ce cas :  il est orienté dans la bonne direction pour voir l'autre: >      v


    //                                                                                A      B
    // Dans ce cas :  il n'est pas orienté dans la bonne direction pour voir l'autre: v      >
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