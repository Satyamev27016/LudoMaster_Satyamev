using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioClip stepSound; // Short step sound (0.2-0.3 seconds)
    [Header("Sound Variation")]
    [Range(0.8f, 1.2f)] public float pitchVariationRange = 0.2f;
    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false; // No looping for step sounds
        audioSource.volume = volume;
    }

    public void PlayStepSound()
    {
        if (audioSource != null && stepSound != null)
        {
            // Add pitch variation to make it less repetitive
            float randomPitch = Random.Range(1f - pitchVariationRange, 1f + pitchVariationRange);

            // Create temporary audio source for pitch variation without affecting main source
            GameObject tempAudio = new GameObject("TempStepSound");
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = stepSound;
            tempSource.volume = volume;
            tempSource.pitch = randomPitch;
            tempSource.Play();

            // Destroy temp object after sound finishes
            Destroy(tempAudio, stepSound.length / randomPitch + 0.1f);
        }
    }

    // Method for synchronized movement sounds
    public void PlayMovementSounds(int numberOfSteps, float stepInterval)
    {
        StartCoroutine(PlayStepSequence(numberOfSteps, stepInterval));
    }

    private IEnumerator PlayStepSequence(int steps, float interval)
    {
        for (int i = 0; i < steps; i++)
        {
            PlayStepSound();
            yield return new WaitForSeconds(interval);
        }
    }

    // Simple volume control
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
            audioSource.volume = volume;
    }
}