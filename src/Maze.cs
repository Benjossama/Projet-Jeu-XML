using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;


[Serializable]
[XmlRoot("Maze", Namespace = "www.silentstrike.com")]
public class Maze : IXmlSerializable
{

    // The maze class starts
    private int _width;
    private int _height;
    public int Width
    {
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Width must be strictly positive.");
            }
            _width = value;
        }
        get => _width;
    }
    public int Height
    {
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Height must be strictly positive.");
            }
            _height = value;
        }
        get => _height;
    }


    private Node[][] Grid;

    private Random randomNumberGenerator = new Random();
    private HashSet<Node> VisitedNodes = new HashSet<Node>();


    // The parameterless private constructor below is used later for serialization and deserialization
    private Maze()
    {
    }
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

        generate();
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

            if (newX >= 0 && newY >= 0 && newX < _height && newY < _width && !VisitedNodes.Contains(Grid[newX][newY]))
            {
                neighbors.Add(Grid[newX][newY]);
            }
        }

        return neighbors;

    }

    void generate()
    {
        Stack<Node> stack = new Stack<Node>();
        VisitedNodes.Add(Grid[0][0]);
        stack.Push(Grid[0][0]);
        while (stack.Count > 0)
        {

            // Engine.Print(this, null, null, "Loading...");
            // Task.Delay(1).Wait();

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

    public bool NoWallBetweenNodes(Node a, Node b)
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


    public System.Xml.Schema.XmlSchema? GetSchema() => null;

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("Width", Width.ToString());
        writer.WriteElementString("Height", Height.ToString());

        writer.WriteStartElement("Grid");
        foreach (var row in Grid)
        {
            writer.WriteStartElement("Row");
            foreach (var node in row)
            {
                // Serialize Node
                writer.WriteStartElement("Node"); // <Node>

                // Write attributes and elements for Node properties
                writer.WriteElementString("X", node.X.ToString());
                writer.WriteElementString("Y", node.Y.ToString());
                writer.WriteElementString("LEFT", node.LEFT.ToString().ToLower());
                writer.WriteElementString("TOP", node.TOP.ToString().ToLower());
                writer.WriteElementString("RIGHT", node.RIGHT.ToString().ToLower());
                writer.WriteElementString("BOTTOM", node.BOTTOM.ToString().ToLower());

                writer.WriteEndElement(); // </Node>
            }
            writer.WriteEndElement(); // End <Row>
        }
        writer.WriteEndElement(); // End <Grid>
    }


    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement(); // Start <Maze>

        Width = int.Parse(reader.ReadElementString("Width"));
        Height = int.Parse(reader.ReadElementString("Height"));

        Grid = new Node[Height][];

        reader.ReadStartElement("Grid");
        for (int x = 0; x < Height; x++)
        {
            reader.ReadStartElement("Row");
            Grid[x] = new Node[Width];
            for (int y = 0; y < Width; y++)
            {
                reader.ReadStartElement("Node"); // <Node>

                Grid[x][y] = new Node
                {
                    X = int.Parse(reader.ReadElementString("X")),
                    Y = int.Parse(reader.ReadElementString("Y")),
                    LEFT = bool.Parse(reader.ReadElementString("LEFT")),
                    TOP = bool.Parse(reader.ReadElementString("TOP")),
                    RIGHT = bool.Parse(reader.ReadElementString("RIGHT")),
                    BOTTOM = bool.Parse(reader.ReadElementString("BOTTOM"))
                };

                reader.ReadEndElement(); // </Node>

            }
            reader.ReadEndElement(); // End <Row>
        }
        reader.ReadEndElement(); // End <Grid>

        reader.ReadEndElement(); // End <Maze>
    }

}