using UnityEngine;

namespace Runtime.Features.World
{
    public class SlowZone2D : MonoBehaviour
    {
        [Range(0.05f, 2f)] public float slowScale = 0.7f;
        private float _defaultFixedDeltaTime;

        void Awake()
        {
            _defaultFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Time.timeScale = slowScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * Time.timeScale;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
        }
    }
}