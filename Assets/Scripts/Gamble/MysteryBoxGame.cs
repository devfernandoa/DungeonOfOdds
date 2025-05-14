using UnityEngine;

public class MysteryBoxGame : IGambleGame
{
    public string Name => "Mystery Box";
    public float BaseWinChance => 0.2f;

    public bool PlayGame(GambleGameData data, out bool isWin, out int payout)
    {
        float chance = LuckMath.GetLuckWinModifier(data.totalLuck, BaseWinChance);

        isWin = (Random.value < chance);
        payout = isWin ? data.wager * 4 : 0;

        Debug.Log($"[MysteryBox] Chance: {chance:P1}, Win: {isWin}");

        return isWin;
    }

    public int CalculatePayout(int wager, GambleGameData data)
    {
        return wager * 4;
    }
}
