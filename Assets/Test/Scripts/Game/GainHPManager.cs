using UnityEngine;

public class GainHPManager : MonoBehaviour
{
    public static GainHPManager Instance;

    public GameObject gainHPPrefab;
    public Canvas canvas;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowGainHP(int value, Vector3 worldPos)
    {

        if (gainHPPrefab == null || canvas == null)
        {
            Debug.LogError("❌ Missing prefab or canvas!");
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);


        GameObject obj = Instantiate(gainHPPrefab, canvas.transform);
        obj.transform.position = screenPos;

        var dt = obj.GetComponent<GainHPText>();
        if (dt != null)
            dt.Setup(value);
    }
}
