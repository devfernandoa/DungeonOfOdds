using UnityEngine;

public class SlotMachineGame : IGambleGame
{
    public string Name => "Slot Machine";
    public float BaseWinChance => 0.15f; // lower than expected

    private string[] symbols = { "🍒", "🍋", "🔔", "💎", "7️⃣" };

    public bool PlayGame(GambleGameData data, out bool isWin, out int payout)
    {
        float chance = LuckMath.GetLuckWinModifier(data.totalLuck, BaseWinChance);

        // Determinar primeiro se vamos ganhar com base na chance
        isWin = Random.value < chance;

        int slot1, slot2, slot3;

        if (isWin)
        {
            // Se for para ganhar, forçar um match (três símbolos iguais)
            int winningSymbol = Random.Range(0, symbols.Length);
            slot1 = winningSymbol;
            slot2 = winningSymbol;
            slot3 = winningSymbol;
        }
        else
        {
            // Se for para perder, gerar resultado aleatório sem garantia de match
            slot1 = Random.Range(0, symbols.Length);
            slot2 = Random.Range(0, symbols.Length);
            slot3 = Random.Range(0, symbols.Length);

            // Se por acaso saiu um match, força pelo menos um símbolo diferente
            if (slot1 == slot2 && slot2 == slot3)
            {
                // Altera um dos slots para evitar um match acidental
                slot3 = (slot3 + 1) % symbols.Length;
            }
        }

        payout = isWin ? data.wager * 5 : 0;

        string result = $"{symbols[slot1]} | {symbols[slot2]} | {symbols[slot3]}";
        Debug.Log($"[SlotMachine] Spin: {result} | Luck: {data.totalLuck} | WinChance: {chance:P1} | Win: {isWin}");

        return isWin;
    }

    public int CalculatePayout(int wager, GambleGameData data)
    {
        return wager * 5;
    }
}