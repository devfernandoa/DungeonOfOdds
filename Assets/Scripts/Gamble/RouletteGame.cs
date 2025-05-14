using UnityEngine;

public class RouletteGame : IGambleGame
{
    public string Name => "Roulette";
    public float BaseWinChance => 15f / 37f; // for red/black

    public bool PlayGame(GambleGameData data, out bool isWin, out int payout)
    {
        int roll = Random.Range(0, 37);
        string choice = data.playerChoice.ToLowerInvariant();

        if (choice == "green")
        {
            isWin = (roll == 0);
            payout = isWin ? data.wager * 14 : 0;
        }
        else if (choice == "red" || choice == "black")
        {
            bool isRed = roll % 2 == 0 && roll != 0;
            bool pickedRed = choice == "red";
            bool baseMatch = isRed == pickedRed;

            if (baseMatch)
            {
                float chance = LuckMath.GetLuckWinModifier(data.totalLuck);
                isWin = Random.value < chance;
            }
            else
            {
                isWin = false;
            }

            payout = isWin ? data.wager * 2 : 0;
        }
        else
        {
            isWin = false;
            payout = 0;
        }

        Debug.Log($"[Roulette] Rolled: {roll}, Pick: {choice}, Win: {isWin}");
        return isWin;
    }

    public int CalculatePayout(int wager, GambleGameData data)
    {
        return data.playerChoice.ToLower() == "green" ? wager * 14 : wager * 2;
    }
}
