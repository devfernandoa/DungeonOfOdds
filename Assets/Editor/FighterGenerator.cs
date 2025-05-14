#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FighterGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Common Fighters")]
    public static void GenerateFighters()
    {
        string path = "Assets/Fighters/Common/";

        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Fighters", "Common");
        }

        List<Fighter> fightersToCreate = new()
        {
            NewFighter("Bow", 5, 10, 200, 1.5f, 5, true, 400, 5),
            NewFighter("Sword", 10, 10, 200, 1.5f, 5, false, 100, 5),
            NewFighter("Club", 12, 7, 150, 1.6f, 3, false, 90, 0),
            NewFighter("Dagger", 8, 9, 250, 1.2f, 4, false, 70, 0),
            NewFighter("Spear", 10, 8, 200, 1.4f, 3, false, 120, 0),
            NewFighter("Axe", 11, 11, 180, 1.6f, 3, false, 100, 0),
            NewFighter("Flail", 13, 6, 160, 1.8f, 2, false, 100, 0),
            NewFighter("Mace", 14, 5, 140, 2.0f, 1, false, 100, 0),
            NewFighter("Halberd", 12, 9, 160, 1.6f, 2, false, 140, 0),
            NewFighter("Hammer", 15, 4, 130, 2.2f, 1, false, 80, 0),
            NewFighter("Pitchfork", 10, 6, 180, 1.6f, 3, true, 250, 5),
            NewFighter("Slingshot", 6, 8, 210, 1.4f, 5, true, 300, 6),
            NewFighter("Blowgun", 4, 9, 230, 1.1f, 6, true, 350, 7),
            NewFighter("Javelin", 7, 10, 200, 1.7f, 3, true, 400, 6),
            NewFighter("Throwing Axe", 9, 7, 200, 1.5f, 4, true, 280, 5),
            NewFighter("Chakram", 8, 7, 200, 1.3f, 5, true, 320, 6),
            NewFighter("Boomerang", 7, 6, 220, 1.5f, 5, true, 300, 5),
            NewFighter("Crossbow", 5, 11, 150, 2.0f, 2, true, 420, 7),
            NewFighter("Shuriken", 5, 8, 250, 1.0f, 6, true, 280, 6),
            NewFighter("Rapier", 9, 9, 220, 1.3f, 4, false, 90, 0),
            NewFighter("Spiked Shield", 13, 6, 140, 1.8f, 2, false, 80, 0),
            NewFighter("Trident", 10, 9, 190, 1.5f, 3, false, 120, 0),
            NewFighter("Whip", 8, 7, 200, 1.4f, 5, true, 220, 5),
            NewFighter("Hook", 9, 6, 210, 1.6f, 4, false, 100, 0),
            NewFighter("Tanto", 7, 10, 240, 1.2f, 5, false, 60, 0),
            NewFighter("Sickle", 9, 7, 200, 1.5f, 4, false, 90, 0),
            NewFighter("Needle Gun", 4, 10, 230, 1.1f, 6, true, 380, 8),
            NewFighter("Torch", 8, 5, 200, 1.4f, 5, true, 200, 4),
            NewFighter("Throwing Knife", 6, 9, 230, 1.3f, 5, true, 260, 6),
            NewFighter("Nunchaku", 10, 8, 210, 1.5f, 4, false, 85, 0)
        };

        foreach (var fighter in fightersToCreate)
        {
            string assetPath = path + fighter.fighterName + ".asset";
            AssetDatabase.CreateAsset(fighter, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Common Fighters Generated!");
    }

    private static Fighter NewFighter(string name, int hp, int atk, float moveSpeed, float cooldown, int luck, bool isRanged, float range, float projSpeed)
    {
        Fighter f = ScriptableObject.CreateInstance<Fighter>();
        f.fighterName = name;
        f.health = hp;
        f.attack = atk;
        f.moveSpeed = moveSpeed;
        f.attackCooldown = cooldown;
        f.luck = luck;
        f.isRanged = isRanged;
        f.range = range;
        f.projectileSpeed = projSpeed;
        f.rarity = Fighter.Rarity.Common;
        return f;
    }

    [MenuItem("Tools/Generate Rare Fighters")]
    public static void GenerateRareFighters()
    {
        string path = "Assets/Fighters/Rare/";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Fighters", "Rare");
        }

        List<Fighter> fightersToCreate = new()
        {
            NewRare("Katana", 13, 13, 240f, 1.2f, 6, false, 110, 0),
            NewRare("Recurve Bow", 8, 14, 220f, 1.5f, 7, true, 450, 7),
            NewRare("Battle Axe", 18, 12, 160f, 1.8f, 3, false, 110, 0),
            NewRare("Warhammer", 22, 9, 130f, 2.2f, 2, false, 100, 0),
            NewRare("Boom Lance", 14, 13, 200f, 1.5f, 5, true, 350, 6),
            NewRare("Twin Daggers", 11, 11, 260f, 1.0f, 8, false, 85, 0),
            NewRare("Repeater Crossbow", 9, 12, 200f, 1.3f, 6, true, 420, 7),
            NewRare("Meteor Staff", 10, 15, 180f, 2.0f, 4, true, 480, 5),
            NewRare("Falchion", 14, 12, 210f, 1.4f, 5, false, 100, 0),
            NewRare("Whip Blade", 12, 10, 220f, 1.4f, 6, true, 250, 6),
            NewRare("Throwing Stars", 10, 13, 230f, 1.2f, 7, true, 280, 6),
            NewRare("Explosive Flask", 9, 14, 180f, 1.7f, 6, true, 300, 5),
            NewRare("Magic Baton", 8, 11, 250f, 1.3f, 9, true, 370, 6),
            NewRare("Saber", 13, 13, 230f, 1.3f, 5, false, 90, 0),
            NewRare("Trick Cards", 7, 12, 260f, 1.1f, 10, true, 260, 7)
        };

        foreach (var fighter in fightersToCreate)
        {
            string assetPath = path + fighter.fighterName + ".asset";
            AssetDatabase.CreateAsset(fighter, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Rare Fighters Generated with scaled moveSpeed!");
    }

    private static Fighter NewRare(string name, int hp, int atk, float moveSpeed, float cooldown, int luck, bool isRanged, float range, float projSpeed)
    {
        Fighter f = ScriptableObject.CreateInstance<Fighter>();
        f.fighterName = name;
        f.health = hp;
        f.attack = atk;
        f.moveSpeed = moveSpeed; // already scaled above
        f.attackCooldown = cooldown;
        f.luck = luck;
        f.isRanged = isRanged;
        f.range = range;
        f.projectileSpeed = projSpeed;
        f.rarity = Fighter.Rarity.Rare;
        return f;
    }

    [MenuItem("Tools/Generate Epic Fighters")]
    public static void GenerateEpicFighters()
    {
        string path = "Assets/Fighters/Epic/";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Fighters", "Epic");
        }

        List<Fighter> fightersToCreate = new()
        {
            NewEpic("Flaming Scythe", 22, 20, 190f, 1.4f, 7, false, 120, 0),
            NewEpic("Arcane Staff", 18, 25, 200f, 1.6f, 10, true, 500, 6),
            NewEpic("Dragon Lance", 24, 22, 180f, 1.5f, 6, true, 420, 7),
            NewEpic("Runic Blade", 26, 21, 200f, 1.3f, 9, false, 130, 0),
            NewEpic("Volt Chakram", 16, 19, 230f, 1.1f, 11, true, 400, 8),
            NewEpic("Celestial Bow", 14, 26, 210f, 1.8f, 8, true, 600, 9),
            NewEpic("Phantom Dagger", 15, 23, 240f, 1.0f, 10, false, 90, 0),
            NewEpic("Tidal Trident", 20, 20, 190f, 1.5f, 9, true, 350, 6),
            NewEpic("Gravity Hammer", 30, 18, 160f, 2.2f, 6, false, 110, 0),
            NewEpic("Spectral Cards", 13, 21, 250f, 1.1f, 12, true, 300, 7)
        };

        foreach (var fighter in fightersToCreate)
        {
            string assetPath = path + fighter.fighterName + ".asset";
            AssetDatabase.CreateAsset(fighter, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✨ Epic Fighters Generated!");
    }

    private static Fighter NewEpic(string name, int hp, int atk, float moveSpeed, float cooldown, int luck, bool isRanged, float range, float projSpeed)
    {
        Fighter f = ScriptableObject.CreateInstance<Fighter>();
        f.fighterName = name;
        f.health = hp;
        f.attack = atk;
        f.moveSpeed = moveSpeed;
        f.attackCooldown = cooldown;
        f.luck = luck;
        f.isRanged = isRanged;
        f.range = range;
        f.projectileSpeed = projSpeed;
        f.rarity = Fighter.Rarity.Epic;
        return f;
    }

    [MenuItem("Tools/Generate Legendary Fighters")]
    public static void GenerateLegendaryFighters()
    {
        string path = "Assets/Fighters/Legendary/";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Fighters", "Legendary");
        }

        List<Fighter> fightersToCreate = new()
        {
            NewLegendary("Excalibur", 35, 35, 300f, 1.2f, 100, false, 130, 0),
            NewLegendary("Heavenly Longbow", 20, 42, 320f, 1.6f, 100, true, 650, 10),
            NewLegendary("Doom Halberd", 40, 30, 280f, 1.4f, 100, false, 150, 0),
            NewLegendary("Astral Scepter", 22, 45, 310f, 1.7f, 100, true, 700, 12),
            NewLegendary("Starshard Fan", 18, 38, 340f, 1.3f, 100, true, 480, 9)
        };

        foreach (var fighter in fightersToCreate)
        {
            string assetPath = path + fightersToCreate.IndexOf(fightersToCreate.Find(f => f.fighterName == fighter.fighterName)) + "_" + fighter.fighterName + ".asset";
            AssetDatabase.CreateAsset(fighter, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("👑 Legendary Fighters Generated!");
    }

    private static Fighter NewLegendary(string name, int hp, int atk, float moveSpeed, float cooldown, int luck, bool isRanged, float range, float projSpeed)
    {
        Fighter f = ScriptableObject.CreateInstance<Fighter>();
        f.fighterName = name;
        f.health = hp;
        f.attack = atk;
        f.moveSpeed = moveSpeed;
        f.attackCooldown = cooldown;
        f.luck = luck;
        f.isRanged = isRanged;
        f.range = range;
        f.projectileSpeed = projSpeed;
        f.rarity = Fighter.Rarity.Legendary;
        return f;
    }


}
#endif
