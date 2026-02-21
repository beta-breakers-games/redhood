using System.Collections;
using UnityEngine;

namespace Runtime.Features.World
{
    public class FallingTreeBridge : MonoBehaviour, IInteractable2D
    {
        [Header("Rotation")]
        [SerializeField] private Transform rotateRoot;
        [SerializeField] private float fallAngle = -90f;
        [SerializeField] private float fallDuration = 0.75f;
        [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Colliders")]
        [SerializeField] private Collider2D[] enableWhenFallen;
        [SerializeField] private Collider2D[] disableWhenFallen;

        [Header("State")]
        [SerializeField] private bool disableAfterFall = true;

        private bool _isFalling;
        private bool _isFallen;

        private Transform RotateRoot => rotateRoot != null ? rotateRoot : transform;

        private void Awake()
        {
            if (enableWhenFallen == null)
                return;

            for (int i = 0; i < enableWhenFallen.Length; i++)
            {
                if (enableWhenFallen[i] != null)
                    enableWhenFallen[i].enabled = false;
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

            var root = RotateRoot;
            Quaternion start = root.rotation;
            Quaternion target = start * Quaternion.Euler(0f, 0f, fallAngle);

            float time = 0f;
            float duration = Mathf.Max(0.01f, fallDuration);

            while (time < duration)
            {
                float t = time / duration;
                float eased = fallCurve != null ? fallCurve.Evaluate(t) : t;
                root.rotation = Quaternion.SlerpUnclamped(start, target, eased);
                time += Time.deltaTime;
                yield return null;
            }

            root.rotation = target;
            SetFallenState();

            _isFalling = false;
            _isFallen = true;

            if (disableAfterFall)
                enabled = false;
        }

        private void SetFallenState()
        {
            if (enableWhenFallen != null)
            {
                for (int i = 0; i < enableWhenFallen.Length; i++)
                {
                    if (enableWhenFallen[i] != null)
                        enableWhenFallen[i].enabled = true;
                }
            }

            if (disableWhenFallen != null)
            {
                for (int i = 0; i < disableWhenFallen.Length; i++)
                {
                    if (disableWhenFallen[i] != null)
                        disableWhenFallen[i].enabled = false;
                }
            }
        }
    }
}
