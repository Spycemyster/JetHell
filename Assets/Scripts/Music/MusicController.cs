using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
     private AudioSource m_audioSource;

     private static bool m_alreadyActive = false;

     private void Awake()
     {
        if (m_alreadyActive)
        {
            Destroy(gameObject);
        }
        m_alreadyActive = true;
        DontDestroyOnLoad(transform.gameObject);
        m_audioSource = GetComponent<AudioSource>();
     }
 
     public void PlayMusic()
     {
         if (m_audioSource.isPlaying) return;
         m_audioSource.Play();
     }
 
     public void StopMusic()
     {
        m_audioSource.Stop();
     }

     public void RestartMusic()
     {
        m_audioSource.Play();
     }
}
