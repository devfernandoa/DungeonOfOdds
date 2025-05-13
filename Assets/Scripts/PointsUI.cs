using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    private void Update()
    {
        pointsText.text = $"Points: {PointsManager.Instance.currentPoints}";
    }
}
