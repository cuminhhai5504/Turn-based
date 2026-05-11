using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public MapDialogue mapDialogue;

    void Start()
    {
        // 🔥 nếu dialogue đã chạy thì bỏ qua
        if (SaveLoadManager.Instance.isMapDialogueDone(mapDialogue.dialogueID))
            return;

        if (mapDialogue != null && mapDialogue.dialogueLines.Count > 0)
        {
            dialogueUI.StartDialogue(mapDialogue.dialogueLines);
            // 🔥 đánh dấu đã chạy
            SaveLoadManager.Instance.MarkEventUsed(mapDialogue.dialogueID);
        }
    }
}