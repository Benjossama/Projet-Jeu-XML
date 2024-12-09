using System.Dynamic;
using System.Xml.Serialization;

[Serializable]
public class Maze
{
    private int width;
    private int height;
    [XmlElement("Width")]
    public int Width
    {
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Value must be strictly positive.");
            }
            width = value;
        }
        get => width;
    }
    [XmlElement("Height")]
    public int Height
    {
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Value must be strictly positive.");
            }
            height = value;
        }
        get => height;
    }
    [XmlIgnore]
    private Node[,]? Grid;

    [XmlArray("Nodes")]
    [XmlArrayItem("Node")]
    public List<Node> Nodes
    {
        get
        {
            List<Node> list = new List<Node>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.WriteLine("{0} {1}", Height, Width);
                    list.Add(Grid[i, j]);
                }
            }
            return list;
        }
        set
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Grid[i, j] = value[i * height + j];
                }
            }
        }
    }

    [XmlIgnore]
    Random randomNumberGenerator = new Random();
    [XmlIgnore]
    HashSet<Node> VisitedNodes = new HashSet<Node>();


    // The parameterless private constructor below is used later for serialization and deserialization
    private Maze()
    {
        Console.Write("Creating... {0} {1}\n", Width, Height);
        // Grid = new Node[Height, Width];
    }
    public Maze(int height, int width)
    {
        Height = height;
        Width = width;
        Grid = new Node[Height, Width];
        // Initalizing all the nodes
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < Width; y++)
            {
                Grid[x, y] = new Node();
                Grid[x, y].X = x;
                Grid[x, y].Y = y;
            }
        }

        generate();
    }

    public Node GetNode(int x, int y)
    {
        try
        {
            return Grid[x, y];
        }
        catch (IndexOutOfRangeException)
        {
            return new Node();
        }
    }

    private List<Node> getNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int x = node.X;
        int y = node.Y;

        (int dx, int dy)[] directions = { (1, 0), (-1, 0), (0, 1), (0, -1) };

        foreach (var (dx, dy) in directions)
        {
            int newX = x + dx;
            int newY = y + dy;

            if (newX >= 0 && newY >= 0 && newX < height && newY < width && !VisitedNodes.Contains(Grid[newX, newY]))
            {
                neighbors.Add(Grid[newX, newY]);
            }
        }

        return neighbors;

    }

    void generate()
    {
        Stack<Node> stack = new Stack<Node>();
        VisitedNodes.Add(Grid[0, 0]);
        stack.Push(Grid[0, 0]);
        while (stack.Count > 0)
        {

            // Engine.Print(this, null, null, "Loading...");
            // Task.Delay(25).Wait();

            Node current = stack.Pop();
            List<Node> neighbors = getNeighbors(current);
            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Node neighbor = neighbors[randomNumberGenerator.Next(neighbors.Count)];
                removeWall(current, neighbor);
                VisitedNodes.Add(neighbor);
                stack.Push(neighbor);
            }

        }
    }

    void removeWall(Node a, Node b)
    {
        int xOffset = a.X - b.X;
        int yOffset = a.Y - b.Y;
        // Case of a being above b
        if (xOffset == 1)
        {
            a.TOP = false;
            b.BOTTOM = false;
        }
        // Case of a being under b
        else if (xOffset == -1)
        {
            a.BOTTOM = false;
            b.TOP = false;
        }
        // Case of a being to the right of b
        else if (yOffset == 1)
        {
            a.LEFT = false;
            b.RIGHT = false;
        }
        // Case of a being to the left of b
        else if (yOffset == -1)
        {
            a.RIGHT = false;
            b.LEFT = false;
        }
    }

    public bool NodeAVisibleFromB(Node a, Node b)
    {
        if (a.X == b.X && a.Y == b.Y)
            return true;

        else if (a.X == b.X)
            return IsPathClear(a.X, a.Y, b.Y, true);

        else if (a.Y == b.Y)
            return IsPathClear(a.Y, a.X, b.X, false);

        else
            return false;
    }

    private bool IsPathClear(int fixedCoordinate, int first, int second, bool isVertical)
    {
        int min = Math.Min(first, second);
        int max = Math.Max(first, second);
        bool ClearPath = true;
        for (int i = min; i <= max; i++)
        {
            var node = isVertical ? GetNode(fixedCoordinate, i) : GetNode(i, fixedCoordinate);

            if ((i == min && (isVertical ? node.RIGHT : node.BOTTOM)) ||
                (i == max && (isVertical ? node.LEFT : node.TOP)) ||
                (i > min && i < max &&
                 ((isVertical ? node.LEFT || node.RIGHT
                              : node.TOP || node.BOTTOM))))
            {
                ClearPath = false;
            }
        }

        return ClearPath;
    }

}