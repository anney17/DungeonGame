using System;

namespace DungeonGame.Models;

public class Player
{
    public string Name;
    public HeroClass Class;
    public int Health;
    public int MaxHealth;
    public int Attack;
    public int Defense;
    public int Mana;
    public int MaxMana;
    public int Potions;
    public int Gold;
    public int Level;
    public int Experience;
    public bool IsDefending;

    // Поля для способностей (Задание 8)
    public int AbilityCooldown;
    public int RageTurns;

    public Player(string name, HeroClass heroClass,
                  int health, int attack, int defense,
                  int mana, int potions)
    {
        Name = name;
        Class = heroClass;
        Health = health;
        MaxHealth = health;
        Attack = attack;
        Defense = defense;
        Mana = mana;
        MaxMana = mana;
        Potions = potions;
        Gold = 0;
        Level = 1;
        Experience = 0;
        IsDefending = false;
        AbilityCooldown = 0;
        RageTurns = 0;
    }

    // === Базовые методы ===
    public bool IsAlive()
    {
        return Health > 0;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
        }
    }

    // Удар врага: урон = атака - броня; при защите урон вдвое меньше,
    // после удара защита снимается; урон не меньше 1.
    public int TakeHit(int attackPower)
    {
        int damage = attackPower - Defense;
        if (IsDefending)
        {
            damage /= 2;
            IsDefending = false;
        }
        if (damage < 1)
        {
            damage = 1;
        }
        TakeDamage(damage);
        return damage;
    }

    // Восстанавливает здоровье, но не выше MaxHealth.
    // Возвращает, сколько восстановлено.
    public int Heal(int amount)
    {
        int before = Health;
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        return Health - before;
    }

    // Если зелья есть и здоровье не максимальное — тратит зелье и лечит на 30.
    public bool UsePotion()
    {
        if (Potions <= 0)
        {
            return false;
        }
        if (Health >= MaxHealth)
        {
            return false;
        }
        Potions--;
        Heal(30);
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public bool TrySpendGold(int price)
    {
        if (Gold < price)
        {
            return false;
        }
        Gold -= price;
        return true;
    }

    // === Способности (Задание 8) ===
    public int GetCurrentAttack()
    {
        if (RageTurns > 0)
        {
            return Attack * 3 / 2;
        }
        return Attack;
    }

    public bool CanUseAbility()
    {
        if (AbilityCooldown > 0)
        {
            return false;
        }
        if (Class == HeroClass.Mage && Mana < 20)
        {
            return false;
        }
        return true;
    }

    // Применяет способность класса. Возвращает урон, нанесённый врагу.
    public int UseAbility(Enemy enemy)
    {
        int damage = 0;
        switch (Class)
        {
            case HeroClass.Warrior:
                RageTurns = 3;
                AbilityCooldown = 5;
                damage = enemy.TakeHit(GetCurrentAttack());
                break;

            case HeroClass.Mage:
                // Огненный шар: урон = атака × 2 без учёта брони; стоит 20 маны
                Mana -= 20;
                AbilityCooldown = 3;
                damage = Attack * 2;
                enemy.TakeDamage(damage);
                break;

            case HeroClass.Rogue:
                // Ядовитый клинок: обычный удар + яд (6 урона, 3 хода)
                AbilityCooldown = 4;
                damage = enemy.TakeHit(GetCurrentAttack());
                enemy.ApplyPoison(6, 3);
                break;
        }
        return damage;
    }

    // Вызывается в конце каждого хода
    public void EndTurn()
    {
        if (AbilityCooldown > 0)
        {
            AbilityCooldown--;
        }
        if (RageTurns > 0)
        {
            RageTurns--;
        }
        // Маг восстанавливает 5 маны
        if (Class == HeroClass.Mage)
        {
            Mana += 5;
            if (Mana > MaxMana)
            {
                Mana = MaxMana;
            }
        }
    }

    // Вызывается в начале каждого боя
    public void ResetBattleState()
    {
        AbilityCooldown = 0;
        RageTurns = 0;
        IsDefending = false;
    }

    // === Опыт и уровни (Задание 9) ===
    public bool AddExperience(int amount)
    {
        Experience += amount;
        int needed = Level * 100;
        if (Experience < needed)
        {
            return false;
        }
        Experience -= needed;
        LevelUp();
        return true;
    }

    private void LevelUp()
    {
        Level++;
        switch (Class)
        {
            case HeroClass.Warrior:
                MaxHealth += 20;
                Attack += 2;
                Defense += 1;
                break;
            case HeroClass.Mage:
                MaxHealth += 10;
                Attack += 3;
                MaxMana += 15;
                break;
            case HeroClass.Rogue:
                MaxHealth += 12;
                Attack += 3;
                break;
        }
        // Полное восстановление
        Health = MaxHealth;
        Mana = MaxMana;
    }
}