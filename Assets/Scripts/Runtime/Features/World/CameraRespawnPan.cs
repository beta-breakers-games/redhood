using System.Collections;
using UnityEngine;
using Runtime.Features.Player;

namespace Runtime.Features.World
{
    public class CameraRespawnPan : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 6f;
        [SerializeField] private float holdAfterPan = 0.2f;
        [SerializeField] private float maxPanSeconds = 2.5f;
        [SerializeField] private bool pauseTime = true;
        [SerializeField] private bool enableLogs = false;

        private bool _isPanning;
        private float _previousTimeScale = 1f;
        private float _previousFixedDeltaTime = 0.02f;

        public IEnumerator PanAndRespawn(Player.Player player)
        {
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] PanAndRespawn called for player {player.name}.", this);
            if (!player)
                yield break;
    
            BeginPan();


            Vector3 targetCamPos = new Vector3(
                player.lastStandingPosition.x,
                player.lastStandingPosition.y,
                transform.position.z
            );
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] Start pan. Target={targetCamPos} PauseTime={pauseTime}", this);

            // Smooth pan using unscaled time
            float elapsed = 0f;
            while (Vector3.Distance(transform.position, targetCamPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetCamPos,
                    panSpeed * Time.unscaledDeltaTime
                );

                elapsed += Time.unscaledDeltaTime;
                if (maxPanSeconds > 0f && elapsed >= maxPanSeconds)
                    break;

                yield return null;
            }

            transform.position = targetCamPos;
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] Reached target in {elapsed:0.00}s (max {maxPanSeconds:0.00}s).", this);

            // Small pause before teleport
            yield return new WaitForSecondsRealtime(holdAfterPan);

            // Teleport player
            player.TeleportToLastSafe();
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] Teleported player to last standing position.", player);

            EndPan();
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] End pan. TimeScale restored={pauseTime}.", this);
        }

        private void BeginPan()
        {
            if (_isPanning)
                return;

            _isPanning = true;
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] BeginPan()", this);
            if (!pauseTime)
                return;

            _previousTimeScale = Time.timeScale;
            _previousFixedDeltaTime = Time.fixedDeltaTime;
            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0f;
            if (enableLogs)
                Debug.Log($"[{nameof(CameraRespawnPan)}] Time paused. PrevScale={_previousTimeScale} PrevFixedDelta={_previousFixedDeltaTime}", this);
        }

        private void EndPan()
        {
            if (!_isPanning)
                return;

            if (pauseTime)
            {
                Time.timeScale = _previousTimeScale;
                Time.fixedDeltaTime = _previousFixedDeltaTime;
                if (enableLogs)
                    Debug.Log($"[{nameof(CameraRespawnPan)}] Time restored. Scale={Time.timeScale} FixedDelta={Time.fixedDeltaTime}", this);
            }

            _isPanning = false;
        }

        private void OnDisable()
        {
            if (_isPanning)
            {
                if (enableLogs)
                    Debug.LogWarning($"[{nameof(CameraRespawnPan)}] OnDisable while panning. Forcing EndPan().", this);
                EndPan();
            }
        }

        private void OnDestroy()
        {
            if (_isPanning)
            {
                if (enableLogs)
                    Debug.LogWarning($"[{nameof(CameraRespawnPan)}] OnDestroy while panning. Forcing EndPan().", this);
                EndPan();
            }
        }
    }
}
