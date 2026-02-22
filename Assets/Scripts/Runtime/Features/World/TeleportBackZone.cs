using UnityEngine;
using Runtime.Features.Player;

namespace Runtime.Features.World
{
    [RequireComponent(typeof(Collider2D))]
    public class TeleportBackZone2D : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private CameraRespawnPan cameraRespawnPan;
        public AudioSource screamAudio;

        private void Reset()
        {
            var c = GetComponent<Collider2D>();
            c.isTrigger = true;
        }

        private void Awake()
        {
            if (cameraRespawnPan == null)
            {
                var mainCam = Camera.main;
                if (mainCam != null)
                    cameraRespawnPan = mainCam.GetComponent<CameraRespawnPan>();
                if (cameraRespawnPan == null)
                    Debug.LogError($"{nameof(TeleportBackZone2D)} requires a {nameof(CameraRespawnPan)} reference on the main camera.", this);
            }

            if (cameraRespawnPan == null)
                Debug.LogError($"{nameof(TeleportBackZone2D)} requires a {nameof(CameraRespawnPan)} reference.", this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (screamAudio)
                screamAudio.Play();
            // If collider is on child, GetComponentInParent is safer:
            Player.Player player = other.GetComponentInParent<Player.Player>();
            if (player == null){
                Debug.Log("No Player reference, cannot pan camera on respawn.", this);   
                return;
            }
            if (cameraRespawnPan == null)
            {
                Debug.Log("No CameraRespawnPan reference, cannot pan camera on respawn.", this);
                return;
            }
            StartCoroutine(cameraRespawnPan.PanAndRespawn(player));
        }
    }
}
