using DungeonGame.Models;

namespace DungeonGame.Gameplay;

public static class EnemyGenerator
{
    public static Enemy CreateRandom(Random random, double difficulty)
    {
        Enemy enemy;
        int type = random.Next(1, 4);
        if (type == 1)
        {
            enemy = new Enemy("Гоблин", 40, 8, 2, 20, 5);
        }
        else if (type == 2)
        {
            enemy = new Enemy("Орк", 70, 12, 4, 35, 10);
        }
        else
        {
            enemy = new Enemy("Скелет", 100, 16, 6, 50, 15);
        }
        enemy.ApplyDifficulty(difficulty);
        return enemy;
    }

    public static Enemy CreateStrong(Random random, double difficulty)
    {
        Enemy enemy = CreateRandom(random, difficulty * 1.5);
        enemy.Name = "Сильный " + enemy.Name;
        enemy.ExperienceReward *= 2;
        enemy.GoldReward *= 2;
        return enemy;
    }

    public static Enemy CreateBoss(double difficulty)
    {
        Enemy boss = new Enemy("Хранитель Тьмы", 150, 20, 8, 200, 100);
        boss.ApplyDifficulty(difficulty);
        return boss;
    }
}