using UnityEngine;

public class GambleGameData
{
    public int wager;
    public int totalLuck;
    public string playerChoice; // e.g., "Red", "17", "Heads"
}
public static class LuckMath
{
    /// <summary>
    /// Returns a win chance based on total luck.
    /// Negative luck: Chance decreases below 40%
    /// 0 luck = 40% chance
    /// 100 luck = 50% chance
    /// Above 100: Logarithmic scale approaching 100%
    /// </summary>
    public static float GetLuckWinModifier(int totalLuck)
    {
        if (totalLuck < 0)
        {
            // For negative luck, linearly decrease chance from 40% down to 0%
            // At -100 luck, chance would be 30% (40% - 10%)
            // You can adjust the scaling factor to make it decrease faster/slower
            return 0.4f + (0.2f * (Mathf.Clamp(totalLuck, -100, 0) / 100f));
        }
        else if (totalLuck <= 100)
        {
            // Linear interpolation between 40% and 50% for 0-100 luck
            return 0.4f + (0.1f * (Mathf.Clamp(totalLuck, 0, 100) / 100f));
        }
        else
        {
            // Logarithmic scale above 100 luck
            float x = Mathf.Max(101f, totalLuck);
            float logLuck = Mathf.Log(x - 100f); // Start from 1 when luck=101
            float maxLog = Mathf.Log(100000f); // Adjust this to control how quickly it approaches 100%
            float progress = logLuck / maxLog;
            return 0.5f + (0.5f * Mathf.Clamp01(progress));
        }
    }
}