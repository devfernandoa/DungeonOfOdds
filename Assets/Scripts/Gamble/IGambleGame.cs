using UnityEngine;

public interface IGambleGame
{
    string Name { get; }
    float BaseWinChance { get; }
    int CalculatePayout(int wager, GambleGameData data);
    bool PlayGame(GambleGameData data, out bool isWin, out int payout);
}
