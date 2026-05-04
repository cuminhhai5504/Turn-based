using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI text;

    private List<string> lines;
    private int index;

    bool isActive = false;

    public void StartDialogue(List<string> dialogueLines)
    {
        lines = dialogueLines;
        index = 0;
        isActive = true;

        panel.SetActive(true);
        ShowLine();

        // 🔒 khóa game
        TurnManager.Instance.isUIBlocking = true;
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        if (index < lines.Count)
        {
            text.text = lines[index];
        }
    }

    void NextLine()
    {
        index++;

        if (index >= lines.Count)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
        }
    }

    void EndDialogue()
    {
        panel.SetActive(false);
        isActive = false;

        // 🔓 mở lại game
        TurnManager.Instance.isUIBlocking = false;
    }
}