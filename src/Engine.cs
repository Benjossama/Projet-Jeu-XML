class Engine
{
    private Engine() { }
    static public void Print(Maze maze, Enemy[]? enemies, Player? player, string text)
    {
        Console.Clear();
        Console.WriteLine(text);

        int width = maze.getWidth();
        int height = maze.getHeight();

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Console.Write(maze.getNode(x, y).GetTopWall() ? "+---" : "+   ");
            }
            Console.WriteLine("+");

            for (int y = 0; y < width; y++)
            {
                bool hasLeftWall = maze.getNode(x, y).GetLeftWall();
                string nodeContent = "   ";
                bool nodeContentIsEnemy = false;

                if (player != null && player.getX() == x && player.getY() == y)
                {
                    nodeContent = $" {player} ";
                }
                else if (enemies != null)
                {
                    int enemyIndex = IsEnemyInNode(enemies, x, y);
                    if (enemyIndex > -1)
                    {
                        Enemy enemy = enemies[enemyIndex];
                        nodeContent = enemy.IsAlive() ? $" {enemy} " : " . ";
                        nodeContentIsEnemy = true;
                    }
                }

                Console.Write(hasLeftWall ? "|" : " ");
                // Print enemies in red, the rest in white
                Console.Write(nodeContentIsEnemy ? $"\u001b[31m{nodeContent}\u001b[0m" : nodeContent);

            }
            Console.WriteLine("|");
        }

        // Affichage de la dernière ligne horizontale
        for (int y = 0; y < width; y++)
        {
            Console.Write(maze.getNode(height - 1, y).GetBottomWall() ? "+---" : "+   ");
        }
        Console.WriteLine("+");
    }


    static private int IsEnemyInNode(Enemy[] enemies, int x, int y)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].getX() == x && enemies[i].getY() == y)
            {
                return i; // Retourne l'indice de l'ennemi trouvé
            }
        }
        return -1; // Aucun ennemi trouvé
    }

}