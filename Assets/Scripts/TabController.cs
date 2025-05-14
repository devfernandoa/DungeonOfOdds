using UnityEngine;

public class TabController : MonoBehaviour
{
    public GameObject fightPanel;
    public GameObject teamPanel;
    public GameObject shopPanel;
    public GameObject gamblePanel;
    public GameObject victoryPanel;

    public void ShowFightTab()
    {
        fightPanel.SetActive(true);
        teamPanel.SetActive(false);
        shopPanel.SetActive(false);
        gamblePanel.SetActive(false);
        victoryPanel.SetActive(false);

        FindObjectOfType<BattleController>()?.StartBattle();
    }

    public void ShowTeamTab()
    {
        fightPanel.SetActive(false);
        teamPanel.SetActive(true);
        shopPanel.SetActive(false);
        gamblePanel.SetActive(false);
    }

    public void ShowShopTab()
    {
        fightPanel.SetActive(false);
        teamPanel.SetActive(false);
        shopPanel.SetActive(true);
        gamblePanel.SetActive(false);
    }

    public void ShowGambleTab()
    {
        fightPanel.SetActive(false);
        teamPanel.SetActive(false);
        shopPanel.SetActive(false);
        gamblePanel.SetActive(true);
        victoryPanel.SetActive(false);
    }

    void Start()
    {
        ShowFightTab(); // Show the fight tab by default
    }
}
