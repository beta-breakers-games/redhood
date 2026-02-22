using UnityEngine;



[RequireComponent(typeof(Collider2D))]
public class TeleportBackZone2D : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // If collider is on child, GetComponentInParent is safer:
        var player = other.GetComponentInParent<Player>();
        if (player != null)
            player.TeleportToLastSafe();
    }
}
