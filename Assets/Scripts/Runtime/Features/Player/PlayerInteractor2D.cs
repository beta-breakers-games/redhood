using UnityEngine;

namespace Runtime.Features.Player
{
    public class PlayerInteractor2D : MonoBehaviour
    {
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private float interactRadius = 1.2f;
        [SerializeField] private LayerMask interactMask;

        private void Update()
        {
            if (!Input.GetKeyDown(interactKey)) return;

            var hit = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask);
            if (!hit) return;

            var interactable = hit.GetComponent<IInteractable>();
            interactable?.Interact(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
#endif
    }

    public interface IInteractable
    {
        void Interact(PlayerInteractor2D player);
    }
}