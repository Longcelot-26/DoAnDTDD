using UnityEngine;

public class AudioManager : MonoBehaviour
{
 [SerializeField] private AudioSource effectAudioSource;
 [SerializeField] private AudioClip shootClip;
 [SerializeField] private AudioSource defaultAudioSource;
 [SerializeField] private AudioSource gameOverAudioSource;
 [SerializeField] private AudioSource MenuTheme;
 public void PlayShootSound()
 {
     effectAudioSource.PlayOneShot(shootClip);
 }
 public void PlayDefaultSound()
 {
    MenuTheme.Stop();
    gameOverAudioSource.Stop();
     defaultAudioSource.Play();
 }
 public void StopDefaultSound()
 {
     defaultAudioSource.Stop();
     effectAudioSource.Stop();
     gameOverAudioSource.Stop();
     MenuTheme.Play();
 }
 public void PlayGameOverSound()
 {
     defaultAudioSource.Stop();
     MenuTheme.Stop();
     gameOverAudioSource.Play();
 }
}
