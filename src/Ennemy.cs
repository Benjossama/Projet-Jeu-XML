using System.Xml.Serialization;

[Serializable]
public class Enemy : Person
{
    private bool _Alive;
    [XmlElement("Alive")]
    public bool Alive
    {
        get => _Alive;
        set => _Alive = value;
    }

    public Enemy() { }
    public Enemy(int x, int y) : base(x, y)
    {
        _Alive = true;
    }

    public void Kill()
    {
        _Alive = false;
    }

    new public bool Move(Node enemyLocation)
    {
        Orientation = PreviousOrientation(Orientation);
        while (!base.Move(enemyLocation))
        {
            Orientation = NextOrientation(Orientation);
        }
        return true;
    }
}