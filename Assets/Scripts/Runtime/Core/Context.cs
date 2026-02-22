using UnityEngine;

namespace Runtime.Core
{
    public class GameContext : MonoBehaviour
    /*
     Ensure GameContext exists before anything calls GameContext.I 
     (put it in the first scene, or make a Bootstrap scene).
     Don’t create another GameContext in later scenes (the Awake guard destroys duplicates).
    */
    {
        public static GameContext I { get; private set; }
        public Core.SaveStorage Saves { get; private set; }
        public int ActiveSlot { get; private set; } = 1;

        private void Awake()
        {
            if (I != null) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);
            Saves = new SaveStorage();
            ActiveSlot = GlobalConfigStorage.Load().activeSlot;
        }

        public void SetActiveSlot(int slot)
        {
            ActiveSlot = Mathf.Clamp(slot, 1, 3);
            var cfg = GlobalConfigStorage.Load();
            cfg.activeSlot = ActiveSlot;
            GlobalConfigStorage.Save(cfg);
        }
    }
}