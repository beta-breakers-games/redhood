using UnityEngine;
using Runtime.Core;

namespace Runtime.Features.World
{
    public class DarknessControl: MonoBehaviour
    {
        private GameContext _context;
        [SerializeField] private bool enableLogs = false;
        
        private void Start()
        {
            _context = GameContext.I;
            if (_context == null)
            {
                Debug.LogError("GameContext not initialized yet.");
                return;
            }
            if (enableLogs)
                Debug.Log($"DarknessControl initialized with {_context.Saves.Slot}");
            
        }
    }
}