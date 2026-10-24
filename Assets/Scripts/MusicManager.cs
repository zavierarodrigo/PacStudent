using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip ghostNormalMusic;
    [SerializeField] private float maxIntroTime = 3f;

    void Start()
    {
        audioSource.clip = introMusic;
        audioSource.loop = false;
        audioSource.Play();
        Invoke(nameof(SwitchToGhostMusic), maxIntroTime);
    }

    void SwitchToGhostMusic()
    {
        audioSource.clip = ghostNormalMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
