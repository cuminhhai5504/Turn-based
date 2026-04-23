using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PickupPopupUI : MonoBehaviour
{
    public Text text;

    public void Show(string message, Vector3 worldPos)
    {
        // 👉 convert world → screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;

        text.text = message;

        // reset
        transform.localScale = Vector3.one * 0.8f;
        

        // 🔥 animation
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(transform.position.y + 50f, 1f));
        seq.Join(transform.DOScale(1f, 0.2f));
        seq.Join(text.DOFade(1, 0.2f));

        seq.AppendInterval(0.5f);

        seq.Append(text.DOFade(0, 2f));

        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}