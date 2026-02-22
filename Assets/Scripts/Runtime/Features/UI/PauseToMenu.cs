using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Features.UI
{
    public class PauseToMenu : MonoBehaviour
    {
        [SerializeField] private string menuSceneName = "Menu";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene(menuSceneName);
        }
    }
}
