using UnityEngine;
using Runtime.Features.Player;

namespace Runtime.Features.World
{
    public class PushTree2D : MonoBehaviour, IInteractable
    {
        [Header("Save ID (unique & stable)")] [SerializeField]
        private string id;

        [Header("Fall (fixed)")] [SerializeField]
        private bool fallRight = true; // set in Inspector; never changes at runtime

        [SerializeField] private Transform pivot;
        [SerializeField] private float fallAngle = 90f;
        [SerializeField] private float fallDuration = 0.5f;

        [SerializeField] private Collider2D standingCollider;
        [SerializeField] private Collider2D bridgeCollider;

        private bool _fallen;
        private Quaternion _startRot;
        private Quaternion _targetRot;

        public string Id => id;
        public bool IsFallen => _fallen;
        public bool FallRight => fallRight;

        private void Awake()
        {
            if (!pivot) pivot = transform;
            _startRot = pivot.rotation;
            if (bridgeCollider) bridgeCollider.enabled = false;
        }

        public void Interact(PlayerInteractor2D player)
        {
            if (_fallen) return;
            BeginFall(animated: true);
        }

        public void ApplySavedState(bool fallen)
        {
            _fallen = fallen;
            if (_fallen) SetFallenInstant();
            else SetStanding();
        }

        private void BeginFall(bool animated)
        {
            _fallen = true;
            if (standingCollider) standingCollider.enabled = false;

            _targetRot = ComputeTargetRotation();

            if (!animated)
            {
                SetFallenInstant();
                return;
            }

            StopAllCoroutines();
            StartCoroutine(FallRoutine());
        }

        private Quaternion ComputeTargetRotation()
        {
            float dir = fallRight ? -1f : 1f; // flip sign if your sprite rotates opposite
            return _startRot * Quaternion.Euler(0f, 0f, dir * fallAngle);
        }

        private void SetFallenInstant()
        {
            pivot.rotation = ComputeTargetRotation();
            if (standingCollider) standingCollider.enabled = false;
            if (bridgeCollider) bridgeCollider.enabled = true;
        }

        private void SetStanding()
        {
            pivot.rotation = _startRot;
            if (standingCollider) standingCollider.enabled = true;
            if (bridgeCollider) bridgeCollider.enabled = false;
        }

        private System.Collections.IEnumerator FallRoutine()
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fallDuration;
                float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
                pivot.rotation = Quaternion.Slerp(_startRot, _targetRot, eased);
                yield return null;
            }

            pivot.rotation = _targetRot;
            if (bridgeCollider) bridgeCollider.enabled = true;
        }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id))
            id = System.Guid.NewGuid().ToString("N");
    }
#endif
    }
}