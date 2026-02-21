using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] grassSteps;
    public AudioClip[] gravelSteps;
    public AudioClip[] mudSteps;

    public void PlayFootstep(string surfaceTag)
    {
        AudioClip[] clips = gravelSteps;

        switch (surfaceTag)
        {
            case "Grass":
                clips = grassSteps;
                break;

            case "Gravel":
                clips = gravelSteps;
                break;

            case "Mud":
                clips = mudSteps;
                break;
        }

        if (clips == null || clips.Length == 0) return;

            audioSource.PlayOneShot(
        clips[Random.Range(0, clips.Length)]
    );
}
}
