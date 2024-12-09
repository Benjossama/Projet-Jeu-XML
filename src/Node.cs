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
    // Tracking the Node's position   // Tracking the Node's position
    private int x;
    [XmlElement("X")]
    public int X
    {
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Value must be positive.");
            }
            x = value;
        }
        get => x;
    }
    private int y;
    [XmlElement("Y")]
    public int Y
    {
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Value must be positive.");
            }
            y = value;
        }
        get => y;
    }

    // Walls represented as XML attributes
    [XmlElement("LeftWall")]
    public bool LEFT { set; get; }

    [XmlElement("TopWall")]
    public bool TOP { set; get; }

    [XmlElement("RightWall")]
    public bool RIGHT { set; get; }

    [XmlElement("BottomWall")]
    public bool BOTTOM { set; get; }
    //Useful for generating the maze, that's all, need not be serialized

    public Node()
    {
        TOP = BOTTOM = RIGHT = LEFT = true;
    }
};