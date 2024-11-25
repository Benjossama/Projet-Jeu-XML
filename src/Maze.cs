using System.Net.Security;

public class Maze
{
    private int width;
    private int height;
    private Node[,] Grid;
    Random randomNumberGenerator = new Random();

    public Maze(int height, int width)
    {
        this.height = height;
        this.width = width;
        Grid = new Node[height, width];
        // Initalizing all the nodes
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Grid[x, y] = new Node();
                Grid[x, y].setX(x);
                Grid[x, y].setY(y);
            }
        }

        generate();
    }

    public int getHeight()
    {
        return height;
    }

    public int getWidth()
    {
        return width;
    }

    public Node getNode(int x, int y)
    {
        return Grid[x, y];
    }

    private List<Node> getNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int x = node.getX();
        int y = node.getY();

        (int dx, int dy)[] directions = { (1, 0), (-1, 0), (0, 1), (0, -1) };

        foreach (var (dx, dy) in directions)
        {
            int newX = x + dx;
            int newY = y + dy;

            if (newX >= 0 && newY >= 0 && newX < height && newY < width && !Grid[newX, newY].isVisited())
            {
                neighbors.Add(Grid[newX, newY]);
            }
        }

        return neighbors;

    }

    void generate()
    {
        Stack<Node> stack = new Stack<Node>();
        Grid[0, 0].setAsVisited();
        stack.Push(Grid[0, 0]);

        while (stack.Count > 0)
        {
            Node current = stack.Pop();
            List<Node> neighbors = getNeighbors(current);
            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Node neighbor = neighbors[randomNumberGenerator.Next(neighbors.Count)];
                removeWall(current, neighbor);
                neighbor.setAsVisited();
                stack.Push(neighbor);
            }

        }
    }

    void removeWall(Node a, Node b)
    {
        int xOffset = a.getX() - b.getX();
        int yOffset = a.getY() - b.getY();
        // Case of a being above b
        if (xOffset == 1)
        {
            a.setWall(Wall.TOP, false);
            b.setWall(Wall.BOTTOM, false);
        }
        // Case of a being under b
        else if (xOffset == -1)
        {
            a.setWall(Wall.BOTTOM, false);
            b.setWall(Wall.TOP, false);
        }
        // Case of a being to the right of b
        else if (yOffset == 1)
        {
            a.setWall(Wall.LEFT, false);
            b.setWall(Wall.RIGHT, false);
        }
        // Case of a being to the left of b
        else if (yOffset == -1)
        {
            a.setWall(Wall.RIGHT, false);
            b.setWall(Wall.LEFT, false);
        }

    }
}