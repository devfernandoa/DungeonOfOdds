using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLoopController : MonoBehaviour
{
    public BattleController battleController; // assign in Inspector
    public static GameLoopController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ContinueAfterBattle(bool playerWon)
    {
        if (playerWon)
        {
            BattleDataManager.Instance.currentFloor++;
        }
        else
        {
            int floor = BattleDataManager.Instance.currentFloor;
            bool lostAfterBoss = BattleDataManager.Instance.IsBossFloor(floor - 1);
            int rollbackFloor = BattleDataManager.Instance.GetLastBossCheckpoint(floor);
            if (lostAfterBoss)
                rollbackFloor = BattleDataManager.Instance.GetLastBossCheckpoint(rollbackFloor - 1);

            BattleDataManager.Instance.currentFloor = Mathf.Max(rollbackFloor, 1);
        }

        BattleDataManager.Instance.SaveProgress();

        battleController.StartBattle(); // restart battle on new floor
    }

}
