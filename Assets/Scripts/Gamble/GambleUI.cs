using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GambleUI : MonoBehaviour
{
    public TMP_Dropdown gameDropdown;
    public TMP_InputField wagerInput;
    public TMP_Dropdown choiceDropdown;
    public Button playButton;
    public TMP_Text resultText;
    public TMP_Text winChanceText;


    private Dictionary<GambleGameType, IGambleGame> games = new();
    private IGambleGame currentGame;

    private void Start()
    {
        // Register games
        games[GambleGameType.CoinFlip] = new CoinFlipGame();
        games[GambleGameType.Roulette] = new RouletteGame();
        games[GambleGameType.SlotMachine] = new SlotMachineGame();
        games[GambleGameType.MysteryBox] = new MysteryBoxGame();

        // Populate dropdown
        gameDropdown.ClearOptions();
        gameDropdown.AddOptions(new List<string> { "Coin Flip", "Roulette", "Slot Machine", "Mystery Box" });
        gameDropdown.onValueChanged.AddListener(OnGameChanged);
        OnGameChanged(0);

        playButton.onClick.AddListener(PlaySelectedGame);

        UpdateWinChancePreview();

        wagerInput.onValueChanged.AddListener(_ => UpdateWinChancePreview());
        choiceDropdown.onValueChanged.AddListener(_ => UpdateWinChancePreview());
        gameDropdown.onValueChanged.AddListener(_ => UpdateWinChancePreview());
    }
    void Update()
    {
        playButton.interactable =
            int.TryParse(wagerInput.text, out int wager) &&
            wager > 0 &&
            wager <= PointsManager.Instance.currentPoints;

        UpdateWinChancePreview();
    }

    void OnGameChanged(int index)
    {
        currentGame = games[(GambleGameType)index];

        bool usesChoice = currentGame is RouletteGame || currentGame is MysteryBoxGame;
        choiceDropdown.gameObject.SetActive(usesChoice);

        if (currentGame is MysteryBoxGame)
        {
            choiceDropdown.ClearOptions();
            choiceDropdown.AddOptions(new List<string> { "1", "2", "3", "4", "5" });
        }
        else if (currentGame is RouletteGame)
        {
            choiceDropdown.ClearOptions();
            choiceDropdown.AddOptions(new List<string> { "Red", "Black", "Green" });
        }

        resultText.text = "";
    }

    void PlaySelectedGame()
    {
        if (!int.TryParse(wagerInput.text, out int wager) || wager <= 0)
        {
            resultText.text = "Invalid wager.";
            return;
        }

        if (wager > PointsManager.Instance.currentPoints)
        {
            resultText.text = "Not enough points.";
            return;
        }

        string choice = choiceDropdown.options[choiceDropdown.value].text;
        int totalLuck = 0;

        foreach (var fighter in BattleDataManager.Instance.selectedFighters)
        {
            if (fighter != null)
                totalLuck += fighter.luck;
        }

        var data = new GambleGameData
        {
            wager = wager,
            totalLuck = totalLuck,
            playerChoice = choice
        };

        bool win = currentGame.PlayGame(data, out bool isWin, out int payout);

        if (isWin)
        {
            PointsManager.Instance.AddPoints(payout);
            resultText.text = $"You won {payout} points!";
        }
        else
        {
            PointsManager.Instance.SpendPoints(wager);
            resultText.text = "You lost the gamble.";
        }
    }
    void UpdateWinChancePreview()
    {
        if (winChanceText == null || currentGame == null) return;

        int totalLuck = 0;
        foreach (var fighter in BattleDataManager.Instance.selectedFighters)
            if (fighter != null)
                totalLuck += fighter.luck;

        float baseChance = currentGame.BaseWinChance;
        float modifier = LuckMath.GetLuckWinModifier(totalLuck, baseChance);
        Debug.Log($"Base Chance: {baseChance}, Luck Modifier: {modifier}");

        if (currentGame is RouletteGame && !string.IsNullOrEmpty(choiceDropdown.options[choiceDropdown.value].text))
        {
            string choice = choiceDropdown.options[choiceDropdown.value].text.ToLower();
            if (choice == "green")
            {
                baseChance = 1f / 37f;
                modifier = LuckMath.GetLuckWinModifier(totalLuck, baseChance);
            }
        }

        winChanceText.text = $"Win Chance: {(modifier * 100f):F1}%";
    }
}
