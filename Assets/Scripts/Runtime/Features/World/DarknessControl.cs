using UnityEngine;
using UnityEngine.SceneManagement;
using Runtime.Core;

namespace Runtime.Features.World
{
    public class DarknessControl: MonoBehaviour
    {
        private GameContext _context;
        [Header("Start")]
        [Tooltip("Mandatory. Initial spawn position for darkness.")]
        [SerializeField] private Transform startPoint;
        [Tooltip("Optional. When darkness reaches/passes this X, it snaps and stops.")]
        [SerializeField] private Transform stopPoint;

        [Header("Motion")]
        [Tooltip("Forward movement speed (units per second).")]
        [SerializeField] private float speed = 0.8f;
        [Tooltip("Time in seconds to animate the repel pushback.")]
        [SerializeField] private float repelAnimationDurationSec = 1f;
        [Tooltip("Extra pause after repel completes (seconds).")]
        [SerializeField] private float repelStunDurationSec = 0.5f;
        [Tooltip("If true, loads saved darkness position on start (when a save exists).")]
        [SerializeField] private bool loadFromSave = true;

        [Header("Player")]
        [Tooltip("Optional. Player component reference for distance checks.")]
        [SerializeField] private Runtime.Features.Player.Player player;
        [Tooltip("Used to auto-find player if Player reference is empty.")]
        [SerializeField] private string playerTag = "Player";

        [Header("Lose")]
        [Tooltip("If enabled, triggers lose when within loseDistance of player.")]
        [SerializeField] private bool enableLoseCheck = true;
        [Tooltip("Distance at or below which the player is considered caught.")]
        [SerializeField] private float loseDistance = 1f;
        [Tooltip("Delay before loading the lose scene (seconds).")]
        [SerializeField] private float loseDelaySec = 1f;
        [Tooltip("Disable player movement while waiting to load the lose scene.")]
        [SerializeField] private bool disablePlayerOnLose = true;
        [Tooltip("Scene name to load when player is caught.")]
        [SerializeField] private string loseSceneName = "you_lost";

        [Tooltip("Enable debug logs for setup and lose events.")]
        [SerializeField] private bool enableLogs = false;

        private float _x;
        private float _y;
        private Coroutine _repelRoutine;
        private float _stunTimer;
        private bool _hasLost;
        private Coroutine _loseRoutine;
        
        private void Awake()
        {
            if (startPoint == null)
            {
                Debug.LogError($"{nameof(DarknessControl)} requires a startPoint.", this);
                enabled = false;
                return;
            }

            _x = startPoint.position.x;
            _y = startPoint.position.y;
            transform.position = new Vector3(_x, _y, transform.position.z);
        }

        private void Start()
        {
            _context = GameContext.I;
            if (_context == null)
            {
                if (enableLogs)
                    Debug.LogWarning("GameContext not initialized yet.", this);
            }
            else if (loadFromSave && _context.Saves.HasSave())
            {
                var data = _context.Saves.Load();
                _x = data.darknessPosition.x;
                _y = data.darknessPosition.y;
                transform.position = new Vector3(_x, _y, transform.position.z);
            }

            if (player == null && !string.IsNullOrEmpty(playerTag))
            {
                var go = GameObject.FindGameObjectWithTag(playerTag);
                if (go != null)
                    player = go.GetComponentInParent<Runtime.Features.Player.Player>();
            }

            if (enableLogs && _context != null)
                Debug.Log($"DarknessControl initialized with slot {_context.Saves.Slot}", this);
        }

        private void Update()
        {
            if (!_hasLost && enableLoseCheck && GetDistanceToPlayer() <= loseDistance)
            {
                TriggerLose();
                return;
            }
            if (_repelRoutine != null)
                return;
            if (_stunTimer > 0f)
            {
                _stunTimer -= Time.deltaTime;
                return;
            }
            if (stopPoint != null && _x >= stopPoint.position.x)
            {
                _x = stopPoint.position.x;
                transform.position = new Vector3(_x, _y, transform.position.z);
                return;
            }
            _x += speed * Time.deltaTime;
            transform.position = new Vector3(_x, _y, transform.position.z);
        }

        public void Repel(float strength)
        {
            _stunTimer = 0f;
            if (repelAnimationDurationSec <= 0f)
            {
                _x -= strength;
                transform.position = new Vector3(_x, _y, transform.position.z);
                _stunTimer = Mathf.Max(0f, repelStunDurationSec);
                return;
            }

            if (_repelRoutine != null)
                StopCoroutine(_repelRoutine);
            _repelRoutine = StartCoroutine(RepelOverTime(strength));
        }

        private System.Collections.IEnumerator RepelOverTime(float strength)
        {
            float duration = Mathf.Max(0.01f, repelAnimationDurationSec);
            float remaining = strength;
            while (remaining > 0f)
            {
                float step = (strength / duration) * Time.deltaTime;
                if (step > remaining)
                    step = remaining;
                _x -= step;
                remaining -= step;
                transform.position = new Vector3(_x, _y, transform.position.z);
                yield return null;
            }
            _repelRoutine = null;
            _stunTimer = Mathf.Max(0f, repelStunDurationSec);
        }

        public float GetDistanceToPlayer()
        {
            if (player == null)
                return float.PositiveInfinity;
            return Vector2.Distance(transform.position, player.transform.position);
        }

        public void TriggerLose()
        {
            if (_hasLost)
                return;
            _hasLost = true;
            if (player != null)
            {
                if (stopPoint == null)
                    stopPoint = player.transform;
                else
                    stopPoint.position = player.transform.position;
                _x = player.transform.position.x;
                transform.position = new Vector3(_x, _y, transform.position.z);
                if (disablePlayerOnLose)
                    player.enabled = false;
            }
            if (enableLogs)
                Debug.Log("DarknessControl: player caught, loading lose scene.", this);
            if (_loseRoutine != null)
                StopCoroutine(_loseRoutine);
            _loseRoutine = StartCoroutine(LoadLoseSceneAfterDelay());
        }

        private System.Collections.IEnumerator LoadLoseSceneAfterDelay()
        {
            if (loseDelaySec > 0f)
                yield return new WaitForSeconds(loseDelaySec);
            if (!string.IsNullOrEmpty(loseSceneName) && CanLoadScene(loseSceneName))
            {
                SceneManager.LoadScene(loseSceneName);
            }
            else if (CanLoadScene("Menu"))
            {
                SceneManager.LoadScene("Menu");
            }
            else if (enableLogs)
            {
                Debug.LogWarning($"DarknessControl: no valid lose scene to load ('{loseSceneName}').", this);
            }
        }

        private static bool CanLoadScene(string sceneName)
        {
            return Application.CanStreamedLevelBeLoaded(sceneName);
        }
    }
}
