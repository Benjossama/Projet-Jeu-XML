using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
public class Maze
{

    // The maze class starts
    private int _width;
    private int _height;
    public int Width
    {
        set
        {
            if (value <= 0 || value >= 40)
            {
                throw new ArgumentOutOfRangeException("Width must be strictly positive and less than 40.");
            }
            _width = value;
        }
        get => _width;
    }
    public int Height
    {
        set
        {
            if (value <= 0 || value >= 40)
            {
                throw new ArgumentOutOfRangeException("Height must be strictly positive and less than 40.");
            }
            _height = value;
        }
        get => _height;
    }

    [XmlArray("Grid")]
    [XmlArrayItem("Row")]
    public Node[][] Grid;
    private Random randomNumberGenerator = new Random();
    private HashSet<Node> VisitedNodes = new HashSet<Node>();

    // Used for serialization and deserialization.
    // It makes sense that a 10x10 maze would be used, neither easy nor difficult
#pragma warning disable
    public Maze()
    {
    }
#pragma warning restore
    public Maze(int _height, int _width)
    {
        Height = _height;
        Width = _width;
        Grid = new Node[Height][];
        // Initalizing all the nodes
        for (int x = 0; x < _height; x++)
        {
            Grid[x] = new Node[_width];
            for (int y = 0; y < _width; y++)
            {
                Grid[x][y] = new Node();
                Grid[x][y].X = x;
                Grid[x][y].Y = y;
            }
        }

        Generate();
    }

    public Node GetNode(int x, int y)
    {
        try
        {
            return Grid[x][y];
        }
        catch (IndexOutOfRangeException)
        {
            return new Node();
        }
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int x = node.X;
        int y = node.Y;

        (int dx, int dy)[] directions = { (1, 0), (-1, 0), (0, 1), (0, -1) };

        foreach (var (dx, dy) in directions)
        {
            int newX = x + dx;
            int newY = y + dy;

            if (newX >= 0 && newY >= 0 && newX < _height && newY < _width && !VisitedNodes.Contains(Grid[newX][newY]))
            {
                neighbors.Add(Grid[newX][newY]);
            }
        }

        return neighbors;

    }

    void Generate()
    {
        Stack<Node> stack = new Stack<Node>();
        VisitedNodes.Add(Grid[0][0]);
        stack.Push(Grid[0][0]);
        while (stack.Count > 0)
        {

            Graphics.Print(this, null, null, "Loading...");
            Task.Delay(2000 / (Height * Width)).Wait();

            Node current = stack.Pop();
            List<Node> neighbors = GetNeighbors(current);
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

    // Given two nodes A and B
    // This retunrs true if on of them is accessible from the other
    // meaning they are on the same vertical or horizontal line and there
    // no wall between them
    public bool NoWallBetweenNodes(Node a, Node b)
    {
        if (a.X == b.X && a.Y == b.Y)
            return true;

        // If they are on the same horizontal line
        else if (a.X == b.X)
            return IsPathClear(a.X, a.Y, b.Y, true);
        // If they are on the same vertical line
        else if (a.Y == b.Y)
            return IsPathClear(a.Y, a.X, b.X, false);

        else
            return false;
    }


    // Checks if there is a clear path between the two nodes
    private bool IsPathClear(int fixedCoordinate, int first, int second, bool isVertical)
    {
        int min = Math.Min(first, second);
        int max = Math.Max(first, second);
        bool ClearPath = true; // By default, assume there is no wall

        // Loop through the nodes between the two coordinates, depending on the direction (vertical or horizontal)
        for (int i = min; i <= max; i++)
        {
            // If the direction is vertical, use the fixed coordinate (y-axis) and loop through positions on the x-axis.
            // If the direction is horizontal, use the fixed coordinate (x-axis) and loop through positions on the y-axis.
            var node = isVertical ? GetNode(fixedCoordinate, i) : GetNode(i, fixedCoordinate);


            // Check for walls on each node:
            // - If we're at the start of the path (min), check if there is a wall to the right (horizontal) or bottom (vertical).
            // - If we're at the end of the path (max), check if there is a wall to the left (horizontal) or top (vertical).
            // - If we're between min and max, check for a left/right wall (horizontal) or top/bottom wall (vertical).
            if ((i == min && (isVertical ? node.RIGHT : node.BOTTOM)) ||
                (i == max && (isVertical ? node.LEFT : node.TOP)) ||
                (i > min && i < max &&
                 (isVertical ? node.LEFT || node.RIGHT
                              : node.TOP || node.BOTTOM)))
            {
                ClearPath = false;
            }
        }

        return ClearPath;
    }
}