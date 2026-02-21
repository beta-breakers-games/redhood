using System.Collections;
using UnityEngine;

namespace Runtime.Features.World
{
    public class PushTree2D : MonoBehaviour, IInteractable2D
    {
        public string id;
        public bool fallRight = true;
        public Transform pivot;
        public float fallAngle = 90f;
        public float fallDuration = 0.5f;
        public Collider2D standingCollider;
        public Collider2D bridgeCollider;

        [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private bool _isFalling;
        private bool _isFallen;

        private void Awake()
        {
            if (bridgeCollider != null)
            {
                bridgeCollider.enabled = false;
                bridgeCollider.gameObject.SetActive(false);
            }
        }

        public bool Interact(GameObject interactor)
        {
            if (_isFalling || _isFallen)
                return false;

            StartCoroutine(FallRoutine());
            return true;
        }

        private IEnumerator FallRoutine()
        {
            _isFalling = true;

            Transform stand = standingCollider != null ? standingCollider.transform : transform;
            Vector3 pivotPos = pivot != null ? pivot.position : stand.position;
            Vector3 startPos = stand.position;
            Quaternion startRot = stand.rotation;
            Vector3 offset = startPos - pivotPos;

            float signedAngle = fallRight ? -Mathf.Abs(fallAngle) : Mathf.Abs(fallAngle);
            float duration = Mathf.Max(0.01f, fallDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float eased = fallCurve != null ? fallCurve.Evaluate(t) : t;
                float angle = signedAngle * eased;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);

                stand.rotation = rot * startRot;
                stand.position = pivotPos + (rot * offset);

                elapsed += Time.deltaTime;
                yield return null;
            }

            Quaternion finalRot = Quaternion.Euler(0f, 0f, signedAngle);
            stand.rotation = finalRot * startRot;
            stand.position = pivotPos + (finalRot * offset);

            if (standingCollider != null)
                standingCollider.enabled = false;

            if (bridgeCollider != null)
            {
                bridgeCollider.gameObject.SetActive(true);
                bridgeCollider.enabled = true;
            }

            _isFalling = false;
            _isFallen = true;
        }
    }
}
