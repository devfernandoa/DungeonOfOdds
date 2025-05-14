using UnityEditor;
using UnityEngine;

public class UnlockFighterDebugWindow : EditorWindow
{
    private Vector2 scrollPos;

    [MenuItem("Debug Tools/Unlock Fighters")]
    public static void ShowWindow()
    {
        GetWindow<UnlockFighterDebugWindow>("Unlock Fighters");
    }

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("This tool only works in Play Mode.", MessageType.Info);
            return;
        }

        if (BattleDataManager.Instance == null || BattleDataManager.Instance.allAvailableFighters == null)
        {
            EditorGUILayout.LabelField("No BattleDataManager or fighter list available.");
            return;
        }

        EditorGUILayout.LabelField("Click to Unlock Fighters", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var fighter in BattleDataManager.Instance.allAvailableFighters)
        {
            if (fighter == null) continue;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(fighter.fighterName);

            if (GUILayout.Button("Unlock", GUILayout.Width(80)))
            {
                AvailableFightersPanel.Instance.AddFighterToList(fighter);
                BattleDataManager.Instance.SaveUnlockedFighters();
                Debug.Log($"Unlocked {fighter.fighterName}");
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
