class Engine
{
    private Engine() { }
    static public void Print(Maze maze, Enemy[]? enemies, Player? player, string text)
    {
        Console.Clear();
        Console.WriteLine(text);

        int width = maze.Width;
        int height = maze.Height;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Console.Write(maze.GetNode(x, y).TOP ? "+---" : "+   ");
            }
            Console.WriteLine("+");

            for (int y = 0; y < width; y++)
            {
                bool hasLeftWall = maze.GetNode(x, y).LEFT;
                string nodeContent = "   ";
                bool nodeContentIsEnemy = false;

                if (player != null && player.X == x && player.Y == y)
                {
                    nodeContent = $" {player} ";
                }
                else if (enemies != null)
                {
                    int enemyIndex = IsEnemyInNode(enemies, x, y);
                    if (enemyIndex > -1)
                    {
                        Enemy enemy = enemies[enemyIndex];
                        nodeContent = enemy.Alive ? $" {enemy} " : " . ";
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
            Console.Write(maze.GetNode(height - 1, y).BOTTOM ? "+---" : "+   ");
        }
        Console.WriteLine("+");
    }


    static private int IsEnemyInNode(Enemy[] enemies, int x, int y)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].X == x && enemies[i].Y == y)
            {
                return i; // Retourne l'indice de l'ennemi trouvé
            }
        }
        return -1; // Aucun ennemi trouvé
    }

}