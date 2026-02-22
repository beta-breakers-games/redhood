using System.Collections;
using UnityEngine;
using Runtime.Features.Player;

namespace Runtime.Features.World
{
    public class CameraRespawnPan : MonoBehaviour
    {
        [SerializeField] float panSpeed = 6f;
        [SerializeField] float holdAfterPan = 0.2f;

        public IEnumerator PanAndRespawn(Player.Player player)
        {
            Time.timeScale = 0f;

            Vector3 targetCamPos = new Vector3(
                player.lastStandingPosition.x,
                player.lastStandingPosition.y,
                transform.position.z
            );

            // Smooth pan using unscaled time
            while (Vector3.Distance(transform.position, targetCamPos) > 0.05f)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetCamPos,
                    panSpeed * Time.unscaledDeltaTime
                );

                yield return null;
            }

            transform.position = targetCamPos;

            // Small pause before teleport
            yield return new WaitForSecondsRealtime(holdAfterPan);

            // Teleport player
            player.TeleportToLastSafe();

            Time.timeScale = 1f;
        }
    }
}
