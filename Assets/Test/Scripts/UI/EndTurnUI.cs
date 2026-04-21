using UnityEngine;
using TMPro;
using System.Collections;

public class EndTurnUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI text;
    public float displayTime = 1f;

    public void ShowEndTurn(int turnNumber)
    {
        StartCoroutine(ShowEndTurnCoroutine(turnNumber));
    }    
    public IEnumerator ShowEndTurnCoroutine(int turnNumber)
    {
        panel.SetActive(true);
        text.text = "Kết thúc lượt " + turnNumber;

        yield return new WaitForSeconds(displayTime);

        panel.SetActive(false);

    }
}