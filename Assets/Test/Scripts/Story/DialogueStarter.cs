using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public MapDialogue mapDialogue;

    void Start()
    {
        if (mapDialogue != null && mapDialogue.dialogueLines.Count > 0)
        {
            dialogueUI.StartDialogue(mapDialogue.dialogueLines);
        }
    }
}