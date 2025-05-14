using UnityEngine;

public class CoinFlipGame : IGambleGame
{
    public string Name => "Coin Flip";
    public float BaseWinChance => 0.4f; // less than 50%

    public bool PlayGame(GambleGameData data, out bool isWin, out int payout)
    {
        float chance = LuckMath.GetLuckWinModifier(data.totalLuck, BaseWinChance);
        float roll = Random.value;
        isWin = roll < chance;
        payout = isWin ? data.wager * 2 : 0;

        Debug.Log($"[CoinFlip] Roll: {roll:F2}, Chance: {chance:P1}, Luck: {data.totalLuck}, Win: {isWin}");
        return isWin;
    }

    public int CalculatePayout(int wager, GambleGameData data)
    {
        return wager * 2;
    }
}
