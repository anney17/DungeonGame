namespace DungeonGame.Models;

public class Enemy
{
    public string Name;
    public int Health;
    public int MaxHealth;
    public int Attack;
    public int Defense;
    public int ExperienceReward;
    public int GoldReward;

    // Поля для яда (Задание 8)
    public int PoisonTurns;
    public int PoisonDamage;

    public Enemy(string name, int health, int attack,
                 int defense, int experienceReward, int goldReward)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
        Attack = attack;
        Defense = defense;
        ExperienceReward = experienceReward;
        GoldReward = goldReward;
        PoisonTurns = 0;
        PoisonDamage = 0;
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    // Урон без учёта брони (ловушка, яд, магия)
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
        }
    }

    // Удар оружием: броня уменьшает урон
    public int TakeHit(int attackPower)
    {
        int damage = attackPower - Defense;
        if (damage < 1)
        {
            damage = 1;
        }
        TakeDamage(damage);
        return damage;
    }

    // Усиливает/ослабляет врага под сложность
    public void ApplyDifficulty(double multiplier)
    {
        MaxHealth = (int)(MaxHealth * multiplier);
        Health = MaxHealth;
        Attack = (int)(Attack * multiplier);
    }

    // === Яд (Задание 8) ===
    public void ApplyPoison(int damagePerTurn, int turns)
    {
        PoisonDamage = damagePerTurn;
        PoisonTurns = turns;
    }

    // Урон ядом в начале хода врага.
    // Возвращает урон (0, если яда нет).
    public int ProcessPoison()
    {
        if (PoisonTurns <= 0)
        {
            return 0;
        }
        PoisonTurns--;
        TakeDamage(PoisonDamage);
        return PoisonDamage;
    }
}