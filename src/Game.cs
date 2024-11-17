using System.IO.Compression;

public class Game
{
    Maze maze;
    Person player = new Player(0, 0);
    Enemy enemy = new Enemy(0, 2);

    public Game(int width, int height)
    {
        maze = new Maze(width, height);
    }

    private bool winner()
    {
        return (player.getX() == maze.getHeight() - 1) && (player.getY() == maze.getWidth() - 1);
    }
    public void Run()
    {
        while (!winner())
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
                default:
                    break;
            }
            MovePlayer(player);
        }
        PrintGrid();
    }
    public void MovePlayer(Person p)
    {
        Node playerLocation = maze.getNode(p.getX(), p.getY());
        // Check if the current cell has a wall in the side facing the orientation of the person
        if (p.getOrientation() == Orientation.NORTH && !playerLocation.GetTopWall())
        {
            p.setX(p.getX() - 1);
        }
        else if (p.getOrientation() == Orientation.EAST && !playerLocation.GetRightWall())
        {
            p.setY(p.getY() + 1);
        }
        else if (p.getOrientation() == Orientation.SOUTH && !playerLocation.GetBottomWall())
        {
            p.setX(p.getX() + 1);
        }
        else if (p.getOrientation() == Orientation.WEST && !playerLocation.GetLeftWall())
        {
            p.setY(p.getY() - 1);
        }
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
                else if (enemy.getX() == x && enemy.getY() == y)
                    Console.Write(maze.getNode(x, y).GetLeftWall() ? "| {0} " : "  {0} ", enemy);
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


}