using UnityEngine;
using Runtime.Core;

namespace Runtime.Features.World
{
    public class DarknessControl: MonoBehaviour
    {
        private GameContext _context;
        [Header("Start")]
        [SerializeField] private Transform startPoint;

        [Header("Motion")]
        [SerializeField] private float speed = 0.8f;
        [SerializeField] private float repelAnimationDurationSec = 1f;
        [SerializeField] private float repelStunDurationSec = 0.5f;
        [SerializeField] private bool loadFromSave = true;

        [SerializeField] private bool enableLogs = false;

        private float _x;
        private float _y;
        private Coroutine _repelRoutine;
        private float _stunTimer;
        
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

            if (enableLogs && _context != null)
                Debug.Log($"DarknessControl initialized with slot {_context.Saves.Slot}", this);
        }

        private void Update()
        {
            if (_repelRoutine != null)
                return;
            if (_stunTimer > 0f)
            {
                _stunTimer -= Time.deltaTime;
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
    }
}
