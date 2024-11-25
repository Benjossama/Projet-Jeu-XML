public enum Orientation
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}
abstract public class Person
{
    private int x;
    private int y;
    private Orientation orientation;
    public Person(int x, int y)
    {
        this.x = x;
        this.y = y;
        orientation = Orientation.EAST;
    }
    public int getX()
    {
        return x;
    }
    public void setX(int x)
    {
        this.x = x;
    }
    public int getY()
    {
        return y;
    }

    public void setY(int y)
    {
        this.y = y;
    }
    public Orientation getOrientation()
    {
        return orientation;
    }
    public void setOrientation(Orientation orientation)
    {
        this.orientation = orientation;
    }

    public void Rotate()
    {
        Orientation[] values = (Orientation[])Enum.GetValues(typeof(Orientation));
        int nextIndex = ((int)orientation + new Random().Next(100)) % values.Length;

        this.orientation = values[nextIndex];
    }

    public bool Move(Node playerLocation)
    {
        // Check if the current cell has a wall in the side facing the orientation of the person
        if (getOrientation() == Orientation.NORTH && !playerLocation.GetTopWall())
        {
            setX(getX() - 1);
            return true;

        }
        else if (getOrientation() == Orientation.EAST && !playerLocation.GetRightWall())
        {
            setY(getY() + 1);
            return true;

        }
        else if (getOrientation() == Orientation.SOUTH && !playerLocation.GetBottomWall())
        {
            setX(getX() + 1);
            return true;

        }
        else if (getOrientation() == Orientation.WEST && !playerLocation.GetLeftWall())
        {
            setY(getY() - 1);
            return true;

        }
        return false;
    }

    public override string ToString()
    {
        String p = ">";
        switch (getOrientation())
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
