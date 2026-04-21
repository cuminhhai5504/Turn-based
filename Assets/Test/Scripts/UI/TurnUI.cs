using UnityEngine;
using TMPro;
using System.Collections;

public class TurnUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI turnText;

    public float displayTime = 1.5f;

    public void ShowTurn(int turnNumber)
    {
        StartCoroutine(ShowTurnCoroutine(turnNumber));
    }

    IEnumerator ShowTurnCoroutine(int turnNumber)
    {
        panel.SetActive(true);
        turnText.text = "Lượt " + turnNumber;

        yield return new WaitForSeconds(displayTime);

        panel.SetActive(false);
    }
}