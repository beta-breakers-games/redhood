using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Core
{
    public class BootstrapLoader : MonoBehaviour
    {
        [SerializeField] private string firstScene = "Menu";

        private async void Start()
        {
            var op = SceneManager.LoadSceneAsync(firstScene);
            while (!op.isDone)
                await System.Threading.Tasks.Task.Yield();
        }
    }
}

