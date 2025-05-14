using UnityEngine;

public class RouletteGame : IGambleGame
{
    public string Name => "Roulette";
    public float BaseWinChance => 10f / 37f; // for red/black (18 números de 37)
    private const float GreenBaseWinChance = 1f / 37f; // for green (apenas o zero)

    public bool PlayGame(GambleGameData data, out bool isWin, out int payout)
    {
        string choice = data.playerChoice.ToLowerInvariant();
        float chance;

        if (choice == "green")
        {
            // Chance base para verde é 1/37, modificada por sorte
            chance = LuckMath.GetLuckWinModifier(data.totalLuck, GreenBaseWinChance);
            isWin = Random.value < chance;
            payout = isWin ? data.wager * 35 : 0;
        }
        else if (choice == "red" || choice == "black")
        {
            // Chance base para vermelho/preto é 18/37, modificada por sorte
            chance = LuckMath.GetLuckWinModifier(data.totalLuck, BaseWinChance);
            isWin = Random.value < chance;
            payout = isWin ? data.wager * 2 : 0;
        }
        else
        {
            chance = 0f;
            isWin = false;
            payout = 0;
        }

        Debug.Log($"[Roulette] Choice: {choice}, Chance: {chance:P2}, Win: {isWin}, Payout: {payout}");
        return isWin;
    }

    public int CalculatePayout(int wager, GambleGameData data)
    {
        return data.playerChoice.ToLower() == "green" ? wager * 35 : wager * 2;
    }
}