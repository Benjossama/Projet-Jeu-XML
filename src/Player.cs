public class Player : Person
{
    public Player(int x, int y) : base(x, y) { }
    public override string ToString()
    {
        String p = ">";
        switch (getOrientation())
        {
            case Orientation.NORTH:
                p = "^";
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