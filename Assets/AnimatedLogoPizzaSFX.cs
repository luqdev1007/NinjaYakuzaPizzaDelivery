using UnityEngine;

public class AnimatedLogoPizzaSFX : MonoBehaviour
{
    public AudioClip SliceSFX;
    public AudioClip HookLaunchSFX;
    public AudioClip HookLoopSFX;
    public AudioClip CloseBoxSFX;

    public AudioSource AudioSource;

    public void PlaySliceSound()
    {
        AudioSource.pitch = 2 * Random.Range(0.7f, 1.5f);
        AudioSource.PlayOneShot(SliceSFX);
    }

    public void PlayHookLaucnSound()
    {
        AudioSource.pitch = 1;
        AudioSource.volume = 0.3f;
        AudioSource.PlayOneShot(HookLaunchSFX);
    }

    public void PlayHookLoopSound()
    {
        AudioSource.pitch = 1;
        AudioSource.PlayOneShot(HookLoopSFX);
    }

    public void PlayCloseBoxSound()
    {
        AudioSource.pitch = 1;
        AudioSource.PlayOneShot(CloseBoxSFX);
    }
}
