using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    public Button button;
    public Text label;

    public void SetWeapon(string name)
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