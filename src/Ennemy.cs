public class Enemy : Person
{
    private bool Alive = true;
    public Enemy(int x, int y) : base(x, y) { }

    public void Kill()
    {
        Alive = false;
    }

    public bool IsAlive()
    {
        return Alive;
    }

    new public bool Move(Node playerLocation)
    {
        while (!base.Move(playerLocation))
        {
            Rotate();
        }
        return true;
    }

}