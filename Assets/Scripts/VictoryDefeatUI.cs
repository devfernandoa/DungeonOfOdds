using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VictoryDefeatUI : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI floorText;
    public Button gambleButton;
    public Button continueButton;

    public void Show(bool playerWon)
    {
        gameObject.SetActive(true);

        int floor = BattleDataManager.Instance.currentFloor;
        resultText.text = playerWon ? "Victory!" : "Defeat...";
        floorText.text = $"Floor Reached: {floor}";

        // Set button label
        continueButton.GetComponentInChildren<TextMeshProUGUI>().text = playerWon ? "Continue" : "Retry";

        // Hook buttons
        gambleButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();

        gambleButton.onClick.AddListener(() =>
        {
            // GambleSystem.Instance.OpenGamblingUI(); // your gambling core logic
        });

        continueButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            GameLoopController.Instance.ContinueAfterBattle(playerWon); // loads next battle or resets
        });

        BattleDataManager.Instance.SaveProgress(); // Save here always
    }
}
