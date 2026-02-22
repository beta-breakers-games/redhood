using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Features.World
{
    [RequireComponent(typeof(Collider2D))]
    public class WinZone2D : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string winSceneName = "you_win";
        [SerializeField] private float delaySec = 0f;
        [SerializeField] private bool disablePlayerOnWin = true;

        private bool _triggered;

        private void Reset()
        {
            var c = GetComponent<Collider2D>();
            c.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered || !other.CompareTag(playerTag)) return;
            _triggered = true;

            var player = other.GetComponentInParent<Runtime.Features.Player.Player>();
            if (disablePlayerOnWin && player != null)
                player.enabled = false;

            if (delaySec > 0f)
                StartCoroutine(LoadAfterDelay());
            else
                SceneManager.LoadScene(winSceneName);
        }

        private IEnumerator LoadAfterDelay()
        {
            yield return new WaitForSeconds(delaySec);
            SceneManager.LoadScene(winSceneName);
        }
    }
}
