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
    protected Orientation orientation;
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
    public Orientation GetOrientation()
    {
        return orientation;
    }
    public void setOrientation(Orientation orientation)
    {
        this.orientation = orientation;
    }

    protected Orientation NextOrientation(Orientation orientation)
    {
        return orientation == Orientation.WEST ? Orientation.NORTH : orientation + 1;
    }

    protected Orientation PreviousOrientation(Orientation orientation)
    {
        return orientation == Orientation.NORTH ? Orientation.WEST : orientation - 1;
    }


    public bool Move(Node playerLocation)
    {
        // Check if the current cell has a wall in the side facing the orientation of the person
        if (GetOrientation() == Orientation.NORTH && !playerLocation.GetTopWall())
        {
            setX(getX() - 1);
            return true;

        }
        else if (GetOrientation() == Orientation.EAST && !playerLocation.GetRightWall())
        {
            setY(getY() + 1);
            return true;

        }
        else if (GetOrientation() == Orientation.SOUTH && !playerLocation.GetBottomWall())
        {
            setX(getX() + 1);
            return true;

        }
        else if (GetOrientation() == Orientation.WEST && !playerLocation.GetLeftWall())
        {
            setY(getY() - 1);
            return true;

        }
        return false;
    }

    public override string ToString()
    {
        String p = ">";
        switch (GetOrientation())
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
