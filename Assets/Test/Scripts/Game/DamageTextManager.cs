using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    public GameObject damageTextPrefab;
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

    public void ShowDamage(int damage, Vector3 worldPos)
    {

        if (damageTextPrefab == null || canvas == null)
        {
            Debug.LogError("❌ Missing prefab or canvas!");
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        

        GameObject obj = Instantiate(damageTextPrefab, canvas.transform);
        obj.transform.position = screenPos;

        var dt = obj.GetComponent<DamageText>();
        if (dt != null)
            dt.Setup(damage);
    }
}