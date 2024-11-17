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
}
