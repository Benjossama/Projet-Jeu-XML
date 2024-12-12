using System.Xml.Serialization;

[Serializable]
public class Player : Person
{
    private Player() { }
    public Player(int x, int y) : base(x, y) { }
}