using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public AudioSource audioSource; // kam se má zvuk přehrát
    public AudioClip hoverClip;     // zvuk při najetí myší

    // Tato funkce se zavolá, když myš najede na tlačítko
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }
}
