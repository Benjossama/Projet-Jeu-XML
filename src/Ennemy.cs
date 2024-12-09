using System.Xml.Serialization;

[Serializable]
public class Enemy : Person
{
    [XmlElement("Alive")]
    private bool _Alive;
    public bool Alive
    {
        get => _Alive;
        set { }
    }

    private Enemy() { }
    public Enemy(int x, int y) : base(x, y)
    {
        _Alive = true;
    }

    public void Kill()
    {
        _Alive = false;
    }

    new public bool Move(Node playerLocation)
    {
        Orientation = PreviousOrientation(Orientation);
        while (!base.Move(playerLocation))
        {
            Orientation = NextOrientation(Orientation);
        }
        return true;
    }
}