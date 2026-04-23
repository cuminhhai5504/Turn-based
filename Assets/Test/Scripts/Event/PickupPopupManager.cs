using UnityEngine;

public class PickupPopupManager : MonoBehaviour
{
    public static PickupPopupManager Instance;

    public GameObject popupPrefab;
    public Canvas canvas;

    void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(string message, Vector3 worldPos)
    {
        GameObject obj = Instantiate(popupPrefab, canvas.transform);
        obj.GetComponent<PickupPopupUI>().Show(message, worldPos);
    }
}