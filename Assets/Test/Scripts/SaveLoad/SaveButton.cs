using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void OnClickSave()
    {
        SaveLoadManager.Instance.SaveGame();
    }
}