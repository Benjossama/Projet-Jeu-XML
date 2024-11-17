public enum Wall
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
};

public class Node
{
    // Tracking the Node's position
    private int x;
    private int y;
    private Dictionary<Wall, bool> walls = new Dictionary<Wall, bool>();
    private bool visited = false;

    public Node()
    {
        walls.Add(Wall.TOP, true);
        walls.Add(Wall.RIGHT, true);
        walls.Add(Wall.BOTTOM, true);
        walls.Add(Wall.LEFT, true);
    }

    public void setWall(Wall wall, bool state)
    {
        walls[wall] = state;
    }

    public bool GetTopWall()
    {
        return walls[Wall.TOP];
    }

    public bool GetRightWall()
    {
        return walls[Wall.RIGHT];
    }
    public bool GetBottomWall()
    {
        return walls[Wall.BOTTOM];
    }
    public bool GetLeftWall()
    {
        return walls[Wall.LEFT];
    }
    public bool isVisited()
    {
        return visited;
    }

    public void setAsVisited()
    {
        visited = true;
    }

    public void show()
    {
        // Première ligne : Mur du haut
        if (walls[Wall.TOP])
            Console.WriteLine(" --- ");
        else
            Console.WriteLine("     ");

        // Ligne centrale avec murs gauche et droite
        if (walls[Wall.LEFT])
            Console.Write("|");
        else
            Console.Write(" ");

        Console.Write("   "); // espace au centre pour le nœud lui-même

        if (walls[Wall.RIGHT])
            Console.WriteLine("|");
        else
            Console.WriteLine(" ");

        // Dernière ligne : Mur du bas
        if (walls[Wall.BOTTOM])
            Console.WriteLine(" --- ");
        else
            Console.WriteLine("     ");
    }


    public void setX(int x)
    {
        this.x = x;
    }

    public void setY(int y)
    {
        this.y = y;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }
};