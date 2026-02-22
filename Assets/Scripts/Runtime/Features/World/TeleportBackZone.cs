using UnityEngine;

namespace Runtime.Features.World
{
    [RequireComponent(typeof(Collider2D))]
    public class TeleportBackZone2D : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private CameraRespawnPan cameraRespawnPan;
        [SerializeField] private AudioSource screamAudio;
        private bool _isHandlingTeleport;

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
            if (_isHandlingTeleport) return;
            StartCoroutine(HandleTeleport(other));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private System.Collections.IEnumerator HandleTeleport(Collider2D other)
        {
            _isHandlingTeleport = true;
            // If collider is on child, GetComponentInParent is safer:
            Player.Player player = other.GetComponentInParent<Player.Player>();
            if (player == null){
                Debug.Log("No Player reference, cannot pan camera on respawn.", this);   
                _isHandlingTeleport = false;
                yield break;
            }
            if (cameraRespawnPan == null)
            {
                Debug.Log("No CameraRespawnPan reference, cannot pan camera on respawn.", this);
                _isHandlingTeleport = false;
                yield break;
            }

            if (screamAudio != null)
            {
                screamAudio.Play();
                if (screamAudio.clip != null)
                    yield return new WaitForSeconds(screamAudio.clip.length);
            }

            yield return cameraRespawnPan.PanAndRespawn(player);
            _isHandlingTeleport = false;
        }
    }
}
