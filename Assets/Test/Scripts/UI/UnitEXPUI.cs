using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitEXPUI : MonoBehaviour
{
    public GameObject expBarRoot;
    public Slider slider;
    public Text expText;

    public IEnumerator AnimateEXP(int from, int to, int max)
    {
        expBarRoot.SetActive(true);

        float t = 0;
        float duration = 0.5f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float value = Mathf.Lerp(from, to, t / duration);
            slider.value = value / max;
            expText.text = (int)value + "/" + max;

            yield return null;
        }

        slider.value = (float)to / max;
        expText.text = to + "/" + max;
    }

    public IEnumerator HideAfter()
    {
        yield return new WaitForSeconds(2f);
        expBarRoot.SetActive(false);
    }
}