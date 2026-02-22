using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Features.World
{
    public class LoseMessageUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private RectTransform rect;
        [SerializeField] private float fadeIn = 0.35f;
        [SerializeField] private float hold = 0.4f;

        private void Reset()
        {
            group = GetComponent<CanvasGroup>();
            rect = GetComponent<RectTransform>();
        }

        public void Show()
        {
            StopAllCoroutines();
            StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            group.alpha = 0f;
            rect.localScale = Vector3.one * 0.9f;

            float t = 0f;
            while (t < fadeIn)
            {
                t += Time.deltaTime;
                float u = Mathf.SmoothStep(0f, 1f, t / fadeIn);
                group.alpha = u;
                rect.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, u);
                yield return null;
            }

            yield return new WaitForSeconds(hold);
        }
    }
}