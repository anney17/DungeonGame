namespace DungeonGame.Models;

public class GameStatistics
{
    public int RoomsPassed;
    public int EnemiesDefeated;
    public int PotionsUsed;
    public int DamageDealt;
    public int DamageTaken;

    public void RegisterRoom() => RoomsPassed++;
    public void RegisterKill() => EnemiesDefeated++;
    public void RegisterPotion() => PotionsUsed++;
    public void RegisterDamageDealt(int amount) => DamageDealt += amount;
    public void RegisterDamageTaken(int amount) => DamageTaken += amount;
}