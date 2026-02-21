using UnityEngine;
using Runtime.Features.World;

namespace Runtime.Features.Player
{
    public class PlayerInteractor2D : MonoBehaviour
    {
        [Header("Input")]
        public KeyCode interactKey = KeyCode.E;

        [Header("Detection")]
        public float interactRadius = 1.8f;
        public LayerMask interactMask;

        [Header("Debug")]
        public bool enableDebugLogs = false;
        public bool showGizmosAlways = false;

        private readonly Collider2D[] _hits = new Collider2D[16];

        private void Update()
        {
            if (!Input.GetKeyDown(interactKey))
                return;

            int count = interactMask == 0
                ? Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, _hits)
                : Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, _hits, interactMask);

            if (count == 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"{name} Interact: no targets in radius.", this);
                return;
            }

            float bestSqr = float.PositiveInfinity;
            IInteractable2D best = null;
            GameObject bestGo = null;

            for (int i = 0; i < count; i++)
            {
                var col = _hits[i];
                if (col == null)
                    continue;

                var interactable = col.GetComponentInParent<IInteractable2D>();
                if (interactable == null)
                    continue;

                float sqr = (col.transform.position - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = interactable;
                    bestGo = col.gameObject;
                }
            }

            if (best == null)
            {
                if (enableDebugLogs)
                    Debug.Log($"{name} Interact: no interactable components found.", this);
                return;
            }
            bool did = best.Interact(gameObject);
            if (enableDebugLogs)
            {
                var targetName = bestGo != null ? bestGo.name : "unknown";
                Debug.Log($"{name} Interact: {(did ? "used" : "ignored")} {targetName}.", this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(false);
        }

        private void OnDrawGizmos()
        {
            if (showGizmosAlways)
                DrawGizmos(true);
        }

        private void DrawGizmos(bool always)
        {
            if (!always && !showGizmosAlways)
                return;

            Gizmos.color = new Color(1f, 0.7f, 0.1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }
}
