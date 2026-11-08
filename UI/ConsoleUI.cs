using System;
using DungeonGame.Models;

namespace DungeonGame.UI;

public static class ConsoleUI
{
    // === Общее ===
    public static void ShowTitle()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("       ПОДЗЕМЕЛЬЕ: ПУТЬ ГЕРОЯ");
        Console.WriteLine("========================================");
    }

    public static int ReadChoice(int min, int max)
    {
        while (true)
        {
            Console.Write("Ваш выбор: ");
            string input = Console.ReadLine() ?? "";
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine($"Ошибка. Введите число от {min} до {max}.");
                continue;
            }
            if (choice < min || choice > max)
            {
                Console.WriteLine($"Ошибка. Введите число от {min} до {max}.");
                continue;
            }
            return choice;
        }
    }

    // === Создание персонажа ===
    public static HeroClass ChooseHeroClass()
    {
        Console.WriteLine();
        Console.WriteLine("=== ВЫБОР КЛАССА ===");
        Console.WriteLine("1 — Воин");
        Console.WriteLine("2 — Маг");
        Console.WriteLine("3 — Разбойник");
        int choice = ReadChoice(1, 3);
        if (choice == 1) return HeroClass.Warrior;
        if (choice == 2) return HeroClass.Mage;
        return HeroClass.Rogue;
    }

    public static void ShowPlayer(Player player)
    {
        Console.WriteLine();
        Console.WriteLine("============================");
        Console.WriteLine("    Созданный персонаж");
        Console.WriteLine("============================");
        Console.WriteLine();
        Console.WriteLine($"Имя = {player.Name}");
        Console.WriteLine($"Класс = {ClassToString(player.Class)}");
        Console.WriteLine();
        Console.WriteLine("-----Характеристики-----");
        Console.WriteLine($"Здоровье = {player.Health}/{player.MaxHealth}");
        Console.WriteLine($"Атака = {player.Attack}");
        Console.WriteLine($"Защита = {player.Defense}");
        if (player.Class == HeroClass.Mage)
        {
            Console.WriteLine($"Мана = {player.Mana}/{player.MaxMana}");
        }
        Console.WriteLine($"Зелья = {player.Potions}");
        Console.WriteLine($"Золото = {player.Gold}");
        Console.WriteLine($"Уровень {player.Level} (опыт {player.Experience} / {player.Level * 100})");
    }

    public static string ClassToString(HeroClass heroClass)
    {
        if (heroClass == HeroClass.Warrior) return "Воин";
        if (heroClass == HeroClass.Mage) return "Маг";
        return "Разбойник";
    }

    // === Сложность ===
    public static double ChooseDifficulty()
    {
        Console.WriteLine();
        Console.WriteLine("Выберите сложность подземелья:");
        Console.WriteLine("1 — Лёгкий");
        Console.WriteLine("2 — Обычный");
        Console.WriteLine("3 — Сложный");
        int choice = ReadChoice(1, 3);
        if (choice == 1)
        {
            Console.WriteLine("Вы выбрали лёгкую сложность.");
            return 0.8;
        }
        if (choice == 2)
        {
            Console.WriteLine("Вы выбрали обычную сложность.");
            return 1.0;
        }
        Console.WriteLine("Вы выбрали сложную сложность.");
        return 1.3;
    }

    // === Враг ===
    public static void ShowEnemy(Enemy enemy)
    {
        Console.WriteLine($"{enemy.Name}");
        Console.WriteLine($"HP: {enemy.Health} / {enemy.MaxHealth}");
        Console.WriteLine($"Атака: {enemy.Attack}");
        Console.WriteLine($"Броня: {enemy.Defense}");
    }

    // === Бой ===
    public static void ShowBattleStatus(Player player, Enemy enemy)
    {
        Console.WriteLine();
        Console.WriteLine($"Ваше здоровье: {player.Health} / {player.MaxHealth}");
        if (player.Class == HeroClass.Mage)
        {
            Console.WriteLine($"Мана: {player.Mana} / {player.MaxMana}");
        }
        Console.WriteLine($"{enemy.Name}: {enemy.Health} / {enemy.MaxHealth} HP");
        Console.WriteLine($"Зелий: {player.Potions}");
    }

    public static void ShowBattleMenu(HeroClass heroClass, int cooldown)
    {
        Console.WriteLine();
        Console.WriteLine("1 — Атаковать");
        Console.WriteLine("2 — Использовать зелье");
        Console.WriteLine("3 — Защищаться");
        Console.WriteLine("4 — Сбежать");
        string abilityName = GetAbilityName(heroClass);
        string status = cooldown > 0 ? $"через {cooldown} хода" : "готово";
        Console.WriteLine($"5 — {abilityName} ({status})");
    }

    public static string GetAbilityName(HeroClass heroClass)
    {
        if (heroClass == HeroClass.Warrior) return "Ярость";
        if (heroClass == HeroClass.Mage) return "Огненный шар";
        return "Ядовитый клинок";
    }

    public static void ShowAbilityUsed(Player player, int damage)
    {
        Console.WriteLine($"Вы применили способность «{GetAbilityName(player.Class)}»!");
        Console.WriteLine($"Нанесено {damage} урона.");
    }

    // === Награда и уровень ===
    public static void ShowReward(Player player, Enemy enemy, bool levelUp)
    {
        Console.WriteLine($"{enemy.Name} повержен!");
        Console.WriteLine($"Получено золота: {enemy.GoldReward}");
        Console.WriteLine($"Получено опыта: {enemy.ExperienceReward}");
        if (levelUp)
        {
            Console.WriteLine($"ПОВЫШЕНИЕ УРОВНЯ! Теперь уровень {player.Level}.");
            Console.WriteLine($"Здоровье: {player.Health}/{player.MaxHealth}, Атака: {player.Attack}");
        }
    }

    // === Статистика ===
    public static void ShowStatistics(GameStatistics stats)
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("       СТАТИСТИКА ПРИКЛЮЧЕНИЯ");
        Console.WriteLine("========================================");
        Console.WriteLine($"Пройдено комнат: {stats.RoomsPassed}");
        Console.WriteLine($"Побеждено врагов: {stats.EnemiesDefeated}");
        Console.WriteLine($"Использовано зелий: {stats.PotionsUsed}");
        Console.WriteLine($"Нанесено урона: {stats.DamageDealt}");
        Console.WriteLine($"Получено урона: {stats.DamageTaken}");
        Console.WriteLine("========================================");
    }

    // === Финал ===
    public static void FinishGame(Player player)
    {
        if (!player.IsAlive())
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("              ПОРАЖЕНИЕ");
            Console.WriteLine("========================================");
            Console.WriteLine("Ваш персонаж погиб.");
            Console.WriteLine("Подземелье оказалось слишком опасным.");
        }
        else
        {
            Console.WriteLine("Игра завершена.");
        }
    }
}