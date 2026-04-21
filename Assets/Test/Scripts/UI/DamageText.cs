using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float moveDistance = 50f;
    public float duration = 1f;

    public void Setup(int damage)
    {
        text.text = "-" + damage;

        PlayEffect();
    }

    void PlayEffect()
    {
        RectTransform rect = GetComponent<RectTransform>();

        // 🎲 random lệch ngang
        float randomX = Random.Range(-30f, 30f);

        // 🎯 vị trí target
        Vector3 targetPos = rect.anchoredPosition + new Vector2(randomX, moveDistance);

        // 🔥 SEQUENCE DOTWEEN
        Sequence seq = DOTween.Sequence();

        // 1️⃣ scale pop
        rect.localScale = Vector3.zero;
        seq.Append(rect.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));

        // 2️⃣ scale về bình thường
        seq.Append(rect.DOScale(1f, 0.1f));

        // 3️⃣ bay lên + fade cùng lúc
        seq.Join(rect.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic));
        seq.Join(text.DOFade(0, duration));

        // 4️⃣ destroy
        seq.OnComplete(() => Destroy(gameObject));
    }
}