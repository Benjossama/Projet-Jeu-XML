using System.Xml.Serialization;

public enum Wall
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
};

[Serializable]
public class Node
{
    private int x;
    public int X
    {
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("X value must be positive.");
            }
            x = value;
        }
        get => x;
    }
    private int y;
    public int Y
    {
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("Y values must be positive.");
            }
            y = value;
        }
        get => y;
    }

    // Walls represented as XML attributes
    public bool LEFT { set; get; }

    public bool TOP { set; get; }

    public bool RIGHT { set; get; }

    public bool BOTTOM { set; get; }
    //Useful for generating the maze, that's all, need not be serialized

    public Node()
    {
        TOP = BOTTOM = RIGHT = LEFT = true;
    }
};