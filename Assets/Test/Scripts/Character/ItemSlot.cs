using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Button button;
    public Text label;

    public void SetItem(string name)
    {
        label.text = name;
        button.interactable = true;
    }

    public void SetEmpty()
    {
        label.text = "---";
        button.interactable = false;
    }
}