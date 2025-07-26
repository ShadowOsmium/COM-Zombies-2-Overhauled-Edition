public class EnemySpawnInfo
{
    public EnemyType EType { get; set; }

    public int Weight { get; set; }

    public int Count { get; set; }

    public SpawnFromType From { get; set; }

    public EnemySpawnInfo Clone()
    {
        return new EnemySpawnInfo
        {
            EType = this.EType,
            Count = this.Count,
            From = this.From
        };
    }

    public void TryApplyBoost(int boostIntervalCount, float baseChance, float decayFactor = 0.75f)
    {
        float chance = baseChance;

        for (int i = 0; i < boostIntervalCount; i++)
        {
            if (UnityEngine.Random.value < chance)
            {
                Count += 1;
                chance *= decayFactor;
                break;
            }
        }
    }

}
