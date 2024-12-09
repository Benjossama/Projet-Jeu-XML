using System.Xml.Serialization;

public enum Orientation
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

[Serializable]
abstract public class Person
{
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
    [XmlElement("Orientation")]
    public Orientation Orientation { get; set; }

    protected Person() { }
    public Person(int x, int y)
    {
        X = x;
        Y = y;
        Orientation = Orientation.EAST;
    }

    protected Orientation NextOrientation(Orientation Orientation)
    {
        return Orientation == Orientation.WEST ? Orientation.NORTH : Orientation + 1;
    }

    protected Orientation PreviousOrientation(Orientation Orientation)
    {
        return Orientation == Orientation.NORTH ? Orientation.WEST : Orientation - 1;
    }


    public bool Move(Node playerLocation)
    {
        // Check if the current cell has a wall in the side facing the orientation of the person
        if (Orientation == Orientation.NORTH && !playerLocation.TOP)
        {
            X = X - 1;
            return true;

        }
        else if (Orientation == Orientation.EAST && !playerLocation.RIGHT)
        {
            Y = Y + 1;
            return true;

        }
        else if (Orientation == Orientation.SOUTH && !playerLocation.BOTTOM)
        {
            X = X + 1;
            return true;

        }
        else if (Orientation == Orientation.WEST && !playerLocation.LEFT)
        {
            Y = Y - 1;
            return true;

        }
        return false;
    }

    public override string ToString()
    {
        String p = ">";
        switch (Orientation)
        {
            case Orientation.NORTH:
                p = "ʌ";
                break;
            case Orientation.EAST:
                p = ">";
                break;
            case Orientation.SOUTH:
                p = "v";
                break;
            case Orientation.WEST:
                p = "<";
                break;
            default:
                break;
        }
        return p;
    }
}
