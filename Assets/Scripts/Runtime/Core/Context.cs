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
        [SerializeField] private int totalSlots = 3;

        public GlobalConfig Config { get; private set; }
        public SaveStorage Saves { get; private set; }
        public int ActiveSlotIndex { get; private set; } = 1;
        public int TotalSlots => Mathf.Max(1, totalSlots);

        private void Awake()
        {
            if (I != null) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);
            Config = GlobalConfigStorage.Load();
            ActiveSlotIndex = Mathf.Clamp(Config.activeSlot, 1, TotalSlots);
            if (Config.activeSlot != ActiveSlotIndex)
            {
                Config.activeSlot = ActiveSlotIndex;
            }
            Saves = new SaveStorage(ActiveSlotIndex);
        }

        public void SetActiveSlot(int slot)
        {
            ActiveSlotIndex = Mathf.Clamp(slot, 1, TotalSlots);
            if (Config == null)
                Config = GlobalConfigStorage.Load();
            Config.activeSlot = ActiveSlotIndex;
            Saves = new SaveStorage(ActiveSlotIndex);
        }
    }
}
