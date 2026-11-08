using System;
using DungeonGame.Models;
using DungeonGame.Gameplay;
using DungeonGame.UI;

namespace DungeonGame;

class Program
{
    static void Main()
    {
        Random random = new Random();

        ConsoleUI.ShowTitle();

        Player player = CreateCharacter();
        double difficulty = ConsoleUI.ChooseDifficulty();
        GameStatistics stats = new GameStatistics();

        RunDungeon(player, stats, difficulty, random);

        ConsoleUI.ShowStatistics(stats);
        ConsoleUI.FinishGame(player);
    }

    // === Создание персонажа ===
    static Player CreateCharacter()
    {
        Console.Write("Введите имя персонажа: ");
        string name = Console.ReadLine() ?? "Безымянный герой";
        HeroClass heroClass = ConsoleUI.ChooseHeroClass();

        Player player;
        switch (heroClass)
        {
            case HeroClass.Warrior:
                player = new Player(name, heroClass, 120, 18, 10, 0, 3);
                break;
            case HeroClass.Mage:
                player = new Player(name, heroClass, 80, 25, 4, 60, 5);
                break;
            default:
                player = new Player(name, heroClass, 100, 21, 6, 0, 3);
                break;
        }

        ConsoleUI.ShowPlayer(player);
        return player;
    }

    // === Прохождение подземелья ===
    static void RunDungeon(Player player, GameStatistics stats,
                           double difficulty, Random random)
    {
        for (int room = 1; room <= 5; room++)
        {
            if (!player.IsAlive()) return;

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine($"КОМНАТА {room} ИЗ 5");
            Console.WriteLine("========================================");
            Console.WriteLine("1 — Продолжить");
            Console.WriteLine("2 — Покинуть подземелье");
            int choice = ConsoleUI.ReadChoice(1, 2);

            if (choice == 2)
            {
                Console.WriteLine("Вы покинули подземелье.");
                return;
            }

            stats.RegisterRoom();
            RoomType roomType = GenerateRoom(random);
            Console.WriteLine("Вы открываете дверь...");

            ProcessRoom(roomType, player, stats, difficulty, random);
            if (!player.IsAlive()) return;
        }

        // Финальный босс
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("             ФИНАЛЬНЫЙ БОСС");
        Console.WriteLine("========================================");
        Console.WriteLine("В глубине подземелья появляется Хранитель Тьмы!");

        Enemy boss = EnemyGenerator.CreateBoss(difficulty);
        bool won = StartBattle(player, boss, stats, random);

        if (won)
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("                ПОБЕДА!");
            Console.WriteLine("========================================");
            Console.WriteLine("Вы победили Хранителя Тьмы и выбрались из подземелья.");
        }
    }

    // === Генерация комнат ===
    static RoomType GenerateRoom(Random random)
    {
        int r = random.Next(1, 101);
        if (r <= 20) return RoomType.Empty;
        if (r <= 45) return RoomType.Enemy;
        if (r <= 60) return RoomType.StrongEnemy;
        if (r <= 75) return RoomType.Trap;
        if (r <= 87) return RoomType.Treasure;
        if (r <= 95) return RoomType.Rest;
        return RoomType.Merchant;
    }

    // === Обработка комнаты ===
    static void ProcessRoom(RoomType roomType, Player player,
                            GameStatistics stats, double difficulty,
                            Random random)
    {
        switch (roomType)
        {
            case RoomType.Empty:
                Console.WriteLine("Комната пуста. Можно идти дальше.");
                break;

            case RoomType.Enemy:
            {
                Enemy enemy = EnemyGenerator.CreateRandom(random, difficulty);
                StartBattle(player, enemy, stats, random);
                break;
            }

            case RoomType.StrongEnemy:
            {
                Enemy enemy = EnemyGenerator.CreateStrong(random, difficulty);
                Console.WriteLine("Это сильный противник!");
                StartBattle(player, enemy, stats, random);
                break;
            }

            case RoomType.Trap:
                ProcessTrap(player, stats);
                break;

            case RoomType.Treasure:
                ProcessTreasure(player);
                break;

            case RoomType.Rest:
                ProcessRest(player);
                break;

            case RoomType.Merchant:
                ProcessMerchant(player);
                break;
        }
    }

    // === Ловушка ===
    static void ProcessTrap(Player player, GameStatistics stats)
    {
        Console.WriteLine("Вы попали в ловушку!");
        int trapDamage = 15;
        stats.RegisterDamageTaken(trapDamage);
        player.TakeDamage(trapDamage);
        Console.WriteLine($"Получено {trapDamage} урона.");
        Console.WriteLine($"Ваше здоровье: {player.Health}/{player.MaxHealth}");
        if (!player.IsAlive())
        {
            Console.WriteLine("Ловушка оказалась смертельной!");
        }
    }

    // === Сокровище ===
    static void ProcessTreasure(Player player)
    {
        Random r = new Random();
        Console.WriteLine("Вы нашли сундук!");
        int reward = r.Next(1, 4);
        switch (reward)
        {
            case 1:
                player.Potions++;
                Console.WriteLine($"Награда: зелье! Теперь зелий: {player.Potions}");
                break;
            case 2:
                player.Attack += 2;
                Console.WriteLine($"Награда: золото! Атака: {player.Attack}");
                break;
            default:
                player.Attack += 4;
                Console.WriteLine($"Награда: древний артефакт! Атака: {player.Attack}");
                break;
        }
    }

    // === Отдых ===
    static void ProcessRest(Player player)
    {
        Console.WriteLine("Вы нашли безопасное место для отдыха.");
        int healed = player.Heal(20);
        Console.WriteLine($"Восстановлено {healed} здоровья. Здоровье: {player.Health}/{player.MaxHealth}");
    }

    // === Торговец (Задание 9) ===
    static void ProcessMerchant(Player player)
    {
        Console.WriteLine("В комнате вас встречает торговец.");
        Console.WriteLine($"Ваше золото: {player.Gold}");
        Console.WriteLine("1 — Купить зелье (10 золота)");
        Console.WriteLine("2 — Купить увеличение атаки (15 золота)");
        Console.WriteLine("3 — Уйти");
        int choice = ConsoleUI.ReadChoice(1, 3);

        switch (choice)
        {
            case 1:
                if (player.TrySpendGold(10))
                {
                    player.Potions++;
                    Console.WriteLine($"Вы купили зелье! Зелий: {player.Potions}");
                }
                else
                {
                    Console.WriteLine("Недостаточно золота.");
                }
                break;
            case 2:
                if (player.TrySpendGold(15))
                {
                    player.Attack += 3;
                    Console.WriteLine($"Вы усилили атаку! Атака: {player.Attack}");
                }
                else
                {
                    Console.WriteLine("Недостаточно золота.");
                }
                break;
            default:
                Console.WriteLine("Вы ушли от торговца.");
                break;
        }
    }

    // === Бой ===
    static bool StartBattle(Player player, Enemy enemy,
                            GameStatistics stats, Random random)
    {
        ConsoleUI.ShowEnemy(enemy);
        player.ResetBattleState();
        bool fled = false;

        while (player.IsAlive() && enemy.IsAlive() && !fled)
        {
            fled = PlayerTurn(player, enemy, stats, random);

            if (enemy.IsAlive() && !fled)
            {
                EnemyTurn(player, enemy, stats);
            }

            if (player.IsAlive() && enemy.IsAlive() && !fled)
            {
                player.EndTurn();
            }
        }

        if (enemy.IsAlive())
        {
            return false;
        }

        stats.RegisterKill();
        player.AddGold(enemy.GoldReward);
        bool levelUp = player.AddExperience(enemy.ExperienceReward);
        ConsoleUI.ShowReward(player, enemy, levelUp);
        return true;
    }

    // === Ход игрока ===
    static bool PlayerTurn(Player player, Enemy enemy,
                           GameStatistics stats, Random random)
    {
        ConsoleUI.ShowBattleStatus(player, enemy);
        ConsoleUI.ShowBattleMenu(player.Class, player.AbilityCooldown);
        int choice = ConsoleUI.ReadChoice(1, 5);

        if (choice == 1)
        {
            int power = player.GetCurrentAttack();
            if (random.Next(100) < 20)
            {
                power *= 2;
                Console.WriteLine("КРИТИЧЕСКИЙ УДАР!");
            }
            int damage = enemy.TakeHit(power);
            stats.RegisterDamageDealt(damage);
            Console.WriteLine($"Вы наносите {damage} урона.");
        }
        else if (choice == 2)
        {
            if (player.UsePotion())
            {
                stats.RegisterPotion();
                Console.WriteLine($"Вы использовали зелье! Здоровье: {player.Health}/{player.MaxHealth}");
            }
            else
            {
                Console.WriteLine("Не удалось использовать зелье (нет зелий или здоровье максимальное).");
            }
        }
        else if (choice == 3)
        {
            player.IsDefending = true;
            Console.WriteLine("Вы заняли защитную позицию.");
        }
        else if (choice == 4)
        {
            Console.WriteLine("Вы сбежали из боя.");
            return true;
        }
        else // 5
        {
            if (player.CanUseAbility())
            {
                int damage = player.UseAbility(enemy);
                stats.RegisterDamageDealt(damage);
                ConsoleUI.ShowAbilityUsed(player, damage);
            }
            else
            {
                Console.WriteLine("Способность пока недоступна.");
            }
        }

        return false;
    }

    // === Ход противника ===
    static void EnemyTurn(Player player, Enemy enemy, GameStatistics stats)
    {
        // Обработка яда в начале хода врага
        int poison = enemy.ProcessPoison();
        if (poison > 0)
        {
            stats.RegisterDamageDealt(poison);
            Console.WriteLine($"Яд: {enemy.Name} теряет {poison} HP.");
            if (!enemy.IsAlive())
            {
                return;
            }
        }

        int damage = player.TakeHit(enemy.Attack);
        stats.RegisterDamageTaken(damage);
        Console.WriteLine($"{enemy.Name} атакует: {damage} урона.");
        Console.WriteLine($"Ваше здоровье: {player.Health}/{player.MaxHealth}");

        if (!player.IsAlive())
        {
            Console.WriteLine("Ваш персонаж погиб!");
        }
    }
}